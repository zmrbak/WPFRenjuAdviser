using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcesses();
            Process wzqProcess = null;
            foreach (var item in processes)
            {
                if (item.MainWindowTitle == "五子棋")
                {
                    Console.WriteLine(item.ProcessName);
                    Console.WriteLine(item.Id);
                    //窗口名
                    Console.WriteLine(item.MainWindowTitle);
                    Console.WriteLine(item.MainModule.FileName);
                    Console.WriteLine(item.MainModule.FileVersionInfo.FileVersion);
                    Console.WriteLine(item.MainModule.FileVersionInfo.FileDescription);
                    Console.WriteLine(item.MainModule.FileVersionInfo.Comments);
                    Console.WriteLine(item.MainModule.FileVersionInfo.CompanyName);
                    Console.WriteLine(item.MainModule.FileVersionInfo.FileName);
                    //产品名
                    Console.WriteLine(item.MainModule.FileVersionInfo.ProductName);
                    Console.WriteLine(item.MainModule.FileVersionInfo.ProductVersion);
                    Console.WriteLine(item.StartTime);
                    Console.WriteLine(item.MainWindowHandle);
                    wzqProcess = item;
                    break;
                }
            }
            Bitmap bitmap = CaptureImage.Captuer(wzqProcess);
            if (bitmap == null) return;

            //bitmap.Save("a.bmp");
            //Process.Start("mspaint", "a.bmp");
            //左上角
            //227 129
            //右下角
            //721 621
            int width = 721- 227;
            int height = 621- 129;
            int step = width * 15 / 14 / 15;

            Bitmap wzqBoardImage = new Bitmap(width * 15 / 14, height * 15 / 14);
            Graphics g = Graphics.FromImage(wzqBoardImage);
            //
            // 摘要:
            //     在指定位置并且按指定大小绘制指定的 System.Drawing.Image 的指定部分。
            //
            // 参数:
            //   image:
            //     要绘制的 System.Drawing.Image。
            //
            //   destRect:
            //     System.Drawing.Rectangle 结构，它指定所绘制图像的位置和大小。 将图像进行缩放以适合该矩形。
            //
            //   srcRect:
            //     System.Drawing.Rectangle 结构，它指定 image 对象中要绘制的部分。
            //
            //   srcUnit:
            //     System.Drawing.GraphicsUnit 枚举的成员，它指定 srcRect 参数所用的度量单位。
            g.DrawImage(bitmap,
                new Rectangle(0,0, wzqBoardImage.Width, wzqBoardImage.Height),
                new Rectangle(227- step/2, 129- step/2, wzqBoardImage.Width, wzqBoardImage.Height),
                GraphicsUnit.Pixel);
            g.Dispose();

            //把Bitmap转换成Mat
            Mat boardMat = BitmapConverter.ToMat(wzqBoardImage);

            //因为霍夫圆检测对噪声比较敏感，所以首先对图像做一个中值滤波或高斯滤波(噪声如果没有可以不做)
            Mat blurBoardMat = new Mat();
            Cv2.MedianBlur(boardMat, blurBoardMat,9);

            //转为灰度图像
            Mat grayBoardMat = new Mat();
            Cv2.CvtColor(blurBoardMat, grayBoardMat,ColorConversionCodes.BGR2GRAY);

            //3：霍夫圆检测：使用霍夫变换查找灰度图像中的圆。
            CircleSegment[] circleSegments = Cv2.HoughCircles(grayBoardMat, HoughMethods.Gradient, 1, step * 0.4, 70, 30, (int)(step * 0.3), (int)(step * 0.5));

            foreach (var circleSegment in circleSegments)
            {
                Cv2.Circle(boardMat, (int)circleSegment.Center.X, (int)circleSegment.Center.Y, (int)circleSegment.Radius, Scalar.Red,1,LineTypes.AntiAlias);
            }

           

            //判断棋子位置，遍历棋盘上的每个位置
            int rows = 15;
            List<Tuple<int, int>> chessPointList = new List<Tuple<int, int>>();
            //行
            for (int i = 0; i < rows; i++)
            {
                //列
                for (int j = 0; j < rows; j++)
                {
                    //棋盘棋子坐标
                    Point2f point = new Point2f(j* step+0.5f*step, i * step + 0.5f * step);
                    foreach (var circleSegment in circleSegments)
                    {
                        //有棋子
                        if(circleSegment.Center.DistanceTo(point) <0.5*step)
                        {
                            chessPointList.Add(new Tuple<int, int>(i+1,j+1));
                            break;
                        }
                    }
                }
            }

            foreach (var item in chessPointList)
            {
                Console.WriteLine($"{item.Item1},{item.Item2}");
            }
            Cv2.ImShow("boardMat", boardMat);
            Cv2.WaitKey();
        }
    }
}
