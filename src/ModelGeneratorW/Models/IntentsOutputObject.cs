using System.Collections.Generic;
using System.ComponentModel;

namespace ModelGeneratorW.Models
{
    sealed class UserSay
    {
        public string Id { get; set; }
        public List<UserSayData> Data { get; set; }

        public bool IsTemplate { get; set; }
        public int Count { get; set; }

        public UserSay()
        {
            IsTemplate = false;
            Count = 0;
        }
    }

    sealed class UserSayData
    {
        public string Text { get; set; }
        public string Alias { get; set; }
        public string Meta { get; set; }
        public bool? UserDefined { get; set; }
    }

    sealed class IntentContext { }

    sealed class IntentResponse
    {
        public bool ResetContexts { get; set; }
        public List<IntentContext> AffectedContexts { get; set; }
        public List<IntentParameter> Parameters { get; set; }
        public List<IntentMessage> Messages { get; set; }

        public IntentResponse()
        {
            AffectedContexts = new List<IntentContext>();
            Parameters = new List<IntentParameter>();
            Messages = new List<IntentMessage>();
            Messages.Add(new IntentMessage());
        }
    }

    sealed class IntentParameter
    {
        public string DataType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsList { get; set; }

        public IntentParameter(string name)
        {
            Name = name;
            DataType = string.Format("@{0}", name);
            Value = string.Format("${0}", name);
            IsList = false;
        }
    }

    sealed class IntentMessage
    {
        public int Type { get; set; }
        public List<string> Speech { get; set; }
        public IntentMessage()
        {
            Speech = new List<string>();
        }
    }

    sealed class IntentEvent { }

    sealed class IntentsOutputObject
    {
        public List<UserSay> UserSays { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Auto { get; set; }
        public List<IntentContext> Contexts { get; set; }
        public List<IntentResponse> Responses { get; set; }

        public int Priority { get; set; }
        public bool WebhookUsed { get; set; }
        public bool WebhookForSlotFilling { get; set; }
        public bool FallbackIntent { get; set; }
        public List<IntentEvent> Events { get; set; }

        public IntentsOutputObject()
        {
            Auto = true;
            Contexts = new List<IntentContext>();
            Priority = 500000;
            WebhookUsed = false;
            WebhookForSlotFilling = false;
            FallbackIntent = false;
            Events = new List<IntentEvent>();
            Responses = new List<IntentResponse>();
        }
    }



    sealed class IntentOutputObjectW
    {
        public string Value { get; set; }

        public IntentOutputObjectW(string value)
        {
            Value = value;
        }
    }

    sealed class IntentsDataOutputObjectW
    {
        public List<string> Lookups { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        [DefaultValue(true)]
        public bool Exotic { get; set; }
        public string Id { get; set; }

        public List<IntentOutputObjectW> Values { get; set; }

        public string Doc { get; set; }
        [DefaultValue(true)]
        public bool Builtin { get; set; }

        public IntentsDataOutputObjectW()
        {
            Lookups = new List<string>() { "keywords", "free-text" };
            Name = "intent";
            Lang = "en";
            Exotic = false;
            Id = "intent";

            Values = new List<IntentOutputObjectW>();

            Doc = "User-defined entity";
            Builtin = false;
        }
    }

    sealed class IntentsOutputObjectW
    {
        public IntentsDataOutputObjectW Data { get; set; }

        public IntentsOutputObjectW()
        {
            Data = new IntentsDataOutputObjectW();
        }
    }
}
