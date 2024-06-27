using System.Collections.Generic;

namespace OpenAI
{
    [System.Serializable]
    public class OpenAIChatRequest
    {
        public string model { get; set; }
        public List<ChoiceMessage> messages { get; set; }
    }

    public class OpenAITextToSpeechRequest
    {
        public string model { get; set; }
        public string input { get; set; }
        public string voice { get; set; }
        public string response_format { get; set; }

        public OpenAITextToSpeechRequest(string model, string input, string voice, string response_format)
        {
            this.model = model;
            this.input = input;
            this.voice = voice;
            this.response_format = response_format;
        }
    }
}