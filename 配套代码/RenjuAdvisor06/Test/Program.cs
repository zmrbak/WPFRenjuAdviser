using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcesses();
            Process wzqProcess = null;
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
                    wzqProcess = item;
                    break;
                }
            }
            Bitmap bitmap = CaptureImage.Captuer(wzqProcess);
            if (bitmap == null) return;

            bitmap.Save("a.bmp");
            Process.Start("mspaint", "a.bmp");

            //截图
        }
    }
}
