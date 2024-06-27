using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice.Dictation;
using Meta.Voice.Samples.Dictation;
using Meta.WitAi.Dictation;
using UnityEngine.Networking;
using System.Text;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private DictationService witDictation;
    public DictationActivation dictationActivation;
    public AppDictationExperience appDictationExperience;
    public buddyController buddyController;
    public AudioSource audioSource;

    // OpenAI API Key - set via the Unity Editor
    public string authToken = "";
    public RequestTranscription multiRequestTranscription;

    List<string> messages = new List<string>()
        {
            "{\"role\": \"system\", \"content\": \"You're a helpful & cheery assistant called 'Buddy'. You respond in a friendly and engaging tone. e.g: user: Hey how's it going. assistant: Great thank you, how are you :).  .\"}",
        };

    public void Awake()
    {
        if (!witDictation) witDictation = FindObjectOfType<DictationService>();
        witDictation.DictationEvents.OnFullTranscription.AddListener(GotFullTranscript);
        witDictation.DictationEvents.OnPartialTranscription.AddListener(GotPartialTranscript);
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
        var completionsEndpoint = "https://api.openai.com/v1/chat/completions";
        messages.Add(",{\"role\": \"user\", \"content\": \"" + transcript + "\"}");
        var json = new StringBuilder();
        json.AppendLine("{");
        json.AppendLine("\"model\": \"gpt-4o\",");
        json.AppendLine("\"messages\": [");
        foreach (var message in messages)
        {
            json.AppendLine(message);
        }
        json.AppendLine("]");
        json.AppendLine("}");
        Debug.Log(json.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(completionsEndpoint, json.ToString(), "application/json"))
        {
            www.SetRequestHeader("Authorization", "Bearer " + authToken);
            www.SetRequestHeader("Content-Type", "application/json");
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
                Debug.Log(responseText);
                multiRequestTranscription._text.AppendLine();
                multiRequestTranscription._text.AppendLine();
                multiRequestTranscription._text.Append("Buddy:" + responseText);
                multiRequestTranscription.OnTranscriptionUpdated();

                StartCoroutine(TtsRequest(responseText));
                messages.Add(",{\"role\": \"user\", \"content\": \"" + responseText + "\"}");
            }
        }
    }
    IEnumerator TtsRequest(string response)
    {
        var audioEndpoint = "https://api.openai.com/v1/audio/speech";

        var json = new StringBuilder();
        json.AppendLine("{");
        json.AppendLine("\"model\": \"tts-1\",");
        json.AppendLine("\"input\": \"" + response + "\",");
        json.AppendLine("\"voice\": \"alloy\",");
        json.AppendLine("\"response_format\": \"wav\"");
        json.AppendLine("}");
        Debug.Log(json.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(audioEndpoint, json.ToString(), "application/json"))
        {
            www.SetRequestHeader("Authorization", "Bearer " + authToken);
            www.SetRequestHeader("Content-Type", "application/json");
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
                    buddyController = FindObjectOfType<buddyController>();
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

    public void StartRecording()
    {
        Debug.Log("Start recording");
        dictationActivation.ToggleActivation();
    }

    public void StopRecording()
    {
        dictationActivation.ToggleActivation();
        Debug.Log("Stop recording");
    }
}