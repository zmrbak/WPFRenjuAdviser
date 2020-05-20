using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RenjuAdvisor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

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
        private void OpenChooseGameWindow()
        {
            oldProcess = WzqGameProcess;

            WindowProcess windowProcess = new WindowProcess();
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
            if (process == null)
            {
                WzqGameProcess = oldProcess;
                return;
            }
        }

        private void WindowProcess_MouseMoveWinProcess(Process process)
        {
            WzqGameProcess = process;
        }
    }
}