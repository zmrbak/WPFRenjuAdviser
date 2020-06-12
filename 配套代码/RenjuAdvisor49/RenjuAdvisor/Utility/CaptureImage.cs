using RenjuAdvisor.Service;
using RenjuAdvisor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Utility
{
    public class CaptureImage
    {
        public static Bitmap Captuer(Process wzqProcess,IConfiguration configuration=null)
        {
            if (wzqProcess == null) return null;

            // 1)获取设备上下文
            IntPtr windowDCHandle =Win32Helper.GetWindowDC(IntPtr.Zero);
            if (windowDCHandle == IntPtr.Zero)
            {
                Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            //   2）获取指定窗口边界和尺寸：GetWindowRect，
            Rectangle rectangle = new Rectangle();
            if (Win32Helper.GetWindowRect(wzqProcess.MainWindowHandle, ref rectangle) == 0)
            {
                Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            };

            //3）计算窗口大小
            //注意C#中的Rectangle与C++中RECT区别
            int width = rectangle.Width - rectangle.X;
            int height = rectangle.Height - rectangle.Y;
            if(configuration!=null)
            {
                BaseConfiguration baseConfiguration = configuration as BaseConfiguration;
                baseConfiguration.GameWindowPoint = new Point(rectangle.X, rectangle.Y);
            }

            // 4）创建一个设备上下文相关的位图：CreateCompatibleBitmap->DeleteObject
            IntPtr compatibleBitmapHandle = Win32Helper.CreateCompatibleBitmap(windowDCHandle, width, height);
            if (compatibleBitmapHandle == IntPtr.Zero)
            {
                Win32Helper.DeleteObject(compatibleBitmapHandle);
                Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            //  5）创建一个内存上下文兼容的句柄：CreateCompatibleDC->DeleteDC
            IntPtr compatibleDCHandle = Win32Helper.CreateCompatibleDC(windowDCHandle);
            if (compatibleDCHandle == IntPtr.Zero)
            {
                Win32Helper.DeleteObject(compatibleBitmapHandle);
                Win32Helper.DeleteDC(compatibleDCHandle);
                Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            // 6）选择一个设备上下文对象：SelectObject
            if (Win32Helper.SelectObject(compatibleDCHandle, compatibleBitmapHandle) == IntPtr.Zero)
            {
                Win32Helper.DeleteObject(compatibleBitmapHandle);
                Win32Helper.DeleteDC(compatibleDCHandle);
                Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            // 7）拷贝窗口到设备上下文：PrintWindow
            if (Win32Helper.PrintWindow(wzqProcess.MainWindowHandle, compatibleDCHandle, 0) == 0)
            {
                Win32Helper.DeleteObject(compatibleBitmapHandle);
                Win32Helper.DeleteDC(compatibleDCHandle);
                Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);
                return null;
            }

            Bitmap wzqImage = Image.FromHbitmap(compatibleBitmapHandle);

            //  8）清理垃圾
            Win32Helper.DeleteObject(compatibleBitmapHandle);
            Win32Helper.DeleteDC(compatibleDCHandle);
            Win32Helper.ReleaseDC(wzqProcess.MainWindowHandle, windowDCHandle);

            return wzqImage;
        }

    }
}
