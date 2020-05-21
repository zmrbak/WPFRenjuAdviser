using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Utility
{
    public class UtilityTool
    {
        /// <summary>
        /// 获取系统中带有窗口的进程
        /// </summary>
        public static List<Process> GetSysProcessWithWindow()
        {
            List<Process> processWithWindow = new List<Process>();
            Process[] processes = Process.GetProcesses();
            foreach (var item in processes)
            {
                if (item.MainWindowHandle == IntPtr.Zero) continue;
                if (item.MainWindowTitle.Length == 0) continue;
                processWithWindow.Add(item);
            }
            return processWithWindow;
        }

        /// <summary>
        /// 获得窗口标题为“五子棋”的进程
        /// </summary>
        public static Process GetWzqGameProcess()
        {
            Process[] processes = Process.GetProcesses();
            foreach (var item in processes)
            {
                if (item.MainWindowTitle == "五子棋")
                {
                    return item;
                }
            }
            return null;
        }
    }
}
