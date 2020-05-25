using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Test
{
    public abstract class BaseConfiguration:IConfiguration
    {
        /// <summary>
        /// 游戏进程ID
        /// </summary>
        public int GameProcessId { set; get; }

        /// <summary>
        /// 游戏界面标题
        /// </summary>
        public string GameUiTitle { set; get; }

        /// <summary>
        /// 游戏界面大小
        /// </summary>
        public Size GameUiSize { set; get; }

        /// <summary>
        /// 游戏中棋盘相对于游戏界面的左上角
        /// </summary>
        public Point GameBoardPoint { set; get; }

        /// <summary>
        /// 棋盘落子范围大小
        /// </summary>
        public int GameBoardInsideWidth { set; get; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public abstract string FileSufix { get; }

        /// <summary>
        /// 存盘文件
        /// </summary>
        protected string SavedFileName
        {
            get
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "config");
                if(Directory.Exists(configPath)==false)
                {
                    Directory.CreateDirectory(configPath);
                }
                return Path.Combine(configPath, "appSetting."+ FileSufix);
            }
        }

        public abstract bool Load();

        public abstract bool Save();

        public void SetGameBoardInsideWidth(int gameBoardInsideWidth)
        {
            GameBoardInsideWidth = gameBoardInsideWidth;
        }


        public void SetGameBoardPoint(Point gameBoardPoint)
        {
            GameBoardPoint = gameBoardPoint;
        }

        public void SetGameProcessId(int gameProcessId)
        {
            GameProcessId = gameProcessId;
        }

        public void SetGameUiSize(Size gameUiSize)
        {
            GameUiSize = gameUiSize;
        }

        public void SetGameUiTitle(string gameUiTitle)
        {
            GameUiTitle = gameUiTitle;
        }

       
    }
}
