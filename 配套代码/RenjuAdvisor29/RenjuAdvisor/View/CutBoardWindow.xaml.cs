using GalaSoft.MvvmLight.Messaging;
using RenjuAdvisor.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RenjuAdvisor.View
{
    /// <summary>
    /// CutBoardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CutBoardWindow : Window
    {
        public CutBoardWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<CloseWindowArgs>(this, OnReceivedCloseWindowArgs);
            this.Unloaded += CutBoardWindow_Unloaded;
        }

        private void CutBoardWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<CloseWindowArgs>(this, OnReceivedCloseWindowArgs);
        }

        private void OnReceivedCloseWindowArgs(CloseWindowArgs e)
        {
            this.DialogResult = e.DialogResult;
        }
    }
}
