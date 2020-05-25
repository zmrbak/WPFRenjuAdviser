using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RenjuAdvisor.Service
{
    public class JSONConfiguration : BaseConfiguration
    {
        protected override string FileSufix { get => "json"; }

        public override bool Load()
        {
            string json = File.ReadAllText(SavedFileName);
            JSONConfiguration a = new JavaScriptSerializer().Deserialize<JSONConfiguration>(json);

            GameProcessId = a.GameProcessId;
            GameUiTitle = a.GameUiTitle;
            GameUiSize = a.GameUiSize;
            GameBoardPoint = a.GameBoardPoint;
            GameBoardInsideWidth = a.GameBoardInsideWidth;

            return true;
        }

        public override bool Save()
        {
            try
            {
                string json = new JavaScriptSerializer().Serialize(this);
                File.WriteAllText(SavedFileName, json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
