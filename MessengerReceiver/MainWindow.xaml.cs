using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace MessengerReceiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queue<string> receivedTextStrings = new Queue<string>();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            Thread receivingThread = new Thread(StartReceiveThread);
            receivingThread.Start();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            while (receivedTextStrings.Any())
            {
                lock (receivedTextStrings)
                {
                    while (receivedTextStrings.Any())
                    {
                        tbxMessages.AppendText(receivedTextStrings.Dequeue() + "\n");
                    }
                }
            }
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
            //Laptop
            tbxMessages.AppendText(text + "\n");
            tbxInput.Text = "";

            TcpClient socket = new TcpClient("localhost", 85);

            //Laptop
            //TcpClient socket = new TcpClient("192.168.43.100", 85);

            byte[] data = Encoding.UTF8.GetBytes(text);
            NetworkStream stream = socket.GetStream();
            stream.Write(data, 0, data.Length);
        }

        private void StartReceiveThread()
        {
            //Laptop
            //IPAddress iPAddress = IPAddress.Parse("192.168.43.127");

            IPAddress iPAddress = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener tcpListener = new TcpListener(iPAddress, 86);
            tcpListener.Start();

            TcpClient tcpClient = tcpListener.AcceptTcpClient();

            byte[] buffer = new byte[1024];
            NetworkStream stream = tcpClient.GetStream();

            int byteCount;

            while (true)
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    while (stream.DataAvailable && (byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string receivedText = Encoding.UTF8.GetString(buffer, 0, byteCount);

                        lock (receivedTextStrings)
                        {
                            receivedTextStrings.Enqueue(receivedText);
                            tcpClient = null;
                        }
                    }
                }
                else
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    stream = tcpClient.GetStream();
                }
            }
        }
    }
}
