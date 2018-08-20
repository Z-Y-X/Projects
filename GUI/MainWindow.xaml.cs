using Core;
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

using Core;
using System.Globalization;

namespace GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Analysis analysis = new Analysis();
        Core.Core Core = new Core.Core();


        public MainWindow()
        {
            InitializeComponent();

            time_update_timer = new DispatcherTimer();
            time_update_timer.Tick += TimeUpdate_Tick;
            time_update_timer.Interval = new TimeSpan(0, 0, 60);
            time_update_timer.IsEnabled = true;
            TimeUpdate_Tick(null, null);

            Strudent_Date.DataContext = Core.Student;

            //Record_DataGrid.ItemsSource = analysis.GetAllRecords();
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

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (FindStudent())
            {
                if (Main_Tab.SelectedIndex == 1)
                {
                    if (SignIn())
                    {
                        Reward();
                    }
                }
            }
            Strudent_Date.DataContext = Core.Student;
            StudentID_TextBox.Clear();
        }
        
        private void EditOK_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveStudent();
            ToNotEditMode();
        }

        enum MessageType
        {
            Info,Warning,Error
        }
        void ShowMessage(string message, MessageType type = MessageType.Info)
        {
            Color color = new Color { A = 0xFF, R = 0x00, G = 0x00, B = 0x00 };
            switch (type)
            {
                case MessageType.Warning:
                    color.R = 0xAA;
                    color.G = 0x88;
                    break;
                case MessageType.Error:
                    color.R = 0xFF;
                    break;
                default:
                    break;
            }
            Message_Display.Foreground = new SolidColorBrush(color);
            Message_Display.Text += '\n' + message;
        }
        bool FindStudent()
        {
            if (string.IsNullOrEmpty(StudentID_TextBox.Text))
            {
                ShowMessage("请输入卡号", MessageType.Warning);
                return false;
            }
            else
            {
                long studentID = 0;
                try
                {
                    studentID = long.Parse(StudentID_TextBox.Text);
                    if (studentID > Core.Settings.CardIDMax)
                    {
                        if (Core.FindStudent(studentID))
                        {
                            ShowMessage("欢迎 " + Core.Student.Name, MessageType.Info);
                            return true;
                        }
                        else
                        {
                            ShowMessage("没有找到卡号：" + studentID, MessageType.Warning);
                            return false;
                        }
                    }
                    else
                    {
                        if (Core.FindStudent((int)studentID))
                        {
                            ShowMessage("使用卡表号 " + Core.Student.Name, MessageType.Info);
                            return true;
                        }
                        else
                        {
                            ShowMessage("没有找到卡表号：" + studentID, MessageType.Warning);
                            return false;
                        }
                    }
                }
                catch
                {
                    try
                    {
                        if (Core.FindStudent(StudentID_TextBox.Text))
                        {
                            ShowMessage("使用姓名 " + Core.Student.Name, MessageType.Info);
                            return true;
                        }
                        else
                        {
                            ShowMessage("没有找到姓名 " + StudentID_TextBox.Text, MessageType.Warning);
                            return false;
                        }
                    }
                    catch
                    {
                        ShowMessage("查找学生发生错误", MessageType.Error);
                        Core.ClearStudent();
                        return false;
                    }

                }
            }
        }
        bool SignIn()
        {
            try
            {
                if (Core.Student != null)
                {
                    Core.SignIn();
                    ShowMessage("签到成功", MessageType.Info);
                    return true;
                }
                else
                {
                    ShowMessage("签到发生逻辑错误", MessageType.Error);
                    return false;
                }
            }
            catch
            {
                ShowMessage("签到发生错误", MessageType.Error);
                return false;
            }
        }
        bool Reward()
        {
            try
            {
                if (Core.HasReward)
                {
                    ShowMessage("可以兑换礼品", MessageType.Info);
                    //TODO
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        bool SaveStudent()
        {
            try
            {
                Core.SaveStudent();
                ShowMessage("保存成功", MessageType.Info);
                return false;
            }
            catch
            {
                ShowMessage("保存失败", MessageType.Error);
                return false;
            }
        }


        void ToEditMode()
        {
            StudentEdit.IsEnabled = true;
            Edit_Button.Content = "取消";
            EditOK_Button.Visibility = Visibility.Visible;
        }
        void ToNotEditMode()
        {
            StudentEdit.IsEnabled = false;
            Edit_Button.Content = "编辑";
            EditOK_Button.Visibility = Visibility.Hidden;
            Core.ReloadStudent();
            Strudent_Date.DataContext = Core.Student;
        }
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (StudentEdit.IsEnabled)
                ToNotEditMode();
            else
                ToEditMode();
        }
        private void RefreshEdit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student != null)
                Core.ReloadStudent();
        }

        private double InputMoney { get; set; } = 0;
        private bool InputMoneyLock = false;
        private void Deposit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                Deposit_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (Core.DepositMoney(InputMoney))
                {
                    ShowMessage(Core.Student.Name + " 交款 " + InputMoney.ToString("F2") + "元 成功", MessageType.Info);
                    return;
                }
                else
                {
                    ShowMessage("交款失败", MessageType.Error);
                    return;
                }
            }
            catch
            {
                ShowMessage("交款发生错误", MessageType.Error);
                Core.ClearStudent();
                return;
            }
        }
        private void DepositMoney_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InputMoneyLock)
                return;
            if (Core.Student != null
                && double.TryParse(DepositMoney_TextBox.Text, out double money)
                && money > 0)
            {
                InputMoneyLock = true;
                InputMoney = money;
                DepositMonth_TextBox.Text = (InputMoney / Core.Student.CardType.MonthlyFee).ToString("F2");
                Deposit_Button.IsEnabled = true;
                InputMoneyLock = false;
            }
            else
            {
                Deposit_Button.IsEnabled = false;
            }
        }
        private void DepositMonth_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InputMoneyLock)
                return;
            if (Core.Student != null
                && double.TryParse(DepositMonth_TextBox.Text, out double month)
                && month > 0)
            {
                InputMoneyLock = true;
                InputMoney = month * Core.Student.CardType.MonthlyFee;
                DepositMoney_TextBox.Text = InputMoney.ToString("F2");
                Deposit_Button.IsEnabled = true;
                InputMoneyLock = false;
            }
            else
            {
                Deposit_Button.IsEnabled = false;
            }
        }

        private void Withdraw_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                Deposit_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (Core.Student.Balance >= InputMoney)
                {
                    if (Core.WithdrawMoney(InputMoney))
                    {
                        ShowMessage(Core.Student.Name + " 取款 " + InputMoney.ToString("F2") + "元 成功", MessageType.Info);
                        return;
                    }
                    else
                    {
                        ShowMessage("取款失败", MessageType.Error);
                        return;
                    }
                }
                else
                {
                    ShowMessage(Core.Student.Name + " 余额不足", MessageType.Warning);
                    return;
                }
            }
            catch
            {
                ShowMessage("取款发生错误", MessageType.Error);
                Core.ClearStudent();
                return;
            }
        }

        private void Consume_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                Deposit_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (Core.Student.Balance >= InputMoney)
                {
                    if (Core.WithdrawMoney(InputMoney))
                    {
                        ShowMessage(Core.Student.Name + " 消费 " + InputMoney.ToString("F2") + "元 成功", MessageType.Info);
                        return;
                    }
                    else
                    {
                        ShowMessage("消费失败", MessageType.Error);
                        return;
                    }
                }
                else
                {
                    ShowMessage(Core.Student.Name + " 余额不足", MessageType.Warning);
                    return;
                }
            }
            catch
            {
                ShowMessage("消费发生错误", MessageType.Error);
                Core.ClearStudent();
                return;
            }
        }

    }
}
