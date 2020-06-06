using OpenCvSharp;
using OpenCvSharp.Extensions;
using RenjuAdvisor.Event;
using RenjuAdvisor.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RenjuAdvisor.Utility
{
    public class ChessRecognize
    {
        Timer timer = null;
        BaseConfiguration baseConfiguration=null;
        Process wzqProcess = null;
        int chessStep = 0;
        int rows = 15;
        //计算棋子颜色的阈值
        Scalar scalarLower = new Scalar(128, 128, 128);
        Scalar scalarUpper = new Scalar(255, 255, 255);
        List<Tuple<int, int, int>> lastChessPointList = new List<Tuple<int, int, int>>();

        public Process WzqProcess { get => wzqProcess; set => wzqProcess = value; }

        public delegate void ChessRecognizeHandler(object sender, ChessRecongizedEventArgs e);
        public event ChessRecognizeHandler ChessRecognized;

        public ChessRecognize(IConfiguration configuration)
        {
            baseConfiguration = configuration as BaseConfiguration;

            timer = new Timer();
            timer.Interval = baseConfiguration.CaptureInterval;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bitmap bitmap = CaptureImage.Captuer(wzqProcess);
            if (bitmap == null) return;

            //裁剪棋盘
            int width = baseConfiguration.GameBoardInsideWidth;
            chessStep = width * 15 / 14 / 15;
            Bitmap wzqBoardImage = new Bitmap(width * 15 / 14, width * 15 / 14);
            Graphics g = Graphics.FromImage(wzqBoardImage);
            g.DrawImage(bitmap,
               new Rectangle(0, 0, wzqBoardImage.Width, wzqBoardImage.Height),
               new Rectangle(227 - chessStep / 2, 129 - chessStep / 2, wzqBoardImage.Width, wzqBoardImage.Height),
               GraphicsUnit.Pixel);
            g.Dispose();

            //识别棋子
            List<Tuple<int, int, int>> chessPointList = GetChessCoorinate(wzqBoardImage);

            Console.WriteLine("Captured -- " + DateTime.Now.ToString());

            //把旧棋子合并到新棋子中
            foreach (var item in lastChessPointList)
            {
                if (chessPointList.Contains(item) == false)
                {
                    chessPointList.Add(item);
                }
            }

            //判断棋子有没增加
            if (chessPointList.Count > lastChessPointList.Count)
            {
                lastChessPointList.Clear();
                foreach (var item in chessPointList)
                {
                    lastChessPointList.Add(item);
                }
                ChessRecognized?.Invoke(this, new ChessRecongizedEventArgs { ChessPointList = chessPointList });
            }
        }

        /// <summary>
        /// 识别棋子坐标&颜色
        /// </summary>
        public List<Tuple<int, int, int>> GetChessCoorinate(Bitmap wzqBoardImage)
        {
            //把Bitmap转换成Mat
            Mat boardMat = BitmapConverter.ToMat(wzqBoardImage);

            //因为霍夫圆检测对噪声比较敏感，所以首先对图像做一个中值滤波或高斯滤波(噪声如果没有可以不做)
            Mat blurBoardMat = new Mat();
            Cv2.MedianBlur(boardMat, blurBoardMat, 9);

            //转为灰度图像
            Mat grayBoardMat = new Mat();
            Cv2.CvtColor(blurBoardMat, grayBoardMat, ColorConversionCodes.BGR2GRAY);

            //3：霍夫圆检测：使用霍夫变换查找灰度图像中的圆。
            CircleSegment[] circleSegments = Cv2.HoughCircles(
                grayBoardMat,                       //The 8-bit, single-channel, grayscale input image
                HoughMethods.Gradient,              //Currently, the only implemented method is HoughCirclesMethod.Gradient
                1,                                  //The inverse ratio of the accumulator resolution to the image resolution.
                chessStep * 0.8,                    //Minimum distance between the centers of the detected circles.
                100,                                //The first method-specific parameter. [By default this is 100]
                10,                                 //10 The second method-specific parameter. [By default this is 100]
                (int)(chessStep * 0.4),             //Minimum circle radius. [By default this is 0]
                (int)(chessStep * 0.6));            //Maximum circle radius. [By default this is 0]

            //foreach (var circleSegment in circleSegments)
            //{
            //    Cv2.Circle(boardMat, (int)circleSegment.Center.X, (int)circleSegment.Center.Y, (int)circleSegment.Radius, Scalar.Red, 1, LineTypes.AntiAlias);
            //}
            //Cv2.ImShow("boardMat", boardMat);
            //Cv2.WaitKey();

            //判断棋子位置，遍历棋盘上的每个位置
            List<Tuple<int, int, int>> chessPointList = new List<Tuple<int, int, int>>();
 
            //行
            for (int i = 0; i < rows; i++)
            {
                //列
                for (int j = 0; j < rows; j++)
                {
                    //棋盘棋子坐标
                    Point2f point = new Point2f(j * chessStep + 0.5f * chessStep, i * chessStep + 0.5f * chessStep);
                    foreach (var circleSegment in circleSegments)
                    {
                        //有棋子
                        if (circleSegment.Center.DistanceTo(point) < 0.5 * chessStep)
                        {
                            //检查棋子的颜色
                            //以棋子中心为中心点，截取一部分图片（圆内切正方形），来计算图片颜色
                            //r^2 = a^2 + a^2
                            //--> a= ((r^2)/2)^-2

                            double len = Math.Sqrt(circleSegment.Radius * circleSegment.Radius / 2);
                            Rect rect = new Rect((int)(circleSegment.Center.X - len), (int)(circleSegment.Center.Y - len), (int)(len * 2), (int)(len * 2));
                            Mat squareMat = new Mat(grayBoardMat, rect);

                            //计算颜色
                            Mat calculatedMat = new Mat();
                            Cv2.InRange(squareMat, scalarLower, scalarUpper, calculatedMat);
                            float result = 100f * Cv2.CountNonZero(calculatedMat) / (calculatedMat.Width * calculatedMat.Height);

                            chessPointList.Add(new Tuple<int, int, int>(i + 1, j + 1, result < 50 ? 1 : 2));
                            break;
                        }
                    }
                }
            }

            return chessPointList;
        }

        /// <summary>
        /// 开始识别图片
        /// </summary>
        public void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// 停止识别图片
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// 从配置文件中更新参数
        /// </summary>
        public void UpdateSetting()
        {
            timer.Stop();

            timer.Interval = baseConfiguration.CaptureInterval;
            wzqProcess = Process.GetProcessById(baseConfiguration.GameProcessId);

            timer.Start();
        }
    }
}
