using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RenjuAdvisor.Event;
using RenjuAdvisor.Interface;
using RenjuAdvisor.Service;
using RenjuAdvisor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Linq;

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
    public class MainViewModel : ViewModelBase, IReloadData
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IConfiguration configuration)
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
            ChooseWzqGameRelayCommnad = new RelayCommand(OpenChooseGameWindow);
            SaveCaptureIntevalCommand = new RelayCommand<string>(SaveCaptureInteval);
            ViewLoadedCommnad = new RelayCommand<RoutedEventArgs>(ViewLoaded);

            baseConfiguration = configuration as BaseConfiguration;
            baseConfiguration.Load();
            WzqGameProcess = UtilityTool.GetWzqGameProcess(baseConfiguration.GameUiTitle);

            chessRecognize = new ChessRecognize(configuration);
            chessRecognize.ChessRecognized += ChessRecognize_ChessRecognized;

            //���WzqGameProcess
            new Thread(() =>
            {
                Process lastProcess = null;
                while (true)
                {
                    if(isClosing==true)
                    {
                        return;
                    }

                    if (WzqGameProcess == null || WzqGameProcess.HasExited)
                    {
                        WzqGameProcess = UtilityTool.GetWzqGameProcess(baseConfiguration.GameUiTitle);

                        if (WzqGameProcess!=null && lastProcess != WzqGameProcess)
                        {
                            lastProcess = WzqGameProcess;
                            baseConfiguration.GameProcessId = WzqGameProcess.Id;
                            chessRecognize.UpdateSetting();
                        }
                    }
                    else
                    {
                        if(WzqGameProcess.Id != baseConfiguration.GameProcessId)
                        {
                            baseConfiguration.GameProcessId = WzqGameProcess.Id;
                            chessRecognize.UpdateSetting();
                        }
                    }
                    Thread.Sleep(200);
                }
            }).Start();
        }

        public void ClosingCommand()
        {
            isClosing = true;
            chessRecognize.Stop();
        }

        private void ViewLoaded(RoutedEventArgs e)
        {
            MainWindow mainWindow= e.Source as MainWindow;
            TextBox textBox= mainWindow.FindName("textBox") as TextBox;
            textBox.Text = baseConfiguration.CaptureInterval.ToString();
        }

        private void SaveCaptureInteval(string obj)
        {
            baseConfiguration.CaptureInterval = int.Parse(obj);
            baseConfiguration.Save();
            chessRecognize.UpdateSetting();
        }

        private void ChessRecognize_ChessRecognized(object sender, ChessRecongizedEventArgs e)
        {
            Console.WriteLine("-------------------" + DateTime.Now.ToString() + "-------------------");
            //�ж������Ƿ�����
            //��Ч����������=��������
            //��Ч����������=��������+1
            //ʹ��LINQ���в���

            var blackCount = (from r in e.ChessPointList
                              where r.Item3 == 1
                              select r).Count();
            var whiteCount= (from r in e.ChessPointList
                             where r.Item3 == 2
                             select r).Count();

            if ((blackCount == whiteCount || blackCount == whiteCount + 1) == false)
            {
                Console.WriteLine($"ʶͼ����blackCount={blackCount},whiteCount={whiteCount}");
                //���ִ�е��������Ҫ�˹�����
            }
            else
            {
                //��������
                foreach (var item in e.ChessPointList)
                {
                    Console.WriteLine($"{item.Item1},{item.Item2},{item.Item3}");
                }
            }
        }
        Boolean isClosing = false;
        Process oldProcess = null;
        WindowProcess windowProcess = null;
        ChessRecognize chessRecognize = null;
        private void OpenChooseGameWindow()
        {
            chessRecognize.Stop();
            //���������
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

        //����
        private Process wzqGameProcess;
        public Process WzqGameProcess
        {
            get { return wzqGameProcess; }
            set
            {
                Set(ref wzqGameProcess, value);
            }
        }

        //����
        private RelayCommand chooseWzqGameRelayCommnad;
        public RelayCommand ChooseWzqGameRelayCommnad { get => chooseWzqGameRelayCommnad; set => chooseWzqGameRelayCommnad = value; }
        public RelayCommand<string> SaveCaptureIntevalCommand { get => saveCaptureIntevalCommand; set => saveCaptureIntevalCommand = value; }
        public RelayCommand<RoutedEventArgs> ViewLoadedCommnad { get => viewLoadedCommnad; set => viewLoadedCommnad = value; }

        private BaseConfiguration baseConfiguration;
        private RelayCommand<string> saveCaptureIntevalCommand;
        private RelayCommand<RoutedEventArgs> viewLoadedCommnad;

        //�¼��ص�����
        private void WindowProcess_MouseDownWinProcess(Process process)
        {
            //�ָ�ϵͳĬ�Ϲ��
            Win32Helper.SystemParametersInfo(Win32Helper.SPI_SETCURSORS, 0, 0, Win32Helper.SPIF_SENDWININICHANGE);

            windowProcess.MouseMoveWinProcess -= WindowProcess_MouseMoveWinProcess;
            windowProcess.MouseDownWinProcess -= WindowProcess_MouseDownWinProcess;

            if (process == null)
            {
                WzqGameProcess = oldProcess;
                return;
            }

            //������Ϣ���򿪲ü����̵Ĵ���
            Messenger.Default.Send(new CutBoardArgs { WzqGameProcess = process });
        }

        private void WindowProcess_MouseMoveWinProcess(Process process)
        {
            WzqGameProcess = process;
        }

        public void ReloadData()
        {
            baseConfiguration.GameProcessId = WzqGameProcess.Id;
            chessRecognize.UpdateSetting();
        }

        public void RestoreData()
        {
            WzqGameProcess = oldProcess;
            chessRecognize.Start();
        }
    }
}