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

        private void ws_Opened(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                tbxMessageWindow.AppendText("[Connection established]\n");
            }));
        }

        private void ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                Message message = SimpleJson.SimpleJson.DeserializeObject<Message>(e.Message);

                tbxMessageWindow.Text += message.message + "\n";
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
                this.ws.Send(tbxMessage.Text);
            }));
        }
    }
}
