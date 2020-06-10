using GalaSoft.MvvmLight.Messaging;
using RenjuAdvisor.Event;
using RenjuAdvisor.Service;
using RenjuAdvisor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RenjuAdvisor.View
{
    /// <summary>
    /// ArrowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ArrowWindow : Window
    {
        BaseConfiguration baseConfiguration;
        Timer timer;
        ChessPointArgs chessPointArgs;
        public ArrowWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<ChessPointArgs>(this, OnRecivedChessPointArgs);
            baseConfiguration = (DataContext as ArrowViewModel).Configuration;

            timer = new Timer();
            timer.Interval = 200;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            this.Visibility = Visibility.Collapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MoveWindow();
        }

        private void OnRecivedChessPointArgs(ChessPointArgs e)
        {
            timer.Stop();
            chessPointArgs = e;
            MoveWindow();
            timer.Start();

            Dispatcher.Invoke(() =>
            {
                this.Visibility = Visibility.Visible;
                WindowState = WindowState.Normal;
            });
        }

        private void MoveWindow()
        {
            if (chessPointArgs == null) return;

            double width = baseConfiguration.GameBoardInsideWidth * 15 / 14;
            double step = width / 15;
            double chessLeft = (chessPointArgs.X - 1) * step + baseConfiguration.GameBoardPoint.X;
            double chessTop = (chessPointArgs.Y - 1) * step + baseConfiguration.GameBoardPoint.Y;

            Dispatcher.Invoke(() =>
            {
                this.Top = baseConfiguration.GameWindowPoint.Y + chessTop + step / 4;
                this.Left = baseConfiguration.GameWindowPoint.X + chessLeft + step / 4;
                if(chessPointArgs.ChessColor== ChessColors.Black)
                {
                    this.pathArrow.Fill = Brushes.Black;
                    this.pathArrow.Stroke = Brushes.White;
                }
                else
                {
                    this.pathArrow.Fill = Brushes.White;
                    this.pathArrow.Stroke = Brushes.Black;
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
        }

        private void pathArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
