using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //获得系统当前所有进程
            Process[] processes = Process.GetProcesses();
            foreach (var item in processes)
            {
                if (item.MainWindowTitle == "五子棋")
                {
                    Console.WriteLine(item.ProcessName);
                    Console.WriteLine(item.Id);
                    //窗口名
                    Console.WriteLine(item.MainWindowTitle);
                    Console.WriteLine(item.MainModule.FileName);
                    Console.WriteLine(item.MainModule.FileVersionInfo.FileVersion);
                    Console.WriteLine(item.MainModule.FileVersionInfo.FileDescription);
                    Console.WriteLine(item.MainModule.FileVersionInfo.Comments);
                    Console.WriteLine(item.MainModule.FileVersionInfo.CompanyName);
                    Console.WriteLine(item.MainModule.FileVersionInfo.FileName);
                    //产品名
                    Console.WriteLine(item.MainModule.FileVersionInfo.ProductName);
                    Console.WriteLine(item.MainModule.FileVersionInfo.ProductVersion);
                    Console.WriteLine(item.StartTime);
                    Console.WriteLine(item.MainWindowHandle);
                    break;
                }
            }
            Console.ReadLine();
        }
    }
}
