using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickMailGenerator
{
    [JsonObject]
    public class Settings
    {
        [JsonProperty("menu")]
        public List<MenuItem> MenuItems { get; set; }
        [JsonProperty("general")]
        public GeneralSettings GeneralSettings { get; set; }
        [JsonProperty("templates")]
        public List<Template> Templates { get; set; }
    }


    [JsonObject]
    public class MenuItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    [JsonObject]
    public class GeneralSettings
    {
        [JsonProperty("input")]
        public List<Input> Inputs { get; set; }
    }

    [JsonObject]
    public class Template
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("to")]
        public string MailTo { get; set; }
        [JsonProperty("cc")]
        [DefaultValue("")]
        public string MailCc { get; set; }
        [JsonProperty("bcc")]
        [DefaultValue("")]
        public string MailBcc { get; set; }
        [JsonProperty("subject")]
        public string MailSubject { get; set; }
        [JsonProperty("body")]
        public List<string> MailBody { get; set; }
        [JsonProperty("input")]
        public List<Input> Inputs { get; set; }
    }

    public enum InputType
    {
        Null,
        Singleline,
        Multiline
    }

    [JsonObject]
    public class Input
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("default")]
        public string Default { get; set; }
        [JsonProperty("type")]
        public InputType Type { get; set; }
    }
}
