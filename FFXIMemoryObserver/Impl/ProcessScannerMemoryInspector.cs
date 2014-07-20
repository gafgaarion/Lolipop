using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CommunicationHandler;
using FFXICommands.Commands;
using System.Threading;
using FFACETools;
using System.Windows.Forms;
using System.IO;
using Commons;

namespace FFXIMemoryObserver.Impl
{
    public class ProcessScannerMemoryInspector : IMemoryInspector
    {
        private string leaderId = String.Empty;
        private IBus bus;
        private Process[] ffxiProcess;
        private Thread memoryThread;
        private readonly ManualResetEvent _stop, _ready;
        private bool stop = false;
        List<FFACE> fface_instance;
        DateTime loopStartTime;
        double longestTime = 0;

        public ProcessScannerMemoryInspector(IBus bus)
        {
            this.bus = bus;
            _ready = new ManualResetEvent(false);
        }

        public void setLeaderId(string id)
        {
            this.leaderId = id;
        }

        public string getLeaderId()
        {
            return this.leaderId;
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        public static byte[] ReadMemory(Process process, int address, int numOfBytes, out int bytesRead)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, process.Id);

            byte[] buffer = new byte[numOfBytes];

            ReadProcessMemory(hProc, new IntPtr(address), buffer, numOfBytes, out bytesRead);
            return buffer;
        }


        public void Start()
        {
            fface_instance = new List<FFACE>();
            this.ffxiProcess = Process.GetProcessesByName("pol");

            if (ffxiProcess == null)
                throw new Exceptions.GameNotFoundException();

            try
            {
                    foreach (Process process in this.ffxiProcess)
                    {
                        
                        fface_instance.Add(new FFACE((int)process.Id));

                        string name = fface_instance[fface_instance.Count - 1].Player.Name;
                        int id = fface_instance[fface_instance.Count - 1].Player.ID;

                        this.bus.Send(new InitializeCharacterCommand { instance = fface_instance[fface_instance.Count - 1], characterName = name });

                    }

                    this.memoryThread = new Thread(ScanMemoryWorker);
                    this.memoryThread.Start();
                    

                    _ready.Set();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public void ScanMemoryWorker()
        {

              WaitHandle[] wait = new[] { _ready };

              FFACE leaderInstance = null;
              List<FFACE.ChatTools.ChatLine> leaderLines = new List<FFACE.ChatTools.ChatLine>();
              List<FFACE.ChatTools.ChatLine> lines = new List<FFACE.ChatTools.ChatLine>();
              leaderLines = new List<FFACE.ChatTools.ChatLine>();
              lines = new List<FFACE.ChatTools.ChatLine>();

              while (0 == WaitHandle.WaitAny(wait) && !stop)
              {
                 this.loopStartTime = DateTime.Now;

                 foreach(FFACE curInst in fface_instance)
                 {

                     while (curInst.Chat.IsNewLine)
                     {

                         FFACE.ChatTools.ChatLine line = curInst.Chat.GetNextLine();
                         curInst.Chat.Clear();


                         if (line != null)
                         {
                             //using (StreamWriter w = File.AppendText("logMemoryInspec.txt"))
                             //{
                             //    w.WriteLine(line.Now + "  /  " + DateTime.Now.ToString("yyyyMMddHHmmss") + "    " + line.Text);
                             //}

                            lines.Add(line);
                         }
                     }

                     bus.Send(new UpdateCharacterStatusCommand
                     {
                         characterName = curInst.Player.Name,
                         player = curInst.Player,
                         party = curInst.Party,
                         target = curInst.Target,
                         chat = new List<FFACE.ChatTools.ChatLine>(lines)
                     });



                      if (curInst.Player.Name == this.leaderId)
                      {
                          leaderLines = lines;
                          leaderInstance = curInst;
                      }

                }


                 if (leaderId != String.Empty)
                 {
                     bus.Send(new UpdateGameObjectsCommand
                     {
                         instance = leaderInstance
                     });
                 
                
                    bus.Send(new UpdateChatCommand
                    {
                        characterName = String.Empty, // Destined to objects
                        lines = new List<FFACE.ChatTools.ChatLine>(leaderLines)
                    });
                }

                leaderLines.Clear();
                lines.Clear();

                if ((DateTime.Now - this.loopStartTime).TotalSeconds > longestTime)
                    longestTime = (DateTime.Now - this.loopStartTime).TotalSeconds;

                Thread.Sleep(200);
             }
           
        }

        public void Stop()
        {
            stop = true;
            memoryThread.Join();
        }

        public void Terminate()
        {
           // MessageBox.Show("Longest update time: " + this.longestTime.ToString());
            Stop();
            bus.Terminate();
        }
    }
}
