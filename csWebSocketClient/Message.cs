using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csWebSocketClient
{
    class Message
    {
        // needed attributes
        // 'type', 'message', 'time', 'name', color', 'clients'
        public string type { get; set; }
        public string message { get; set; }
        public string time { get; set; }
        public string color { get; set; }
        //public string clients { get; set; }
    }
}
