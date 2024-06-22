using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Voice.Dictation;
using Meta.WitAi.Events.UnityEventListeners;
using Meta.Voice.Samples.Dictation;
using Meta.WitAi.Events;
using Meta.WitAi.Dictation;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private DictationService witDictation;

    public DictationActivation dictationActivation;
    public AppDictationExperience appDictationExperience;
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
