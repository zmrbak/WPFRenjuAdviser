using RenjuAdvisor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SelectWindow
{
    class Program
    {
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }

        static void Main(string[] args)
        {
            List<WindowInfo> windowInfos = new List<WindowInfo>();
            EnumWindows((hWnd, lParam)=>{
                StringBuilder sb = new StringBuilder(256);

                WindowInfo windowInfo = new WindowInfo();
                windowInfo.hWnd = hWnd;

                GetWindowTextW(hWnd,sb,sb.Capacity);
                windowInfo.szWindowName = sb.ToString();

                GetClassNameW(hWnd, sb, sb.Capacity);
                windowInfo.szClassName = sb.ToString();

                windowInfos.Add(windowInfo);
                return true;
            },0);

            foreach (var item in windowInfos)
            {
                if (item.szWindowName == "Default IME") continue;
                if (item.szWindowName == "MSCTFIME UI") continue;
                if (item.szClassName == "PalmInputUIStatus") continue;
                if (item.szWindowName == "PalmInputUIStatus") continue;
                if (item.szClassName == "tooltips_class32") continue;
                if (item.szClassName == "WorkerW") continue;
                if (item.szWindowName == "Default IME") continue;


                Rectangle rectangle = new Rectangle();
                GetWindowRect(item.hWnd,ref rectangle);
                int width = rectangle.Width - rectangle.X;
                int height = rectangle.Height - rectangle.Y;
                if (width < 20) continue;
                if (height < 20) continue;


                Console.WriteLine($"{item.hWnd}");
                Console.WriteLine($"{item.szWindowName}");
                Console.WriteLine($"{item.szClassName}");
                Console.WriteLine(rectangle.X);
                Console.WriteLine(rectangle.Y);
                Console.WriteLine(width);
                Console.WriteLine(height);
                Console.WriteLine();

                if (item.szClassName == "BS2CHINAUI" && item.szWindowName == "BlueStacks App Player")
                {
                    IntPtr windowHandle = item.hWnd;
                    Bitmap bitmap = CaptureImage.Captuer(windowHandle);
                    bitmap.Save("a.bmp");
                    Process.Start("mspaint", "a.bmp");
                }
            }



        }
    }
}
