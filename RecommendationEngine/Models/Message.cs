using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created_At { get; set; }
        public string Assistant_Id { get; set; }
        public string Thread_Id { get; set; }
        public string Run_Id { get; set; }
        public string Role { get; set; }
        public List<ContentItem> Content { get; set; }
        public List<string> File_Ids { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class ContentItem
    {
        public string Type { get; set; }
        public TextContent Text { get; set; }
    }

    public class TextContent
    {
        public string Value { get; set; }
        public List<object> Annotations { get; set; }
    }

    public class MessageResponse
    {
        public string Object { get; set; }
        public List<Message> Data { get; set; }
        public string first_id { get; set; }
        public string last_id { get; set; }
        public bool has_more { get; set; }
    }
}
