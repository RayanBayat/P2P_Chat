using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace P2P_Chat.Models
{
    public class Message
    {
        public string? jsrequesttype { get; set; }

        public string? jsname { get; set; }
        public string? jsmsg { get; set; }
        public string? jstime { get; set; }


        public JObject msgToJson()
        {
            return new JObject(
                    new JProperty("jsrequesttype", jsrequesttype),
                    new JProperty("jsname", jsname),
                    new JProperty("jsmsg", jsmsg),
                    new JProperty("jstime", jstime)
                    );
        }

    }
}
