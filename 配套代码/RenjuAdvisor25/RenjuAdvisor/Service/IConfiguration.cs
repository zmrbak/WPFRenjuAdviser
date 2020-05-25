using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Service
{
    public interface IConfiguration
    {
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        bool Load();

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <returns></returns>
        bool Save();

        /// <summary>
        /// 游戏进程ID
        /// </summary>
        void SetGameProcessId(int gameProcessId);
        /// <summary>
        /// 游戏界面标题
        /// </summary>
        void SetGameUiTitle(string gameUiTitle);
        /// <summary>
        /// 游戏界面大小
        /// </summary>
        void SetGameUiSize(Size gameUiSize);
        /// <summary>
        /// 游戏中棋盘相对于游戏界面的左上角
        /// </summary>
        void SetGameBoardPoint(Point gameBoardPoint);
        /// <summary>
        /// 棋盘落子范围大小
        /// </summary>
        void SetGameBoardInsideWidth(int gameBoardInsideWidth);
    }

}
