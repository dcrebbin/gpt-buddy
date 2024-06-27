using System.Collections.Generic;

namespace OpenAI
{
    [System.Serializable]
    public class OpenAIChatRequest
    {
        public string model { get; set; }
        public List<ChoiceMessage> messages { get; set; }
    }
}