using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Behavior
{
    public class TextBoxErrorHelper : IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                string result = $"输入的数字必须是在{MinNumber}与{MaxNumber}之间的整数";
                if (columnName == "TextNumber")
                {
                    if (int.TryParse(TextNumber, out int number))
                    {
                        if (number >= MinNumber && number <= MaxNumber)
                        {
                            result = string.Empty;
                        }
                    }
                }
                return result;
            }
        }

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string TextNumber { set; get; }
        public int MinNumber { set; get; }
        public int MaxNumber { set; get; }
    }
}
