using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RenjuAdvisor.Event;
using RenjuAdvisor.Interface;
using RenjuAdvisor.Service;
using RenjuAdvisor.Utility;
using RenjuAdvisor.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RenjuAdvisor.ViewModel
{
    public class CutBoardViewModel : ViewModelBase, ICutBoard
    {
        private CutBoardArgs cutBoardArgs;

        //画在Canvas中的矩形
        System.Windows.Shapes.Rectangle drawingRectangle = null;

        public CutBoardViewModel(IConfiguration configuration)
        {
            //ViewLoadedCommnad = new RelayCommand(ViewLoaded);
            ViewLoadedCommnad = new RelayCommand<RoutedEventArgs>(ViewLoaded);
            CanvasMouseLeftButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(CanvasMouseLeftButtonDown);
            CanvasMouseMoveCommand = new RelayCommand<MouseEventArgs>(CanvasMouseMove);
            Configuration = configuration;
        }
        Canvas canvas = null;
        private void ViewLoaded(RoutedEventArgs e)
        {
            Bitmap bitmap = CaptureImage.Captuer(CutBoardArgs.WzqGameProcess);
            GameBoardBitmap = bitmap;

            CutBoardWindow cutBoardWindow = e.Source as CutBoardWindow;
            StackPanel stackPanel = cutBoardWindow.Content as StackPanel;

            //寻找Canvas
            foreach (var item in stackPanel.Children)
            {
                if (item is Canvas)
                {
                    canvas = item as Canvas;
                    break;
                }
            }

            Configuration.Load();
            var config = Configuration as BaseConfiguration;
            MouseDownLocation = new System.Windows.Point(config.GameBoardPoint.X, config.GameBoardPoint.Y);
            MouseUpLocation = new System.Windows.Point(
                MouseDownLocation.X + config.GameBoardInsideWidth,
                MouseDownLocation.Y + config.GameBoardInsideWidth);

            drawingRectangle = new System.Windows.Shapes.Rectangle()
            {
                Fill = System.Windows.Media.Brushes.DarkGray,
                Opacity = 0.8,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            canvas.Children.Add(drawingRectangle);

            //重新移动矩形的位置
            DrawRectangle();
        }

        //矩形左上角
        private System.Windows.Point mouseDownLocation;
        //矩形右下角
        private System.Windows.Point mouseUpLocation;
        //棋盘大小
        private System.Windows.Size rectangleSize;

        private IConfiguration configuration;

        private void CanvasMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //获取鼠标按下的位置
            MouseDownLocation = MouseUpLocation = e.GetPosition(canvas);

            canvas.Children.Remove(drawingRectangle);
            drawingRectangle = null;
        }

        private void CanvasMouseMove(MouseEventArgs e)
        {
            //确保鼠标左键按下
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //获取当前鼠标坐标
                MouseUpLocation = e.GetPosition(canvas);

                //鼠标当前位置与最开始点击的位置不同，
                //矩形还不存在，则创建矩形
                if(MouseUpLocation != MouseDownLocation && drawingRectangle==null)
                {
                    drawingRectangle = new System.Windows.Shapes.Rectangle()
                    {
                        Fill=System.Windows.Media.Brushes.DarkGray,
                        Opacity=0.8,
                        HorizontalAlignment= HorizontalAlignment.Left,
                        VerticalAlignment= VerticalAlignment.Top
                    };
                    canvas.Children.Add(drawingRectangle);
                }

                //重新移动矩形的位置
                DrawRectangle();
            }
        }

        /// <summary>
        /// 重新移动矩形的位置
        /// </summary>
        private void DrawRectangle()
        {
            if (drawingRectangle == null) return;
            drawingRectangle.Width = Math.Abs(MouseUpLocation.X - mouseDownLocation.X);
            drawingRectangle.Height = Math.Abs(MouseUpLocation.Y - mouseDownLocation.Y);
            Canvas.SetLeft(drawingRectangle, Math.Min(MouseUpLocation.X, mouseDownLocation.X));
            Canvas.SetTop(drawingRectangle, Math.Min(MouseUpLocation.Y, mouseDownLocation.Y));

            RectangleSize = new System.Windows.Size(drawingRectangle.Width, drawingRectangle.Height);
        }

        //SaveConfig
        public void SaveConfig()
        {
            System.Drawing.Point point = new System.Drawing.Point();
            point.X = Convert.ToInt32(drawingRectangle.GetValue(Canvas.LeftProperty));
            point.Y = Convert.ToInt32(drawingRectangle.GetValue(Canvas.TopProperty));

            var config = Configuration as BaseConfiguration;
            config.GameProcessId = CutBoardArgs.WzqGameProcess.Id;
            config.GameUiTitle = CutBoardArgs.WzqGameProcess.MainWindowTitle;
            config.GameUiSize = GameBoardBitmap.Size;
            config.GameBoardPoint = point;
            config.GameBoardInsideWidth = (int)((drawingRectangle.Width + drawingRectangle.Height) / 2);
            config.Save();

            Messenger.Default.Send(new CloseWindowArgs {  DialogResult=true});
        }

        public CutBoardArgs CutBoardArgs
        {
            get => cutBoardArgs;
            set
            {
                Set(ref cutBoardArgs, value);
            }
        }

        public RelayCommand<RoutedEventArgs> ViewLoadedCommnad { get; }
        public RelayCommand<MouseButtonEventArgs> CanvasMouseLeftButtonDownCommand { get; }
        public RelayCommand<MouseEventArgs> CanvasMouseMoveCommand { get; }

        private Bitmap gameBoardBitmap;
        public Bitmap GameBoardBitmap { get => gameBoardBitmap; set => Set(ref gameBoardBitmap, value); }
        public System.Windows.Point MouseDownLocation { get => mouseDownLocation; set => Set(ref mouseDownLocation , value); }
        public System.Windows.Point MouseUpLocation { get => mouseUpLocation; set => Set(ref mouseUpLocation, value); }
        public System.Windows.Size RectangleSize { get => rectangleSize; set => Set(ref rectangleSize, value); }
        public IConfiguration Configuration { get => configuration; set => configuration = value; }
    }
}
