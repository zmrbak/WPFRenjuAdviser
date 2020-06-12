using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RenjuAdvisor.Converter
{
    public class CpuBusyRateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return 0;
            if (values[0].ToString() == "0") return 0;           

            //底部Rectagnle宽度
            double backgroundRectangleWidth = double.Parse(values[0].ToString());
            //遮盖Rectangle宽度的比例
            double foregroundRectanglePercent= double.Parse(values[1].ToString());
            //遮盖Rectangle宽度
            return backgroundRectangleWidth * (100 - foregroundRectanglePercent) / 100;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
