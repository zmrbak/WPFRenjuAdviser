using GalaSoft.MvvmLight;
using RenjuAdvisor.Event;
using RenjuAdvisor.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.ViewModel
{
    public class CutBoardViewModel : ViewModelBase, ICutBoard
    {
        private CutBoardArgs cutBoardArgs;
        public CutBoardArgs CutBoardArgs
        {
            get => cutBoardArgs;
            set
            {
                Set(ref cutBoardArgs, value);
            }
        }        
    }
}
