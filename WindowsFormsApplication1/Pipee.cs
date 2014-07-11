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
            listenerThread.Start();
        }

        public void addToList()
        {
            byte[] buff = new byte[512];
            npcs.Read(buff, 0, 512);
            listBox1.Items.Add(Encoding.ASCII.GetString(buff));
        }
    }
}
