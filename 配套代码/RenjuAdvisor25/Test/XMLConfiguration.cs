using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test
{
    public class XMLConfiguration: BaseConfiguration
    {
        public override string FileSufix => "xml";

        public override bool Load()
        {
            try
            {
                string xml = File.ReadAllText(SavedFileName);
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XMLConfiguration));
                    XMLConfiguration a = xmlSerializer.Deserialize(sr) as XMLConfiguration;

                    GameProcessId = a.GameProcessId;
                    GameUiTitle = a.GameUiTitle;
                    GameUiSize = a.GameUiSize;
                    GameBoardPoint = a.GameBoardPoint;
                    GameBoardInsideWidth = a.GameBoardInsideWidth;

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
