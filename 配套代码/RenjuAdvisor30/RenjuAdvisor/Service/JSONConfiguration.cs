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
            try
            {
                string json = File.ReadAllText(SavedFileName);
                (GameProcessId, GameUiTitle, GameUiSize, GameBoardPoint, GameBoardInsideWidth) = new JavaScriptSerializer().Deserialize<JSONConfiguration>(json);
                return true;
            }
            catch
            {
                return false;
            }
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
