using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessengerClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void tbxInput_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxInput.Text = "";
            tbxInput.Foreground = Brushes.Black;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            getTextAndSend();
        }

        private void tbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                getTextAndSend();
            }
        }

        private void getTextAndSend()
        {
            string text = tbxInput.Text;
            tbxMessages.AppendText(text + "\n");
            tbxInput.Text = "";

            TcpClient socket = new TcpClient("localhost", 85);

            byte[] data = Encoding.UTF8.GetBytes(text);
            NetworkStream stream = socket.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
