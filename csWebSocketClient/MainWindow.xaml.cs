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
        private string[] colorArray = new string[] { "007AFF", "FF7000", "FF7000", "15E25F", "CFC700", "CFC700", "CF1100", "CF00BE", "F00" };
        private Random r = new Random();
        private int id = 0;
        
        private Dictionary<string, Brush> colorDic = new Dictionary<string,Brush>();
        
        public MainWindow()
        {
            InitializeComponent();

            this.colorArray = this.colorArray.Distinct().ToArray();

            foreach (string item in colorArray)
            {
                string extendedItem = "#" + item;
                BrushConverter converter = new BrushConverter();
                Brush brush = (Brush)converter.ConvertFromString(extendedItem);

                this.colorDic.Add(item, brush);   
            }

            this.id = this.r.Next(0, this.colorArray.Length);
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
            this.connectToHost();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            this.sendMessage();
        }

        private void ws_Opened(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                rtbxMessageWindow.AppendText("[Connection established]\n");
                this.isConnected = true;
            }));
        }

        private void ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                Message message = SimpleJson.SimpleJson.DeserializeObject<Message>(e.Message);

                switch (message.type)
	            {
                    case "usermsg":
                        TextRange trTime = new TextRange(rtbxMessageWindow.Document.ContentEnd, rtbxMessageWindow.Document.ContentEnd);
                        trTime.Text = message.time + " ";
                        trTime.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

                        TextRange trName = new TextRange(rtbxMessageWindow.Document.ContentEnd, rtbxMessageWindow.Document.ContentEnd);
                        if (message.name == null)
                        {
                            break;
                        }
                        else
                        {
                            trName.Text = message.name;
                            trName.ApplyPropertyValue(TextElement.ForegroundProperty, this.colorDic[message.color]);
                        }
                        
                        TextRange trMessage = new TextRange(rtbxMessageWindow.Document.ContentEnd, rtbxMessageWindow.Document.ContentEnd);
                        trMessage.Text = ": " + message.message + "\n";
                        trMessage.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

                        rtbxMessageWindow.ScrollToEnd();
                        break;
                    case "system":
                        rtbxMessageWindow.AppendText(message.message + "\n");
                        rtbxMessageWindow.ScrollToEnd();
                        break;
		            default:
                    break;
	            }
            }));
        }

        private void ws_Closed(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                rtbxMessageWindow.AppendText("[Connection closed]\n");
            }));
        }

        private void ws_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                rtbxMessageWindow.AppendText("[Error]: " + e.Exception.Message + "\n");
            }));
        }

        private void connectToHost()
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
                rtbxMessageWindow.AppendText("[Error]: \n");
                rtbxMessageWindow.AppendText(ex.Message + "\n");
            }
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

                message.type = "usermsg";
                message.message = tbxMessage.Text;
                message.time = time.ToLongTimeString();
                message.name = tbxName.Text;
                message.color = this.colorArray[this.id];
                                
                string jsonString = SimpleJson.SimpleJson.SerializeObject(message);

                this.ws.Send(jsonString);
            }));
        }

        private void tbxEnterPress(object sender, KeyEventArgs e)
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

        private void tbxHost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.connectToHost();
            }
        }

        private void tbxMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox tbx = (TextBox)sender;
                tbx.SelectAll();
                tbx.Foreground = Brushes.Black;
            }
        }
    }
}
