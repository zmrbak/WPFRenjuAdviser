using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Utility
{
    public class Win32Helper
    {
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);
        public const uint OCR_NORMAL = 32512;

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
        public const uint SPI_SETCURSORS = 0x0057;
        public const uint SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);
    }
}
