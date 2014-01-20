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
using System.Windows.Threading;
using System.Reflection;
using WebSocket4Net;

namespace csWebSocketClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Connect connect;
        private WebSocket ws;
        private bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void tbx_selectAll(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;
            tbx.SelectAll();
            tbx.Foreground = Brushes.Black;
        }

        private void tbx_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbx = (TextBox)sender;
            BrushConverter converter = new BrushConverter();
            Brush brush = (Brush)converter.ConvertFromString("#FF999999");
            tbx.Foreground = brush;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.connect = new Connect(tbxHost.Text);
                this.ws = connect.getWs();
                this.ws.Closed += new EventHandler(ws_Closed);
                this.ws.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(ws_Error);
                this.ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ws_MessageReceived);
                this.ws.Opened += new EventHandler(ws_Opened);
                this.ws.Open();
            }
            catch (Exception ex)
            {
                tbxMessageWindow.AppendText("[Error]: \n");
                tbxMessageWindow.AppendText(ex.Message + "\n");
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            this.sendMessage();
        }

        private void ws_Opened(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                tbxMessageWindow.AppendText("[Connection established]\n");
                this.isConnected = true;
            }));
        }

        private void ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                Message message = SimpleJson.SimpleJson.DeserializeObject<Message>(e.Message);

                // TODO: check if message attributes are empty
                tbxMessageWindow.Text += message.time + " ";
                tbxMessageWindow.Text += message.name;
                tbxMessageWindow.Text += ": " + message.message + "\n";
                tbxMessageWindow.ScrollToEnd();
            }));
        }

        private void ws_Closed(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                tbxMessageWindow.AppendText("[Connection closed]\n");
            }));
        }

        private void ws_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                tbxMessageWindow.AppendText("[Error]: " + e.Exception.Message + "\n");
            }));
        }

        private void sendMessage()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (this.isConnected == false)
                {
                    MessageBox.Show("Please enter a Host first and click 'Connect'");
                    return;
                }
                
                if (tbxName.Text == "Name")
                {
                    MessageBox.Show("Please enter a Name first");
                    return;
                }
                
                if (tbxMessage.Text == "Message")
                {
                    MessageBox.Show("Please enter a Message first");
                    return;
                }

                // 'type', 'message', 'time', 'name', color', 'clients'
                DateTime time = DateTime.Now;
                Message message = new Message();
                string[] colorArray = new string[] {"007AFF", "FF7000", "FF7000", "15E25F", "CFC700", "CFC700", "CF1100", "CF00BE", "F00"};

                Random r = new Random();
                int id = r.Next(0, colorArray.Length);

                message.type = "usermsg";
                message.message = tbxMessage.Text;
                message.time = time.ToLongTimeString();
                message.name = tbxName.Text;
                message.color = colorArray[id];
                                
                string jsonString = SimpleJson.SimpleJson.SerializeObject(message);

                this.ws.Send(jsonString);
            }));
        }

        private void tbxHost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.sendMessage();
            }
        }

        private void tbxName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                tbxHost.Focus();
            }
        }

        private void tbxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.sendMessage();
            }
        }
    }
}
