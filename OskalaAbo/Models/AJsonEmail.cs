
using System.Collections.Generic;public class AJsonEmail
{
    public class To
    {
        public string email { get; set; }
        public string name { get; set; }
    }
    public class Personalization
    {
        public List<To> to { get; set; }
        public List<To> cc { get; set; }
        public List<To> bcc { get; set; }
        public string subject { get; set; }
        public Dictionary<string, string> headers { get; set; }
    }
    public class Content
    {
        public string type { get; set; }
        public string value { get; set; }
    }
    public class From
    {
        public string email { get; set; }
        public string name { get; set; }
    }
    public class Attachments
    {
        public string content { get; set; }
        public string filename { get; set; }
        public string type { get; set; }
        public string disposition { get; set; } = "attachment"; // /inline public string content_id { get; set; }
    }
    public class ReplyTo
    {
        public string email { get; set; }
        public string name { get; set; }
    }
    public class AJsonEmailContent
    {
        public List<Personalization> personalizations { get; set; }
        public List<Content> content { get; set; }
        public List<Attachments> attachments { get; set; }
        public From from { get; set; }
        public ReplyTo reply_to { get; set; }
        public Dictionary<string, string> headers { get; set; }
        public List<string> categories { get; set; }
        public Dictionary<string, string> custom_args { get; set; }
        public AJsonEmailContent()
        {
            personalizations = new List<Personalization>();
            content = new List<Content>();
            from = new From();
        }
    }
}

