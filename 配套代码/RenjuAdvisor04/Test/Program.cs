using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    class Program
    {

        static void Main1(string[] args)
        {
            //获得系统当前所有进程
            Process[] processes = Process.GetProcesses();
            List<Process> processWithWindow = new List<Process>();
            foreach (var item in processes)
            {
                if (item.MainWindowHandle == IntPtr.Zero) continue;
                if (item.MainWindowTitle.Length==0) continue;

                processWithWindow.Add(item);


                //if (item.MainWindowTitle == "五子棋")
                //{
                //    Console.WriteLine(item.ProcessName);
                //    Console.WriteLine(item.Id);
                //    //窗口名
                //    Console.WriteLine(item.MainWindowTitle);
                //    Console.WriteLine(item.MainModule.FileName);
                //    Console.WriteLine(item.MainModule.FileVersionInfo.FileVersion);
                //    Console.WriteLine(item.MainModule.FileVersionInfo.FileDescription);
                //    Console.WriteLine(item.MainModule.FileVersionInfo.Comments);
                //    Console.WriteLine(item.MainModule.FileVersionInfo.CompanyName);
                //    Console.WriteLine(item.MainModule.FileVersionInfo.FileName);
                //    //产品名
                //    Console.WriteLine(item.MainModule.FileVersionInfo.ProductName);
                //    Console.WriteLine(item.MainModule.FileVersionInfo.ProductVersion);
                //    Console.WriteLine(item.StartTime);
                //    Console.WriteLine(item.MainWindowHandle);
                //    break;
                //}
            }

            foreach (var item in processWithWindow)
            {
                Console.WriteLine(item.MainWindowTitle);
            }

            Console.ReadLine();
        }

        private IKeyboardMouseEvents m_GlobalHook;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Subscribe();
            Console.ReadLine();
            program.Unsubscribe();
        }
        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            //m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("KeyPress: \t{0}", e.KeyChar);
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1} Position:\t{2},{3}", e.Button, e.Timestamp,e.X,e.Y);

            // uncommenting the following line will suppress the middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        public void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            //m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }
    }
}
