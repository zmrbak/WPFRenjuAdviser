using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Event
{
    public class ChessPointArgs : EventArgs
    {
        /// <summary>
        ///  棋子横向坐标 X
        /// </summary>
        public int X { set; get; }

        /// <summary>
        /// 棋子竖向坐标
        /// </summary>
        public int Y { set; get; }

        /// <summary>
        /// 棋子颜色
        /// </summary>
        public ChessColors ChessColor { set; get; }
    }
    public enum ChessColors
    {
        Black=1,
        White=2
    }
}
