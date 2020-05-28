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
    }
}
