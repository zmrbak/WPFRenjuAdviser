using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class RenjuAiHelper
    {
        //        INFO max_memory 83886080
        //INFO timeout_match 180000
        //INFO timeout_turn 300000
        //INFO game_type 1
        //INFO rule 4
        //INFO time_left 178163
        //INFO folder C:\ProgramData
        //BOARD
        private int size = 15;
        private int max_memory = 0;
        private int timeout_match = 0;
        private int timeout_turn = 0;
        private int game_type = 0;
        //0无禁手，4有禁售
        private int rule = 0;
        private int time_left = 0;
        private string folder = "";

        private string aiFileName;
        private Process aiProcess;

        public delegate void AiResponseHandler(object sender, DataReceivedEventArgs e);
        public event AiResponseHandler AiResponse;

        public string AiFileName { get => aiFileName; set => aiFileName = value; }
        public int Timeout_match { get => timeout_match; set => timeout_match = value; }
        public int Timeout_turn { get => timeout_turn; set => timeout_turn = value; }
        public int Rule { get => rule; set => rule = value; }
        public int Time_left { get => time_left; set => time_left = value; }
        public int Size { get => size; set => size = value; }
        public string Folder { get => folder; set => folder = value; }

        //启动 AI
        public void StartAi()
        {
            aiProcess = new Process();
            //AI 程序
            aiProcess.StartInfo.FileName = aiFileName;
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
        }

        public void WriteInfo(string info)
        {
            aiProcess.StandardInput.WriteLine(info);
        }
        

        public void SetBoard(List<Tuple<int, int, int>> chessPointList)
        {
            //            START 15
            aiProcess.StandardInput.WriteLine("START " + size);
            //INFO max_memory 83886080
            aiProcess.StandardInput.WriteLine("INFO max_memory  " + max_memory);
            //INFO timeout_match 180000
            aiProcess.StandardInput.WriteLine("INFO timeout_match  " + timeout_match);
            //INFO timeout_turn 300000
            aiProcess.StandardInput.WriteLine("INFO timeout_turn  " + timeout_turn);
            //INFO game_type 1
            aiProcess.StandardInput.WriteLine("INFO game_type  " + game_type);
            //INFO rule 4
            aiProcess.StandardInput.WriteLine("INFO rule  " + rule);
            //INFO time_left 178163
            aiProcess.StandardInput.WriteLine("INFO time_left  " + time_left);
            //INFO folder C:\ProgramData
            aiProcess.StandardInput.WriteLine("INFO folder  " + folder);
            //BOARD
            aiProcess.StandardInput.WriteLine("BOARD");
            //7,6,2
            foreach (var item in chessPointList)
            {
                aiProcess.StandardInput.WriteLine($"{item.Item1},{item.Item2},{item.Item3}");
            }
            //DONE";
            aiProcess.StandardInput.WriteLine("DONE");
        }

        public void About()
        {
            aiProcess.StandardInput.WriteLine("about");
        }

        public void End()
        {
            aiProcess.StandardInput.WriteLine("End");
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
