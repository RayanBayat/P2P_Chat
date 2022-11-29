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


namespace P2P_Chat.Models
{

    public class FileWriter
    {
        private JObject conversations;
        //private JArray oneConvo = new JArray();
        //private JArray allConvos = new JArray();
        public FileWriter()
        {
            //Skapar folder
            Directory.CreateDirectory(@"c:\TDDD49STORAGE");

            //Skapar fil
            if (!File.Exists(@"c:\TDDD49STORAGE\conversations.json"))
            {
                File.WriteAllText(@"c:\TDDD49STORAGE\conversations.json", String.Empty);
            }

            if (File.ReadAllText(@"c:\TDDD49STORAGE\conversations.json") == String.Empty)
            {
                //Debug.WriteLine("created conversations object");
                conversations = new JObject(
                    new JProperty("conversations", new JArray()));
            }
            else
            {
                conversations = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(@"c:\TDDD49STORAGE\conversations.json"));
                MessageBox.Show(conversations.ToString());
                //Debug.WriteLine(conversations.ToString());

            }
        }

        public void WriteToFile(JObject jsonObj)
        {


            JArray arrayOfConvos = (JArray)conversations["conversations"];
            JObject conversation = (JObject)arrayOfConvos.Last;
            JArray aConvo = (JArray)conversation["convo"];
            aConvo.Add(jsonObj);

            // MessageBox.Show(allConvos.ToString());

            File.WriteAllText(@"TDDD49STORAGE\conversations.json", conversations.ToString());
        }

        public void InitConversation(String name)
        {
            //Debug.WriteLine("Initialised conversations with name " + name);
            JArray arrayOfConvos = (JArray)conversations["conversations"];
            arrayOfConvos.Add(new JObject(
                new JProperty("name", name),
                new JProperty("convo", new JArray())));
        }

        public List<Conversation> GetHistory()
        {
            //gå igenom alla objekt i conversations JArray
            //konverta till conversations-objekt och lägg till i lista
            //retunerna lista
            //Debug.WriteLine("Entering getHistory");
            List<Conversation> aList = new List<Conversation>();

            foreach (JObject value in conversations["conversations"])
            {
                
                var messageList = value["convo"].Value<JArray>();
                MessageBox.Show(messageList.ToString());
                List<Message> messages = messageList.ToObject<List<Message>>();
                aList.Add(new Conversation((string)value["name"], messages));
            }

           // MessageBox.Show(aList[0].ToString());
            //Debug.WriteLine(aList.ToString());
            aList.Reverse();
            return aList;


        }

        public Conversation GetLatestConvo()
        {
            JArray arrayOfConvos = (JArray)conversations["conversations"];
            JObject lastObj = (JObject)arrayOfConvos.Last;
            var messageList = lastObj["convo"].Value<JArray>();
            List<Message> messages = messageList.ToObject<List<Message>>();
            return new Conversation((string)lastObj["name"], messages);
        }
    }

}
