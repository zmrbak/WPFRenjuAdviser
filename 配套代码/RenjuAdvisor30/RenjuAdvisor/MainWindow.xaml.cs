using GalaSoft.MvvmLight.Messaging;
using Gma.System.MouseKeyHook;
using RenjuAdvisor.Event;
using RenjuAdvisor.Interface;
using RenjuAdvisor.View;
using RenjuAdvisor.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RenjuAdvisor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<CutBoardArgs>(this, OnReceivedCutBoardArgs);
        }

        private void OnReceivedCutBoardArgs(CutBoardArgs obj)
        {
            CutBoardWindow cutBoardWindow = new CutBoardWindow();
            //((CutBoardViewModel)(cutBoardWindow.DataContext)).CutBoardArgs = obj;
            (cutBoardWindow.DataContext as ICutBoard).CutBoardArgs = obj;
            var result = cutBoardWindow.ShowDialog();
            if (result == true)
            {
                (this.DataContext as IReloadData).ReloadData();
            }
        }
    }
}
