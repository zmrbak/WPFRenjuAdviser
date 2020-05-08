using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RenjuAdvisor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
      
        List<Process> processWithWindow = new List<Process>();
        WindowProcess windowProcess;

        public MainWindow()
        {
            InitializeComponent();           
            GetSysProcessWithWindow();
            windowProcess = new WindowProcess();
            windowProcess.ProcessWithWindow = processWithWindow;
            windowProcess.MouseMoveWinProcess += WindowProcess_MouseMoveWinProcess;
            windowProcess.MouseDownWinProcess += WindowProcess_MouseDownWinProcess;
            windowProcess.StartGetWinProcess();
        }

        private void WindowProcess_MouseDownWinProcess(Process process)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"名称：{process.MainModule.FileVersionInfo.ProductName}{Environment.NewLine}");
            stringBuilder.Append($"版本：{process.MainModule.FileVersionInfo.FileVersion}{Environment.NewLine}");
            stringBuilder.Append($"描述：{process.MainModule.FileVersionInfo.FileDescription}{Environment.NewLine}");
            this.info.Text = stringBuilder.ToString();
        }

        private void WindowProcess_MouseMoveWinProcess(Process process)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"名称：{process.MainModule.FileVersionInfo.ProductName}{Environment.NewLine}");
            stringBuilder.Append($"版本：{process.MainModule.FileVersionInfo.FileVersion}{Environment.NewLine}");
            stringBuilder.Append($"描述：{process.MainModule.FileVersionInfo.FileDescription}{Environment.NewLine}");
            this.info.Text = stringBuilder.ToString();
        }

        /// <summary>
        /// 获取系统所有进程
        /// </summary>
        private void GetSysProcessWithWindow()
        {
            Process[] processes = Process.GetProcesses();
            processWithWindow.Clear();
            foreach (var item in processes)
            {
                if (item.MainWindowHandle == IntPtr.Zero) continue;
                if (item.MainWindowTitle.Length == 0) continue;
                processWithWindow.Add(item);
            }
        }
    }
}
