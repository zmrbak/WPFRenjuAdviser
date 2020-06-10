using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Service
{
    public abstract class BaseConfiguration : IConfiguration
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
        /// 截图时间间隔
        /// </summary>
        public int CaptureInterval { set; get; } = 500;

        /// <summary>
        /// 步时，每一步棋的时间长度
        /// </summary>
        public int TurnTimeout { set; get; } = 25000;

        /// <summary>
        /// 游戏规则，是否有禁手
        /// 有禁手：true
        /// 无禁手：false
        /// </summary>
        public bool IsGameRule4 { set; get; }

        /// <summary>
        /// 有禁手的AI程序路径
        /// 指定在当前程序目录的AI目录之下
        /// </summary>
        public string Rule4AiFileName { set; get; }

        /// <summary>
        /// 无禁手的AI程序路径
        /// 指定在当前程序目录的AI目录之下
        /// </summary>
        /// 
        public string Rule0AiFileName { set; get; }

        /// <summary>
        /// 游戏窗口坐标
        /// </summary>
        public Point GameWindowPoint { set; get; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        protected abstract string FileSufix { get; }

        /// <summary>
        /// 存盘文件
        /// </summary>
        protected string SavedFileName
        {
            get
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "config");
                if (Directory.Exists(configPath) == false)
                {
                    Directory.CreateDirectory(configPath);
                }
                return Path.Combine(configPath, "appSetting." + FileSufix);
            }
        }

        protected void Deconstruct(
            out int GameProcessId, 
            out string GameUiTitle,
            out Size GameUiSize, 
            out Point GameBoardPoint, 
            out int GameBoardInsideWidth,
            out int CaptureInterval,
            out int TurnTimeout,
            out bool IsGameRule4,
            out string Rule4AiFileName,
            out string Rule0AiFileName)
        {
            GameProcessId = this.GameProcessId;
            GameUiTitle = this.GameUiTitle;
            GameUiSize = this.GameUiSize;
            GameBoardPoint = this.GameBoardPoint;
            GameBoardInsideWidth = this.GameBoardInsideWidth;
            CaptureInterval = this.CaptureInterval;
            TurnTimeout = this.TurnTimeout;
            IsGameRule4 = this.IsGameRule4;
            Rule4AiFileName = this.Rule4AiFileName;
            Rule0AiFileName = this.Rule0AiFileName;
        }

        public abstract bool Load();

        public abstract bool Save();
    }
}
