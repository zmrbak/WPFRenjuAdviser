using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RenjuAdvisor.Event;
using RenjuAdvisor.Interface;
using RenjuAdvisor.Utility;
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

        public CutBoardViewModel()
        {
            ViewLoadedCommnad = new RelayCommand(ViewLoaded);
            CanvasMouseLeftButtonDownCommand = new RelayCommand<MouseButtonEventArgs>(CanvasMouseLeftButtonDown);
            CanvasMouseMoveCommand = new RelayCommand<MouseEventArgs>(CanvasMouseMove);
        }

        //矩形左上角
        private System.Windows.Point mouseDownLocation;
        //矩形右下角
        private System.Windows.Point mouseUpLocation;
        //棋盘大小
        private System.Windows.Size rectangleSize;


        private void CanvasMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //寻找父节点Canvas
            FrameworkElement element = e.Source as FrameworkElement;
            while (element is Canvas == false)
            {
                element = element.Parent as FrameworkElement;
            }
            Canvas canvas = element as Canvas;

            List<Object> rectangleList = new List<object>();
            foreach (var item in canvas.Children)
            {
                if(item is Rectangle)
                {
                    rectangleList.Add(item);
                }
            }

            foreach (var item in rectangleList)
            {
                canvas.Children.Remove((UIElement)item);
            }

            //获取鼠标按下的位置
            MouseDownLocation = MouseUpLocation = e.GetPosition(canvas);

            canvas.Children.Remove(drawingRectangle);
            drawingRectangle = null;
        }

        private void CanvasMouseMove(MouseEventArgs e)
        {
            //寻找父节点Canvas
            FrameworkElement element = e.Source as FrameworkElement;
            while (element is Canvas == false)
            {
                element = element.Parent as FrameworkElement;
            }
            Canvas canvas = element as Canvas;

            //确保鼠标左键按下
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //获取当前鼠标坐标
                System.Windows.Point p = e.GetPosition(canvas);
                MouseUpLocation = p;

                //鼠标当前位置与最开始点击的位置不同，
                //矩形还不存在，则创建矩形
                if(p!= MouseDownLocation && drawingRectangle==null)
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
                if (drawingRectangle == null) return;
                drawingRectangle.Width =Math.Abs( p.X - mouseDownLocation.X);
                drawingRectangle.Height= Math.Abs(p.Y - mouseDownLocation.Y);
                Canvas.SetLeft(drawingRectangle,Math.Min( p.X, mouseDownLocation.X));
                Canvas.SetTop(drawingRectangle, Math.Min(p.Y, mouseDownLocation.Y));

                RectangleSize = new System.Windows.Size(drawingRectangle.Width, drawingRectangle.Height);
            }
        }

       

        private void ViewLoaded()
        {
            //MessageBox.Show("hi");
            //MessageBox.Show(CutBoardArgs.WzqGameProcess.MainWindowTitle);
            //游戏截图
            Bitmap bitmap = CaptureImage.Captuer(CutBoardArgs.WzqGameProcess);
            //bitmap.Save("a.bmp");
            //Process.Start("mspaint", "a.bmp");
            GameBoardBitmap = bitmap;
        }

        public CutBoardArgs CutBoardArgs
        {
            get => cutBoardArgs;
            set
            {
                Set(ref cutBoardArgs, value);
            }
        }

        public RelayCommand ViewLoadedCommnad { get; }
        public RelayCommand<MouseButtonEventArgs> CanvasMouseLeftButtonDownCommand { get; }
        public RelayCommand<MouseEventArgs> CanvasMouseMoveCommand { get; }

        private Bitmap gameBoardBitmap;
        public Bitmap GameBoardBitmap { get => gameBoardBitmap; set => Set(ref gameBoardBitmap, value); }
        public System.Windows.Point MouseDownLocation { get => mouseDownLocation; set => Set(ref mouseDownLocation , value); }
        public System.Windows.Point MouseUpLocation { get => mouseUpLocation; set => Set(ref mouseUpLocation, value); }
        public System.Windows.Size RectangleSize { get => rectangleSize; set => Set(ref rectangleSize, value); }
    }
}
