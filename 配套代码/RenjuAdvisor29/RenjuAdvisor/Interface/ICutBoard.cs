using RenjuAdvisor.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.Interface
{
    public interface ICutBoard
    {
        CutBoardArgs CutBoardArgs { set; get; }
    }
}
