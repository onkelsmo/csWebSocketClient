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

namespace csWebSocketClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        




        
    }
}
