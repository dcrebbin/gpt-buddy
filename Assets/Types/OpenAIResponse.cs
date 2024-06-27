
using System.Collections.Generic;


namespace OpenAI
{


    [System.Serializable]
    public class OpenAIResponse
    {
        public string id { get; set; }

        public string created { get; set; }

        public string model { get; set; }

        public List<Choice> choices { get; set; }

        public Usage usage { get; set; }

        public string system_fingerprint { get; set; }

        public OpenAIResponse()
        {

        }

        public OpenAIResponse(string id, string created, string model, List<Choice> choices, Usage usage, string system_fingerprint)
        {
            this.id = id;
            this.created = created;
            this.model = model;
            this.choices = choices;
            this.usage = usage;
            this.system_fingerprint = system_fingerprint;
        }
    }

    public class Usage
    {
        public string prompt_tokens { get; set; }
        public string completion_tokens { get; set; }
        public string total_tokens { get; set; }
    }

    public class Choice
    {
        public ChoiceMessage message { get; set; }
        public string index { get; set; }
        public string logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class ChoiceMessage
    {
        public string role { get; set; }
        public string content { get; set; }

        public ChoiceMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}