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
using GalaSoft.MvvmLight.Views;
using OpenCvSharp;

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
        public MainViewModel(IConfiguration configuration, IDialogService dialogService)
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
            SaveTurnTimeoutCommand = new RelayCommand<string>(SaveTurnTimeout);

            DialogService = dialogService;

            string aiFilePath = Path.Combine(AppContext.BaseDirectory,"ai");
            if(Directory.Exists(aiFilePath)==false)
            {
                Directory.CreateDirectory(aiFilePath);
            }
           string[] aiFiles= Directory.GetFiles(aiFilePath, "*.exe", SearchOption.TopDirectoryOnly);
            if(aiFiles.Count()==0)
            {
                DialogService.ShowError("ȱ��AI���������ذ�װ","����","",()=> {
                    Process.Start("https://github.com/Gomocup/GomocupDownload");
                });
            }

            AiList = (from r in aiFiles
                      select new FileInfo(r).Name).ToList();

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

                    try
                    {
                        if (WzqGameProcess == null || WzqGameProcess.HasExited)
                        {
                            WzqGameProcess = UtilityTool.GetWzqGameProcess(baseConfiguration.GameUiTitle);

                            if (WzqGameProcess != null && lastProcess != WzqGameProcess)
                            {
                                lastProcess = WzqGameProcess;
                                baseConfiguration.GameProcessId = WzqGameProcess.Id;
                                chessRecognize.UpdateSetting();
                            }
                        }
                        else
                        {
                            if (WzqGameProcess.Id != baseConfiguration.GameProcessId)
                            {
                                baseConfiguration.GameProcessId = WzqGameProcess.Id;
                                chessRecognize.UpdateSetting();
                            }

                            if (chessRecognize.WzqProcess == null || (WzqGameProcess.Id != chessRecognize.WzqProcess.Id))
                            {
                                chessRecognize.UpdateSetting();
                            }
                        }
                    }
                    catch { }
                   
                    Thread.Sleep(200);
                }
            }).Start();
            
        }

        /// <summary>
        /// ���ò�ʱ��ÿһ�����ʱ�䳤��
        /// </summary>
        /// <param name="obj"></param>
        private void SaveTurnTimeout(string obj)
        {
            baseConfiguration.TurnTimeout = int.Parse(obj);
            baseConfiguration.Save();
            RenJunAiUpdate();
            DialogService.ShowMessageBox("��ʱ���³ɹ���", "��Ϣ");
        }

        public void ClosingCommand()
        {
            isClosing = true;
            chessRecognize.Stop();
            renjuAiHelper.End();
        }

        private void ViewLoaded(RoutedEventArgs e)
        {
            MainWindow mainWindow= e.Source as MainWindow;
            (mainWindow.FindName("textBox") as TextBox).Text = baseConfiguration.CaptureInterval.ToString();
            (mainWindow.FindName("textBoxTurnTimeout") as TextBox).Text = baseConfiguration.TurnTimeout.ToString();

            RadioRule4IsChecked = baseConfiguration.IsGameRule4;
            ComboxIndexAiRule4 = AiList.IndexOf(baseConfiguration.Rule4AiFileName);
            ComboxIndexAiRule0 = AiList.IndexOf(baseConfiguration.Rule0AiFileName);

            renjuAiHelper = new RenjuAiHelper();
            renjuAiHelper.AiCpuUtilizationRate += RenjuAiHelper_AiCpuUtilizationRate;
            renjuAiHelper.AiResponse += RenjuAiHelper_AiResponse;
            renjuAiHelper.CheckCpuInterval = 300;

            RenJunAiUpdate();
        }

        private void RenjuAiHelper_AiResponse(object sender, DataReceivedEventArgs e)
        {
            if (WzqGameProcess == null) return;

            string result = e.Data;
            if (string.IsNullOrEmpty(result)) return;

            Console.WriteLine("AI��Ϣ:\t" + result);
            if (result.StartsWith("name="))
            {
                AiAboutInfo = result;
                return;
            }


            result = result.Replace(" ", "");
            string[] chess = result.Split(',');
            if (chess.Length == 2)
            {
                //Y
                if (int.TryParse(chess[0], out int y))
                {
                    //X
                    if (int.TryParse(chess[1], out int x))
                    {
                        //Console.WriteLine($"��һ������λ�ã�{x},{y}");
                        int chesssCount = lastChessPointList.Count + 1;
                        if (chesssCount % 2 == 0)
                        {
                            NextStepInfo = $"�� {y + 1},{x + 1}";
                            Messenger.Default.Send(new ChessPointArgs
                            {
                                X = x + 1,
                                Y = y + 1,
                                ChessColor = ChessColors.White
                            });
                        }
                        else
                        {
                            NextStepInfo = $"�� {y + 1},{x + 1}";
                            Messenger.Default.Send(new ChessPointArgs
                            {
                                X = x + 1,
                                Y = y + 1,
                                ChessColor = ChessColors.Black
                            });
                        }
                    }
                }
            }
        }

        private void RenjuAiHelper_AiCpuUtilizationRate(Process process, double utilizationRate)
        {
            if(utilizationRate<0)
            {
                CpuBusyRate = 0;
                return;
            }

            if(utilizationRate>100)
            {
                CpuBusyRate = 100;
                return;
            }

            CpuBusyRate = (int)utilizationRate;
        }

        private void RenJunAiUpdate()
        {
            if (renjuAiHelper == null) return;
            NextStepInfo = "������...";

            //���µ���ʱ��
            renjuAiHelper.TurnTimeout = baseConfiguration.TurnTimeout;

            //��������
            if(baseConfiguration.IsGameRule4)
            {
                if(renjuAiHelper.AiFileName!= baseConfiguration.Rule4AiFileName)
                {
                    renjuAiHelper.AiFileName = baseConfiguration.Rule4AiFileName;
                    renjuAiHelper.End();
                    renjuAiHelper.StartAi();
                }
            }
            else
            {
                if (renjuAiHelper.AiFileName != baseConfiguration.Rule0AiFileName)
                {
                    renjuAiHelper.AiFileName = baseConfiguration.Rule0AiFileName;
                    renjuAiHelper.End();
                    renjuAiHelper.StartAi();
                }
            }

            //����
            renjuAiHelper.SetBoard(lastChessPointList);
        }

        /// <summary>
        /// ���ý�ͼʱ����
        /// </summary>
        /// <param name="obj"></param>
        private void SaveCaptureInteval(string obj)
        {
            baseConfiguration.CaptureInterval = int.Parse(obj);
            baseConfiguration.Save();
            chessRecognize.UpdateSetting();
            DialogService.ShowMessageBox("��ͼʱ�������³ɹ���", "��Ϣ");
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

                //������������
                lastChessPointList = e.ChessPointList;
                //��AI����������Ϣ
                renjuAiHelper.SetBoard(e.ChessPointList);
            }
        }
        Boolean isClosing = false;
        Process oldProcess = null;
        WindowProcess windowProcess = null;
        ChessRecognize chessRecognize = null;
        RenjuAiHelper renjuAiHelper;
        List<Tuple<int, int, int>> lastChessPointList = new List<Tuple<int, int, int>>();
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
        private Boolean radioRule4IsChecked;
        private List<string> aiList;
        private int comboxIndexAiRule4;
        private int comboxIndexAiRule0;
        private int cpuBusyRate = 0;
        private string aiAboutInfo;
        private string nextStepInfo;

        //����
        private RelayCommand chooseWzqGameRelayCommnad;
        public RelayCommand ChooseWzqGameRelayCommnad { get => chooseWzqGameRelayCommnad; set => chooseWzqGameRelayCommnad = value; }
        public RelayCommand<string> SaveCaptureIntevalCommand { get => saveCaptureIntevalCommand; set => saveCaptureIntevalCommand = value; }
        public RelayCommand<RoutedEventArgs> ViewLoadedCommnad { get => viewLoadedCommnad; set => viewLoadedCommnad = value; }
        public RelayCommand<string> SaveTurnTimeoutCommand { get => saveTurnTimeoutCommand; set => saveTurnTimeoutCommand = value; }
        public IDialogService DialogService { get; }
        public bool RadioRule4IsChecked
        {
            get => radioRule4IsChecked; 
            set
            {
                Boolean lastValue = radioRule4IsChecked;
                Set(ref radioRule4IsChecked, value);
                if (lastValue != radioRule4IsChecked)
                {
                    baseConfiguration.IsGameRule4 = radioRule4IsChecked;
                    baseConfiguration.Save();
                    RenJunAiUpdate();
                }
            }
        }

        public List<string> AiList { get => aiList; set => Set(ref aiList, value); }
        public int ComboxIndexAiRule4
        {
            get => comboxIndexAiRule4; 
            set
            {
                Set(ref comboxIndexAiRule4, value);
                if (comboxIndexAiRule4 >= 0 &&(baseConfiguration.Rule4AiFileName != AiList[comboxIndexAiRule4]))
                {
                    baseConfiguration.Rule4AiFileName = AiList[comboxIndexAiRule4];
                    baseConfiguration.Save();
                    RenJunAiUpdate();
                }
            }
        }

        public int ComboxIndexAiRule0
        {
            get => comboxIndexAiRule0; 
            set
            {
                Set(ref comboxIndexAiRule0, value);
                if (comboxIndexAiRule0 >= 0 && (baseConfiguration.Rule0AiFileName != AiList[comboxIndexAiRule0]))
                {
                    baseConfiguration.Rule0AiFileName = AiList[comboxIndexAiRule0];
                    baseConfiguration.Save();
                    RenJunAiUpdate();
                }
            }
        }

        public int CpuBusyRate { get => cpuBusyRate; set => Set(ref cpuBusyRate, value); }
        public string AiAboutInfo { get => aiAboutInfo; set => Set(ref aiAboutInfo, value); }
        public string NextStepInfo { get => nextStepInfo; set => Set(ref nextStepInfo, value); }

        private BaseConfiguration baseConfiguration;
        private RelayCommand<string> saveCaptureIntevalCommand;
        private RelayCommand<RoutedEventArgs> viewLoadedCommnad;

        private RelayCommand<string> saveTurnTimeoutCommand;

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