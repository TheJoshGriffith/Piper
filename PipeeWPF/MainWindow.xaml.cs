using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Threading;

namespace PipeeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NamedPipeClientStream npcs;
        Thread listenerThread;
        Thread startupThread;
        public MainWindow()
        {
            InitializeComponent();

            listenerThread = new Thread(new ThreadStart(addToList));
            startupThread = new Thread(new ThreadStart(initNpcs));
            startupThread.Start();
        }

        public void initNpcs()
        {
            npcs = new NamedPipeClientStream("piperpipe");
            npcs.Connect();
            npcs.ReadMode = PipeTransmissionMode.Message;
            listenerThread.Start();
        }

        public void addToList()
        {
            while (true)
            {
                byte[] buff = new byte[32];
                npcs.Read(buff, 0, 32);
                string text = Encoding.Default.GetString(buff).Split('\0')[0];
                if (text != "")
                {
                    lbMessages.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        lbMessages.Items.Add(Encoding.Default.GetString(buff).Split('\0')[0]);
                    }));
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listenerThread.Abort();
        }
    }
}
