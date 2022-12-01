using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Text.RegularExpressions;

namespace P2P_Chat.Models
{
    public class FileWriter
    {
        private JObject conversations;
        public FileWriter()
        {
            //Skapar folder
            Directory.CreateDirectory(@"chatstoratge");

            //Skapar fil
            if (!File.Exists(@"chatstoratge\conversations.json"))
            {
                File.WriteAllText(@"chatstoratge\conversations.json", String.Empty);
            }

            if (File.ReadAllText(@"chatstoratge\conversations.json") == String.Empty)
            {
                //Debug.WriteLine("created conversations object");
                conversations = new JObject(
                    new JProperty("conversations", new JArray()));
            }
            else
            {
                conversations = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"chatstoratge\conversations.json"));
            }
        }
        public void WriteToFile(JObject jsonObj)
        {
            JArray arrayOfConvos = (JArray)conversations["conversations"];
            JObject conversation = (JObject)arrayOfConvos.Last;
            JArray aConvo = (JArray)conversation["conversation"];
            aConvo.Add(jsonObj);
            File.WriteAllText(@"chatstoratge\conversations.json", conversations.ToString());
        }
        public void InitConversation(String name)
        {
            JArray arrayOfConvos = (JArray)conversations["conversations"];
            arrayOfConvos.Add(new JObject(
                new JProperty("name", name),
                new JProperty("conversation", new JArray())));
        }
        public List<Conversation> GetHistory()
        {
            List<Conversation> aList = new List<Conversation>();
            foreach (JObject person in conversations["conversations"])
            {                
                var messageList = person["conversation"].Value<JArray>();
                List<Message> messages = messageList.ToObject<List<Message>>();
                aList.Add(new Conversation((string)person["name"], messages));                
            }
            aList.Reverse();
            return aList;
        }
        public IEnumerable<Conversation> filter(string search)
        {
            var myRegex = new Regex("^" + search, RegexOptions.IgnoreCase);
            IEnumerable<Conversation> conversations = from conversation in GetHistory() where myRegex.IsMatch(conversation.Name) select conversation;
            return conversations;
        }
    }

}
