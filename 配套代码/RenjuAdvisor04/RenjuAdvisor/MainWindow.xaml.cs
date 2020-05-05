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
        private IKeyboardMouseEvents m_GlobalHook;
        List<Process> processWithWindow = new List<Process>();

        public MainWindow()
        {
            InitializeComponent();
            Subscribe();
            GetSysProcessWithWindow();
        }
       
        public void Subscribe()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.MouseMoveExt += M_GlobalHook_MouseMoveExt;
        }
        #region Win32 API
        [DllImport("user32.dll",EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);
        #endregion

        private void M_GlobalHook_MouseMoveExt(object sender, MouseEventExtArgs e)
        {
            //计算鼠标在哪个窗口上
            IntPtr handle =WindowFromPoint(e.X, e.Y);
            foreach (var p in processWithWindow)
            {
                if (p.MainWindowHandle == handle)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"名称：{p.MainModule.FileVersionInfo.ProductName}{Environment.NewLine}");
                    stringBuilder.Append($"版本：{p.MainModule.FileVersionInfo.FileVersion}{Environment.NewLine}");
                    stringBuilder.Append($"描述：{p.MainModule.FileVersionInfo.FileDescription}{Environment.NewLine}");
                    this.info.Text = stringBuilder.ToString();
                    break;
                }
            }
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IntPtr handle = WindowFromPoint(e.X, e.Y);
                foreach (var p in processWithWindow)
                {
                    if (p.MainWindowHandle == handle)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append($"名称：{p.MainModule.FileVersionInfo.ProductName}{Environment.NewLine}");
                        stringBuilder.Append($"版本：{p.MainModule.FileVersionInfo.FileVersion}{Environment.NewLine}");
                        stringBuilder.Append($"描述：{p.MainModule.FileVersionInfo.FileDescription}{Environment.NewLine}");
                        this.info.Text = stringBuilder.ToString();
                        break;
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
                m_GlobalHook.MouseDownExt -= M_GlobalHook_MouseMoveExt;
                m_GlobalHook.Dispose();
            }
        }

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
