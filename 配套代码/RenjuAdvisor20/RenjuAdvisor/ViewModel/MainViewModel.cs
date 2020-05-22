using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RenjuAdvisor.Event;
using RenjuAdvisor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace RenjuAdvisor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            ///
            WzqGameProcess = UtilityTool.GetWzqGameProcess();
            ChooseWzqGameRelayCommnad = new RelayCommand(OpenChooseGameWindow);
        }

        Process oldProcess = null;
        WindowProcess windowProcess = null;
        private void OpenChooseGameWindow()
        {
            //设置鼠标光标
            string curFile = Path.Combine(AppContext.BaseDirectory, "Cursor\\aero_cross_xl.cur");
            IntPtr colorCursorHandle = Win32Helper.LoadCursorFromFile(curFile);
            Win32Helper.SetSystemCursor(colorCursorHandle, Win32Helper.OCR_NORMAL);

            oldProcess = WzqGameProcess;

            windowProcess = new WindowProcess();
            windowProcess.ProcessWithWindow = UtilityTool.GetSysProcessWithWindow(); ;
            windowProcess.MouseMoveWinProcess += WindowProcess_MouseMoveWinProcess;
            windowProcess.MouseDownWinProcess += WindowProcess_MouseDownWinProcess;
            windowProcess.StartGetWinProcess();
        }

        //属性
        private Process wzqGameProcess;
        public Process WzqGameProcess
        {
            get { return wzqGameProcess; }
            set
            {
                Set(ref wzqGameProcess, value);
                //wzqGameProcess = value;
                //RaisePropertyChanged();
                //RaisePropertyChanged("WzqGameProcess");
                //RaisePropertyChanged(() => WzqGameProcess);
            }
        }

        //命令
        private RelayCommand chooseWzqGameRelayCommnad;
        public RelayCommand ChooseWzqGameRelayCommnad { get => chooseWzqGameRelayCommnad; set => chooseWzqGameRelayCommnad = value; }


        //时间回调函数
        private void WindowProcess_MouseDownWinProcess(Process process)
        {
            //恢复系统默认光标
            Win32Helper.SystemParametersInfo(Win32Helper.SPI_SETCURSORS, 0,0, Win32Helper.SPIF_SENDWININICHANGE);

            windowProcess.MouseMoveWinProcess -= WindowProcess_MouseMoveWinProcess;
            windowProcess.MouseDownWinProcess -= WindowProcess_MouseDownWinProcess;

            if (process == null)
            {
                WzqGameProcess = oldProcess;
                return;
            }

            //发送消息，打开裁剪棋盘的窗口
            Messenger.Default.Send(new CutBoardArgs { WzqGameProcess = process });
        }

        private void WindowProcess_MouseMoveWinProcess(Process process)
        {
            WzqGameProcess = process;
        }


    }
}