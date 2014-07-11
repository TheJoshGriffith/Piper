using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Pipee : Form
    {
        NamedPipeClientStream npcs;
        Thread listenerThread;

        public Pipee()
        {
            InitializeComponent();

            listenerThread = new Thread(new ThreadStart(addToList));
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
                listBox1.Invoke((MethodInvoker)delegate {
                    listBox1.Items.Add(Encoding.Default.GetString(buff).Split('\0')[0]);
                });
            }
        }
    }
}
