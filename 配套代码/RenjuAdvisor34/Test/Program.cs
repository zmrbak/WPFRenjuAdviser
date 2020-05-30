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
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int index = 0;
            IConfiguration configuration = new XMLConfiguration();
            ConfigurationTest configurationTest = new ConfigurationTest(configuration,index);
            configuration.Load();

           
            while(true)
            {
                index++;
                (configuration as BaseConfiguration).GameProcessId = index;
                Console.ReadLine();
            }
           
            ;
            //var a1 = configuration as IConfiguration;
            
            //var a11 = configuration as BaseConfiguration;
            //a11.GameProcessId = 111;

            //var a2 = configuration as JSONConfiguration;
            //a2.GameUiTitle = "aaa";
            //var a3 = configuration as XMLConfiguration;
            //a3.GameUiTitle = "bbb";

            ;

            //JSONConfiguration configuration = new JSONConfiguration();
            ////configuration.GameProcessId = 100;
            //configuration.Load();
            //configuration.Save();

            //XMLConfiguration xMLConfiguration = new XMLConfiguration();
            //xMLConfiguration.GameProcessId = 150;
            //xMLConfiguration.Save();



            //var b = new JavaScriptSerializer().Deserialize<Configuration>(a);
            ;
            ////游戏进程ID
            //int GameProcessId=0;
            //// 游戏界面标题
            //string GameUiTitle="";
            ////游戏界面大小
            //Size GameUiSize;
            ////游戏中棋盘相对于游戏界面的左上角
            //Point GameBoardPoint;
            ////棋盘落子范围大小
            //int GameBoardInsideWidth;

            //String result = "GameProcessId:" + GameProcessId+Environment.NewLine;
            //result += "GameUiTitle:" + GameUiTitle + Environment.NewLine;

            //String Json = "{\"GameProcessId\":"+ GameProcessId + ",\"GameUiTitle\":"+ GameUiTitle +"}";
            //string xml = "";
        }
        static void Main2(string[] args)
        {
            List<Tuple<int, int, int>> chessPointList = new List<Tuple<int, int, int>>();
            chessPointList.Add(new Tuple<int, int, int>(7, 6, 2));
            //chessPointList.Add(new Tuple<int, int, int>(6, 7, 1));
            //chessPointList.Add(new Tuple<int, int, int>(7, 8, 2));
            //chessPointList.Add(new Tuple<int, int, int>(7, 10, 1));


            RenjuAiHelper renjuAiHelper = new RenjuAiHelper();
            renjuAiHelper.AiFileName = @"C:\Users\Zmrbak\Desktop\piskvork (1)\pbrain-Yixin2018.exe";
            renjuAiHelper.AiResponse += RenjuAiHelper_AiResponse;
            renjuAiHelper.AiCpuUtilizationRate += RenjuAiHelper_AiCpuUtilizationRate;
            renjuAiHelper.Size = 15;
            renjuAiHelper.Rule = 4;
            renjuAiHelper.Timeout_turn = 5000;
            renjuAiHelper.Timeout_match = 50000;
            renjuAiHelper.StartAi();
            

            renjuAiHelper.SetBoard(chessPointList);
            Console.ReadLine();

            renjuAiHelper.SetBoard(chessPointList);
            Console.ReadLine();

            renjuAiHelper.SetBoard(chessPointList);
            Console.ReadLine();

            renjuAiHelper.End();
        }

        private static void RenjuAiHelper_AiCpuUtilizationRate(Process process, double utilizationRate)
        {
            Console.WriteLine("CPU利用率："+ utilizationRate);
        }

        private static void RenjuAiHelper_AiResponse(object sender, DataReceivedEventArgs e)
        {
            string result = e.Data;
            //if (result.StartsWith("DEBUG")) return;
            //if (result.StartsWith("OK")) return;

            Console.WriteLine(e.Data);
        }

        static void Main1(string[] args)
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
            List<Tuple<int, int,int>> chessPointList = new List<Tuple<int, int,int>>();
            //计算棋子颜色的阈值
            Scalar scalarLower = new Scalar(128, 128, 128);
            Scalar scalarUpper = new Scalar(255, 255, 255);

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
                            //检查棋子的颜色
                            //以棋子中心为中心点，截取一部分图片（圆内切正方形），来计算图片颜色
                            //r^2 = a^2 + a^2
                            //--> a= ((r^2)/2)^-2

                            double len=  Math.Sqrt(circleSegment.Radius * circleSegment.Radius / 2);
                            Rect rect = new Rect((int)(circleSegment.Center.X - len), (int)(circleSegment.Center.Y - len), (int)(len * 2), (int)(len * 2));
                            Mat squareMat = new Mat(grayBoardMat, rect);

                            //计算颜色
                            Mat calculatedMat = new Mat();
                            Cv2.InRange(squareMat, scalarLower, scalarUpper, calculatedMat);
                            float result = 100f * Cv2.CountNonZero(calculatedMat) / (calculatedMat.Width * calculatedMat.Height);

                            chessPointList.Add(new Tuple<int, int, int>(i + 1, j + 1, result < 50 ? 0 : 1));
                            break;
                        }
                    }
                }
            }

            foreach (var item in chessPointList)
            {
                Console.WriteLine($"{item.Item1},{item.Item2},{item.Item3}");
            }
            Cv2.ImShow("boardMat", boardMat);
            Cv2.WaitKey();
        }
    }
}
