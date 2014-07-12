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
using System.Diagnostics;

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

            int pid = Process.GetProcessesByName("Tibia")[0].Id;
            
            bool res = InjectDLL(pid);

            if (!res)
            {
                MessageBox.Show("Failed to inject!");
            }

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
            startupThread.Abort();
        }

        private bool InjectDLL(int processId)
        {
            string DLL_PATH = "C:\\Users\\Joshua\\Documents\\visual studio 2013\\Projects\\Piper\\PipeeWPF\\bin\\Debug\\Piper.dll";
            uint DLL_PATH_LENGTH = (uint)DLL_PATH.Length;
            IntPtr kernel = WinAPI.GetModuleHandle("Kernel32");
            IntPtr loadLibrary = WinAPI.GetProcAddress(kernel, "LoadLibraryA");

            if (loadLibrary == IntPtr.Zero)
                return false;

            IntPtr process = WinAPI.OpenProcess(WinAPI.PROCESS_ALL_ACCESS, false, processId);

            if (process == IntPtr.Zero)
                return false;

            IntPtr remoteMemory = WinAPI.VirtualAllocEx(process, IntPtr.Zero, DLL_PATH_LENGTH + 1, WinAPI.AllocationType.Commit, WinAPI.MemoryProtection.ReadWrite);

            if (remoteMemory == IntPtr.Zero)
            {
                WinAPI.CloseHandle(process);
                return false;
            }

            //byte[] bytes = Encoding.ASCII.GetBytes(DLL_PATH);
            UIntPtr lpNumberOfBytesWritten;
            WinAPI.WriteProcessMemory(process, remoteMemory, Encoding.ASCII.GetBytes(DLL_PATH), DLL_PATH_LENGTH, out lpNumberOfBytesWritten);
            IntPtr remoteThread = WinAPI.CreateRemoteThread(process, IntPtr.Zero, 0, loadLibrary, remoteMemory, 0, IntPtr.Zero);

            WinAPI.VirtualFreeEx(process, remoteMemory, DLL_PATH_LENGTH, WinAPI.AllocationType.Release);
            WinAPI.CloseHandle(process);

            return true;
        }
    }
}
