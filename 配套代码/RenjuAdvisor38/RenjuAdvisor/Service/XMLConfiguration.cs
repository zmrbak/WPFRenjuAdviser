using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RenjuAdvisor.Service
{
    public class XMLConfiguration : BaseConfiguration
    {
        protected override string FileSufix => "xml";

        public override bool Load()
        {
            try
            {
                string xml = File.ReadAllText(SavedFileName);
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLConfiguration));
                    (GameProcessId, GameUiTitle, GameUiSize, GameBoardPoint, GameBoardInsideWidth, CaptureInterval, TurnTimeout, IsGameRule4, Rule4AiFileName, Rule0AiFileName)
                                = xmlSerializer.Deserialize(sr) as XMLConfiguration;
                    return true;
                }
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
                MemoryStream Stream = new MemoryStream();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLConfiguration));
                xmlSerializer.Serialize(Stream, this);

                Stream.Position = 0;
                StreamReader sr = new StreamReader(Stream);
                string str = sr.ReadToEnd();
                sr.Dispose();
                Stream.Dispose();

                File.WriteAllText(SavedFileName, str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
