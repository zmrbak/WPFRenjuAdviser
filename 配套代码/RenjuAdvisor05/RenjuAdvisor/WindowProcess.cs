using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenjuAdvisor
{
    public class WindowProcess
    {
        //定义代理
        public delegate void WinProcessHandler(Process process);
        //定义鼠标移动时，获取的窗口所在的进程
        public event WinProcessHandler MouseMoveWinProcess;
        //定义鼠标左击时，获取的窗口所在的进程
        public event WinProcessHandler MouseDownWinProcess;

        private IKeyboardMouseEvents m_GlobalHook;
        List<Process> processWithWindow = new List<Process>();

        //从外部提供带有窗口的进程列表
        public List<Process> ProcessWithWindow { get => processWithWindow; set => processWithWindow = value; }

        //开始HOOK
        public void StartGetWinProcess()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.MouseMoveExt += M_GlobalHook_MouseMoveExt;
        }

        //鼠标移动，发送事件
        private void M_GlobalHook_MouseMoveExt(object sender, MouseEventExtArgs e)
        {
            //计算鼠标在哪个窗口上
            IntPtr handle = WindowFromPoint(e.X, e.Y);
            if (handle == IntPtr.Zero) return;

            foreach (var p in processWithWindow)
            {
                if (p.MainWindowHandle == handle)
                {
                    MouseMoveWinProcess?.Invoke(p);
                    return;
                }
            }
        }

        //鼠标按下，发送事件
        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IntPtr handle = WindowFromPoint(e.X, e.Y);
                if (handle == IntPtr.Zero) return;

                foreach (var p in processWithWindow)
                {
                    if (p.MainWindowHandle == handle)
                    {
                        m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
                        m_GlobalHook.MouseDownExt -= M_GlobalHook_MouseMoveExt;
                        m_GlobalHook.Dispose();
                        MouseDownWinProcess?.Invoke(p);
                        return;
                    }
                }
            }
        }

        #region Win32 API
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);
        #endregion
    }
}
