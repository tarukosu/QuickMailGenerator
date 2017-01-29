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
        [JsonProperty("title")]
        public string MailTitle { get; set; }
        [JsonProperty("content")]
        public string MailContent { get; set; }
        [JsonProperty("input")]
        public List<Input> Inputs { get; set; }
    }

    public enum InputType
    {
        Singleline,
        Multiline
    }

    [JsonObject]
    public class Input
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        [DefaultValue("")]
        public string Description { get; set; }
        [JsonProperty("default")]
        [DefaultValue("")]
        public string Default { get; set; }
        [JsonProperty("type")]
        [DefaultValue(InputType.Singleline)]
        public InputType Type { get; set; }
    }
}
