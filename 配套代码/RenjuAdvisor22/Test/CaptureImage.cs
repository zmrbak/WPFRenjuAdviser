using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class CaptureImage
    {
        public static Bitmap Captuer(Process wzqProcess)
        {
            if (wzqProcess == null) return null;

            // 1)获取设备上下文
            IntPtr windowDCHandle = GetWindowDC(IntPtr.Zero);
            if (windowDCHandle == IntPtr.Zero)
            {
                ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            //   2）获取指定窗口边界和尺寸：GetWindowRect，
            Rectangle rectangle = new Rectangle();
            if (GetWindowRect(wzqProcess.MainWindowHandle, ref rectangle) == 0)
            {
                ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            };

            //3）计算窗口大小
            //注意C#中的Rectangle与C++中RECT区别
            int width = rectangle.Width - rectangle.X;
            int height = rectangle.Height - rectangle.Y;

            // 4）创建一个设备上下文相关的位图：CreateCompatibleBitmap->DeleteObject
            IntPtr compatibleBitmapHandle = CreateCompatibleBitmap(windowDCHandle, width, height);
            if (compatibleBitmapHandle == IntPtr.Zero)
            {
                DeleteObject(compatibleBitmapHandle);
                ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            //  5）创建一个内存上下文兼容的句柄：CreateCompatibleDC->DeleteDC
            IntPtr compatibleDCHandle = CreateCompatibleDC(windowDCHandle);
            if (compatibleDCHandle == IntPtr.Zero)
            {
                DeleteObject(compatibleBitmapHandle);
                DeleteDC(compatibleDCHandle);
                ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            // 6）选择一个设备上下文对象：SelectObject
            if (SelectObject(compatibleDCHandle, compatibleBitmapHandle) == IntPtr.Zero)
            {
                DeleteObject(compatibleBitmapHandle);
                DeleteDC(compatibleDCHandle);
                ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            // 7）拷贝窗口到设备上下文：PrintWindow
            if (PrintWindow(wzqProcess.MainWindowHandle, compatibleDCHandle, 0) == 0)
            {
                DeleteObject(compatibleBitmapHandle);
                DeleteDC(compatibleDCHandle);
                ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            Bitmap wzqImage = Image.FromHbitmap(compatibleBitmapHandle);

            //  8）清理垃圾
            DeleteObject(compatibleBitmapHandle);
            DeleteDC(compatibleDCHandle);
            ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);

            return wzqImage;
        }

        #region Win32 API
        [DllImport("Gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);

        [DllImport("Gdi32.dll")]
        public static extern int DeleteObject(IntPtr ho);

        [DllImport("User32.dll")]
        public static extern int PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);

        [DllImport("Gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        [DllImport("User32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        #endregion
    }
}
