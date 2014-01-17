using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocket4Net;

namespace csWebSocketClient
{
    class Connect
    {
        // Attributes
        private string uri;
        private WebSocket ws;

        // Properties
        private void setUri(string value)
        {
            // TODO: validate the value
            this.uri = value;
        }
        private void setWs(WebSocket value)
        {
            // TODO: validate the value
            this.ws = value;
        }
        public WebSocket getWs()
        {
            return this.ws;
        }

        // constructor
        public Connect(string hostEntry)
        {
            this.setUri(hostEntry);
            this.establishConnection();
        }

        // Methods
        private void establishConnection()
        {
            if (this.uri == "Host")
            {
                throw new Exception("No valid Hostname entered");
            }
            else
            {
                this.setWs(new WebSocket(this.uri));
                
            }
        }
    }
}
