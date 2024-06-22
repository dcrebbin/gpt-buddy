using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Voice.Dictation;
using Meta.WitAi.Events.UnityEventListeners;
using Meta.Voice.Samples.Dictation;
using Meta.WitAi.Events;
using Meta.WitAi.Dictation;
using TMPro;
using UnityEngine.Networking;
using Oculus.Platform;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

class OpenAIResponse
{
    public string id { get; set; }

    public string created { get; set; }

    public string model { get; set; }

    public List<Choice> choices { get; set; }
}

class Choice
{
    public Message2 message { get; set; }
    public string index { get; set; }
    public string logprobs { get; set; }
    public string finish_reason { get; set; }
}

class Message2
{
    public string role { get; set; }
    public string content { get; set; }
}

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private DictationService witDictation;

    public DictationActivation dictationActivation;
    public AppDictationExperience appDictationExperience;

    public TMP_Text text;

    List<string> messages = new List<string>()
        {
            "{\"role\": \"system\", \"content\": \"You are a helpful assistant.\"}",
        };

    private void Awake()
    {
        if (!witDictation) witDictation = FindObjectOfType<DictationService>();

        witDictation.DictationEvents.OnFullTranscription.AddListener(GotFullTranscript);
        witDictation.DictationEvents.OnPartialTranscription.AddListener(GotPartialTranscript);

    }
    public void GotFullTranscript(string transcript)
    {
        Debug.Log("Transcript: " + transcript);
        // Update the transcript text
        StartCoroutine(ChatCompletionRequest(transcript));
    }


    IEnumerator ChatCompletionRequest(string transcript)
    {
        var completionsEndpoint = "https://api.openai.com/v1/chat/completions";

        messages.Add(",{\"role\": \"user\", \"content\": \"" + transcript + "\"}");
        var json = new StringBuilder();
        json.AppendLine("{");
        json.AppendLine("\"model\": \"gpt-3.5-turbo-16k\",");
        json.AppendLine("\"messages\": [");
        foreach (var message in messages)
        {
            json.AppendLine(message);
        }
        json.AppendLine("]");
        json.AppendLine("}");
        Debug.Log(json.ToString());
        var authToken = "";

        using (UnityWebRequest www = UnityWebRequest.Post(completionsEndpoint, json.ToString(), "application/json"))
        {
            www.SetRequestHeader("Authorization", "Bearer " + authToken);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                var stringResponse = www.downloadHandler.text;
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<OpenAIResponse>(stringResponse);
                Debug.Log(response.created);
                Debug.Log(response.choices[0].message.content);
                text.text += "\n" + "Response: " + response.choices[0].message.content;
                messages.Add(",{\"role\": \"assistant\", \"content\": \"" + response.choices[0].message.content + "\"}");
            }
        }
    }

    public void GotPartialTranscript(string transcript)
    {
        Debug.Log("Partial transcript: " + transcript);
        // Update the transcript text
    }

    public void StartRecording()
    {
        Debug.Log("Start recording");
        dictationActivation.ToggleActivation();
        // Start recording
    }

    public void StopRecording()
    {
        dictationActivation.ToggleActivation();

        Debug.Log("Stop recording");
        // Stop recording
    }
}