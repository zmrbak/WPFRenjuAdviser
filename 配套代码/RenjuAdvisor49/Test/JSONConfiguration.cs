﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Test
{
    public class JSONConfiguration : BaseConfiguration
    {
        public override string FileSufix { get => "json"; }

        public override bool Load()
        {
            string json = File.ReadAllText(SavedFileName);
           
            (GameProcessId, GameUiTitle, GameUiSize, GameBoardPoint, GameBoardInsideWidth) = new JavaScriptSerializer().Deserialize<JSONConfiguration>(json);

            //JSONConfiguration a = new JavaScriptSerializer().Deserialize<JSONConfiguration>(json);
            //GameProcessId = a.GameProcessId;
            //GameUiTitle = a.GameUiTitle;
            //GameUiSize = a.GameUiSize;
            //GameBoardPoint = a.GameBoardPoint;
            //GameBoardInsideWidth = a.GameBoardInsideWidth;

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
