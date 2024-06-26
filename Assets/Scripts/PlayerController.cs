using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.Voice.Samples.Dictation;
using Meta.WitAi.Dictation;
using UnityEngine.Networking;
using OpenAI;
using System;

public class PlayerController : MonoBehaviour
{
    private DictationService witDictation;
    private DictationActivation dictationActivation;
    private BuddyController buddyController;
    private AudioSource audioSource;
    private readonly string openAiApiKey = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY") ?? "";
    private RequestTranscription multiRequestTranscription;

    readonly List<ChoiceMessage> messages = new List<ChoiceMessage>(){
        new(){
            role = "system",
            content = "You're a helpful & cheery assistant called 'Buddy'. You respond in a friendly and engaging tone. e.g: user: Hey how's it going. assistant: Great thank you, how are you :)."
        }
    };

    public void Awake()
    {
        AssignVariables();
        SetWitDictationListeners();
    }

    private void SetWitDictationListeners()
    {
        witDictation.DictationEvents.OnFullTranscription.AddListener(GotFullTranscript);
        witDictation.DictationEvents.OnPartialTranscription.AddListener(GotPartialTranscript);
    }

    private void AssignVariables()
    {
        dictationActivation = FindObjectOfType<DictationActivation>();
        audioSource = FindObjectOfType<AudioSource>();
        if (!witDictation) witDictation = FindObjectOfType<DictationService>();
    }

    public void GotFullTranscript(string transcript)
    {
        Debug.Log("Transcript: " + transcript);
        StartCoroutine(ChatCompletionRequest(transcript));
    }

    public void Update()
    {

        if (buddyController)
        {
            if (!audioSource.isPlaying && buddyController.isTalking)
            {
                buddyController.StopTalking();
            }
        }
    }
    IEnumerator ChatCompletionRequest(string transcript)
    {
        if (multiRequestTranscription == null)
        {
            multiRequestTranscription = FindObjectOfType<RequestTranscription>();
        }
        messages.Add(new ChoiceMessage()
        {
            role = "user",
            content = transcript
        });
        var request = new OpenAIChatRequest()
        {
            model = Constants.GPT_4O_MODEL,
            messages = messages
        };

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.COMPLETIONS_ENDPOINT, request.ToString(), Constants.CONTENT_TYPE))
        {
            www.SetRequestHeader("Authorization", "Bearer " + openAiApiKey);
            www.SetRequestHeader("Content-Type", Constants.CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                multiRequestTranscription._text.Append("Chat Error: " + www.error);
                multiRequestTranscription.OnTranscriptionUpdated();

                Debug.LogError(www.error);
            }
            else
            {
                var stringResponse = www.downloadHandler.text;
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenAI.OpenAIResponse>(stringResponse);
                var responseText = response.choices[0].message.content;
                multiRequestTranscription._text.AppendLine();
                multiRequestTranscription._text.AppendLine();
                multiRequestTranscription._text.Append("Buddy:" + responseText);
                multiRequestTranscription.OnTranscriptionUpdated();

                StartCoroutine(TtsRequest(responseText));
                messages.Add(new ChoiceMessage()
                {
                    role = "user",
                    content = responseText
                });
            }
        }
    }
    IEnumerator TtsRequest(string response)
    {
        var audioEndpoint = Constants.TEXT_TO_SPEECH_ENDPOINT;
        var request = new OpenAITextToSpeechRequest(Constants.TTS_1_MODEL, response, Constants.ALLOY_VOICE, Constants.WAV_RESPONSE_FORMAT);

        using (UnityWebRequest www = UnityWebRequest.Post(audioEndpoint, request.ToString(), Constants.CONTENT_TYPE))
        {
            www.SetRequestHeader("Authorization", "Bearer " + openAiApiKey);
            www.SetRequestHeader("Content-Type", Constants.CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                multiRequestTranscription._text.Append("TTS Error: " + www.error);
                multiRequestTranscription.OnTranscriptionUpdated();
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Audio Success");
                var audioClip = CreateAudioClipFromWav(www.downloadHandler.data);
                audioSource.clip = audioClip;
                audioSource.Play();

                if (buddyController == null)
                {
                    buddyController = FindObjectOfType<BuddyController>();
                }
                buddyController.StartTalking();

            }
        }
    }
    AudioClip CreateAudioClipFromWav(byte[] wavFileBytes)
    {
        int sampleRate = System.BitConverter.ToInt32(wavFileBytes, 24);
        int channels = System.BitConverter.ToInt16(wavFileBytes, 22);
        int dataStartIndex = 44;

        int sampleCount = (wavFileBytes.Length - dataStartIndex) / 2;
        float[] audioData = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = System.BitConverter.ToInt16(wavFileBytes, dataStartIndex + i * 2);
            audioData[i] = sample / 32768.0f;
        }

        AudioClip audioClip = AudioClip.Create("GeneratedAudioClip", sampleCount, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);
        return audioClip;
    }

    public void GotPartialTranscript(string transcript)
    {
        Debug.Log("Partial transcript: " + transcript);
    }

    public void StartListening()
    {
        dictationActivation.ToggleActivation();
    }

    public void StopListening()
    {
        dictationActivation.ToggleActivation();
    }
}