using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenjuAdvisor.Utility
{
    public class RenjuAiHelper
    {
        public delegate void AiResponseHandler(object sender, DataReceivedEventArgs e);
        public event AiResponseHandler AiResponse;

        public delegate void AiCpuUtilizationRateHander(Process process, double utilizationRate);
        public event AiCpuUtilizationRateHander AiCpuUtilizationRate;

        private int turnTimeout;
        private string folder = "";
        private string aiFileName;
        private Boolean isGameRule4;
        private Process aiProcess;
        private int checkCpuInterval = 1000;

        public string AiFileName
        {
            get => aiFileName; 
            set
            {
                aiFileName = value;
                if (string.IsNullOrEmpty(aiFileName)) return;
                folder = Path.Combine(AppContext.BaseDirectory,"temp",Path.GetFileNameWithoutExtension(aiFileName));
                if (Directory.Exists(folder) == false)
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }
        public Process AiProcess { get => aiProcess; }
        public int CheckCpuInterval
        {
            get => checkCpuInterval;
            set
            {
                checkCpuInterval = value < 100 ? 100 : (value > 1000 ? 1000 : value);
            }
        }
        public int TurnTimeout { get => turnTimeout; set => turnTimeout = value; }
        public bool IsGameRule4 { get => isGameRule4; set => isGameRule4 = value; }

        //启动 AI
        public void StartAi()
        {
            if (String.IsNullOrEmpty(aiFileName)) return;
            string aiFileFullName = Path.Combine(AppContext.BaseDirectory,"ai", aiFileName);
            if (File.Exists(aiFileFullName) == false) return;

            aiProcess = new Process();
            //AI 程序
            aiProcess.StartInfo.FileName = aiFileFullName;
            //是否使用操作系统shell启动
            aiProcess.StartInfo.UseShellExecute = false;
            //接受来自调用程序的输入信息
            aiProcess.StartInfo.RedirectStandardInput = true;
            //由调用程序获取输出信息
            aiProcess.StartInfo.RedirectStandardOutput = true;
            //重定向标准错误输出
            aiProcess.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            aiProcess.StartInfo.CreateNoWindow = true;
            aiProcess.OutputDataReceived += AiProcess_OutputDataReceived;
            aiProcess.ErrorDataReceived += AiProcess_ErrorDataReceived;
            //启动程序
            aiProcess.Start();
            aiProcess.BeginOutputReadLine();
            aiProcess.BeginErrorReadLine();

            new Thread(()=> {
                try
                {
                    var lastCpuTimeSpan = TimeSpan.Zero;
                    while (!aiProcess.HasExited)
                    {
                        lastCpuTimeSpan = aiProcess.TotalProcessorTime;
                        Thread.Sleep(checkCpuInterval);
                        var value = 100f * (aiProcess.TotalProcessorTime - lastCpuTimeSpan).TotalMilliseconds / checkCpuInterval;
                        AiCpuUtilizationRate?.Invoke(aiProcess, value);
                    }
                }
                catch 
                {
                    AiCpuUtilizationRate?.Invoke(aiProcess, 0);
                }               
            }).Start();

            //输出软件信息
            aiProcess.StandardInput.WriteLine("about");
        }

        public void SetBoard(List<Tuple<int, int, int>> chessPointList)
        {
            if (aiProcess == null) return;

            //START 15
            aiProcess.StandardInput.WriteLine("START 15");
            //INFO max_memory 83886080
            //1G内存
            aiProcess.StandardInput.WriteLine("INFO max_memory " + 1 * 1024 * 1024 * 1024);
            //INFO timeout_match 180000
            aiProcess.StandardInput.WriteLine("INFO timeout_match " + turnTimeout * 3);
            //INFO timeout_turn 300000
            aiProcess.StandardInput.WriteLine("INFO timeout_turn " + turnTimeout);
            //INFO game_type 1
            aiProcess.StandardInput.WriteLine("INFO game_type 0");
            //INFO rule 4
            if (isGameRule4 == true)
            {
                aiProcess.StandardInput.WriteLine("INFO rule 4");
            }
            else
            {
                aiProcess.StandardInput.WriteLine("INFO rule 0");
            }
            //INFO time_left 178163
            aiProcess.StandardInput.WriteLine("INFO time_left " + turnTimeout * 2);
            //INFO folder C:\ProgramData
            aiProcess.StandardInput.WriteLine("INFO folder " + folder);
            //BOARD
            aiProcess.StandardInput.WriteLine("BOARD");
            //7,6,2
            foreach (var item in chessPointList)
            {
                aiProcess.StandardInput.WriteLine($"{item.Item1 - 1},{item.Item2 - 1},{item.Item3}");
            }
            //DONE";
            aiProcess.StandardInput.WriteLine("DONE");
        }

        public void End()
        {
            if (aiProcess == null) return;

            aiProcess.OutputDataReceived -= AiProcess_OutputDataReceived;
            aiProcess.ErrorDataReceived -= AiProcess_ErrorDataReceived;
            aiProcess.StandardInput.WriteLine("End");

            Thread.Sleep(200);
            if(aiProcess.HasExited ==false)
            {
                aiProcess.Kill();
            }
        }

        private void AiProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            AiResponse?.Invoke(sender, e);
        }

        private void AiProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AiResponse?.Invoke(sender, e);
        }
    }
}
