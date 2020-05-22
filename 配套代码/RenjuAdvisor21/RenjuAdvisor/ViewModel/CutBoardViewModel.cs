using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RenjuAdvisor.Event;
using RenjuAdvisor.Interface;
using RenjuAdvisor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RenjuAdvisor.ViewModel
{
    public class CutBoardViewModel : ViewModelBase, ICutBoard
    {
        private CutBoardArgs cutBoardArgs;

        public CutBoardViewModel()
        {
            ViewLoadedCommnad = new RelayCommand(ViewLoaded);
        }

        private void ViewLoaded()
        {
            //MessageBox.Show("hi");
            //MessageBox.Show(CutBoardArgs.WzqGameProcess.MainWindowTitle);
            //游戏截图
            Bitmap bitmap = CaptureImage.Captuer(CutBoardArgs.WzqGameProcess);
            bitmap.Save("a.bmp");
            Process.Start("mspaint", "a.bmp");
        }

        public CutBoardArgs CutBoardArgs
        {
            get => cutBoardArgs;
            set
            {
                Set(ref cutBoardArgs, value);
            }
        } 
        
       public RelayCommand ViewLoadedCommnad { get; }
    }
}
