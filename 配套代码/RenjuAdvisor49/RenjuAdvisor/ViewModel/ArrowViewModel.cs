using GalaSoft.MvvmLight;
using RenjuAdvisor.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenjuAdvisor.ViewModel
{
    public class ArrowViewModel : ViewModelBase
    {
        public ArrowViewModel(IConfiguration configuration)
        {
            Configuration = configuration as BaseConfiguration;
        }
        public BaseConfiguration Configuration { get; }
    }
}
