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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            time_update_timer = new DispatcherTimer();
            time_update_timer.Tick += TimeUpdate_Tick;
            time_update_timer.Interval = new TimeSpan(0,0,60);
            time_update_timer.IsEnabled = true;
            TimeUpdate_Tick(null, null);


        }

        private DispatcherTimer time_update_timer;
        private void TimeUpdate_Tick(object sender, EventArgs e)
        {
            TimeGrid.DataContext = new
            {
                Time = DateTime.Now.ToString("HH:mm"),
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Week = DateTime.Now.ToString("dddd")
            };
        }
    }
}
