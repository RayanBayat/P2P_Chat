using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2P_Chat.Models
{

    public class oneConversation
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<Message> listOfMessages;

        public List<Message> ListOfMessages
        {
            get { return listOfMessages; }
            set { listOfMessages = value; }
        }


        public oneConversation(string name, List<Message> listOfMessages)
        {
            this.name = name;
            this.listOfMessages = listOfMessages;
        }
    }

}
