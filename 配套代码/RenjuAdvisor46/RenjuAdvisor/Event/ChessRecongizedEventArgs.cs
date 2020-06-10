using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Event
{
    public class ChessRecongizedEventArgs : EventArgs
    {
       public List<Tuple<int, int, int>> ChessPointList { set; get; }
    }
}
