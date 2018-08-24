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
        Analysis Analysis = new Analysis();
        Core.Core Core = new Core.Core();
        Printer Printer = new Printer();

        public MainWindow()
        {
            InitializeComponent();

            Init_TimeUpdate_Timer();

            InitPrinter();

            LoadCardTypes();

            if (!Core.LastNormalClosed)
            {
                new AbnormalClosedWindow().ShowDialog();
                //string title = "未正常关闭";
                //string msg = "上次软件没有正常关闭，请检查数据。";
                //MessageBoxButton buttons = MessageBoxButton.OK;
                //MessageBoxImage icon = MessageBoxImage.Warning;

                //MessageBoxResult result = MessageBox.Show(msg, title, buttons, icon);
            }
            //Strudent_Date.DataContext = Core.Student;
            //Record_DataGrid.ItemsSource = Analysis.GetAllRecords();
        }

        private DispatcherTimer time_update_timer;
        private void Init_TimeUpdate_Timer()
        {
            time_update_timer = new DispatcherTimer();
            time_update_timer.Tick += TimeUpdate_Tick;
            time_update_timer.Interval = new TimeSpan(0, 0, 60);
            time_update_timer.IsEnabled = true;
            TimeUpdate_Tick(null, null);
        }
        private void TimeUpdate_Tick(object sender, EventArgs e)
        {
            TimeGrid.DataContext = new
            {
                Time = DateTime.Now.ToString("HH:mm"),
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Week = DateTime.Now.ToString("dddd")
            };
        }

        void LoadCardTypes()
        {
            CardType_ComboBox.ItemsSource = Core.GetCardTypes();
            CardType_ComboBox.SelectedValue = 1;
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

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (FindStudent())
            {
                if (Core.Student.Balance < Core.Student.CardType.CostPerLesson * 2)//TODO
                {
                    Arrearage();
                }
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
                            if (Properties.Settings.Default.AllowNewStudent)
                            {
                                bool ok = false;
                                if (!Properties.Settings.Default.AutoNewStudent)
                                {
                                    string title = "这张卡没有被使用";
                                    string msg = "录入新的学生学生信息？";
                                    MessageBoxButton buttons = MessageBoxButton.YesNo;
                                    MessageBoxImage icon = MessageBoxImage.Question;

                                    MessageBoxResult result = MessageBox.Show(msg, title, buttons, icon);

                                    ok = (result == MessageBoxResult.Yes);
                                }
                                else
                                {
                                    ok = true;
                                }
                                if (ok)
                                {
                                    if (NewStudent(studentID))
                                    {
                                        ToEditMode();
                                        Main_Tab.SelectedIndex = 2;
                                    }
                                }
                            }
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
                        ClearStudent();
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
                    if (!Core.HasSignIn)
                    {
                        Core.SignIn();
                        ShowMessage("签到成功", MessageType.Info);
                        if (!Printer.Print("签到", new { Core.Student, DateTime.Now }))
                        {
                            ShowMessage("小票打印失败", MessageType.Warning);
                        }
                        return true;
                    }
                    else if(Properties.Settings.Default.AllowSignInAgain)
                    {
                        bool ok = false;
                        if (Properties.Settings.Default.AutoSignInAgain)
                        {
                            ok = true;
                        }
                        else
                        {
                            string title = "今天已经签过到了";
                            string msg = "再次签到？";
                            MessageBoxButton buttons = MessageBoxButton.YesNo;
                            MessageBoxImage icon = MessageBoxImage.Question;

                            MessageBoxResult result = MessageBox.Show(msg, title, buttons, icon);

                            ok = (result == MessageBoxResult.Yes);
                        }
                        if (ok)
                        {
                            Core.SignIn();
                            ShowMessage("重复签到成功", MessageType.Info);
                            if (!Printer.Print("签到", new { Core.Student, DateTime.Now }))
                            {
                                ShowMessage("小票打印失败", MessageType.Warning);
                            }
                        }
                    }
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
                    if (!Printer.Print("苹果", new { Core.Student, DateTime.Now }))
                    {
                        ShowMessage("小票打印失败", MessageType.Warning);
                    }
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
        void ReloadStudent()
        {
            if (Core.Student != null)
            {
                Core.ReloadStudent();
                Strudent_Date.DataContext = Core.Student;
            }
        }
        void ClearStudent()
        {
            if (Core.Student != null)
            {
                Core.ClearStudent();
                Strudent_Date.DataContext = Core.Student;
            }
        }
        bool NewStudent(long StudentID)
        {
            try
            {
                Core.NewStudent(StudentID);
                ShowMessage("开卡成功", MessageType.Info);
                return true;
            }
            catch
            {
                ShowMessage("开卡失败", MessageType.Error);
                return false;
            }
        }
        void Arrearage()
        {
            ShowMessage("请注意余额", MessageType.Warning);
            if (!Printer.Print("余额", new { Core.Student, DateTime.Now }))
            {
                ShowMessage("小票打印失败", MessageType.Warning);
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
            ReloadStudent();
        }
        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!StudentEdit.IsEnabled && Core.Student != null)
                ToEditMode();
            else
                ToNotEditMode();
        }
        private void RefreshEdit_Button_Click(object sender, RoutedEventArgs e)
        {
            ReloadStudent();
        }
        private void EditOK_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveStudent();
            ReloadStudent();
            ToNotEditMode();
        }

        private double InputMoney = 0;
        private bool InputMoneyLock = false;
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
                DepositLesson_TextBox.Text = (InputMoney / Core.Student.CardType.CostPerLesson).ToString("F2");
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
                DepositLesson_TextBox.Text = (InputMoney / Core.Student.CardType.CostPerLesson).ToString("F2");
                Deposit_Button.IsEnabled = true;
                InputMoneyLock = false;
            }
            else
            {
                Deposit_Button.IsEnabled = false;
            }
        }
        private void DepositLesson_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InputMoneyLock)
                return;
            if (Core.Student != null
                && double.TryParse(DepositLesson_TextBox.Text, out double lesson)
                && lesson > 0)
            {
                InputMoneyLock = true;
                InputMoney = lesson * Core.Student.CardType.CostPerLesson;
                DepositMoney_TextBox.Text = InputMoney.ToString("F2");
                DepositMonth_TextBox.Text = (InputMoney / Core.Student.CardType.MonthlyFee).ToString("F2");
                Deposit_Button.IsEnabled = true;
                InputMoneyLock = false;
            }
            else
            {
                Deposit_Button.IsEnabled = false;
            }
        }
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
                    ReloadStudent();
                    if (!Printer.Print("充值", new { Core.Student, DateTime.Now }))
                    {
                        ShowMessage("小票打印失败", MessageType.Warning);
                    }
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
                ClearStudent();
                return;
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
                        ReloadStudent();
                        if (!Printer.Print("取款", new { Core.Student, DateTime.Now }))
                        {
                            ShowMessage("小票打印失败", MessageType.Warning);
                        }
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
                ClearStudent();
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
                    if (Core.ConsumeMoney(InputMoney))
                    {
                        ShowMessage(Core.Student.Name + " 消费 " + InputMoney.ToString("F2") + "元 成功", MessageType.Info);
                        ReloadStudent();
                        if (!Printer.Print("消费", new { Core.Student, DateTime.Now }))
                        {
                            ShowMessage("小票打印失败", MessageType.Warning);
                        }
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
                ClearStudent();
                return;
            }
        }


        private int InputApple = 0;
        private void Apple_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Core.Student != null
                && int.TryParse(Apple_TextBox.Text, out int apple)
                && apple > 0)
            {
                InputApple = apple;
                AppleAdd_Button.IsEnabled = true;
            }
            else
            {
                AppleAdd_Button.IsEnabled = false;
            }
        }
        private void AppleAdd_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                AppleAdd_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (Core.AddApple(InputApple))
                {
                    ShowMessage(Core.Student.Name + " 奖励 " + InputApple + "个 苹果", MessageType.Info);
                    ReloadStudent();
                    return;
                }
                else
                {
                    ShowMessage("奖励失败", MessageType.Error);
                    return;
                }
            }
            catch
            {
                ShowMessage("奖励发生错误", MessageType.Error);
                ClearStudent();
                return;
            }
        }
        private void AppleSub_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                AppleAdd_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (InputApple <= Core.Student.Apple)
                {
                    if (Core.AddApple(-InputApple))
                    {
                        ShowMessage(Core.Student.Name + " 扣除 " + InputApple + "个 苹果", MessageType.Info);
                        ReloadStudent();
                        return;
                    }
                    else
                    {
                        ShowMessage("奖励失败", MessageType.Error);
                        return;
                    }
                }
                else
                {
                    ShowMessage("苹果不足", MessageType.Warning);
                    return;
                }
            }
            catch
            {
                ShowMessage("扣除发生错误", MessageType.Error);
                ClearStudent();
                return;
            }
        }

        private long InputNewStudentID = 0;
        private void NewStudentID_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Core.Student != null
                && long.TryParse(NewStudentID_TextBox.Text, out InputNewStudentID)
                && InputNewStudentID > Core.Settings.CardIDMax)
            {
                Replace_Button.IsEnabled = true;
            }
            else
            {
                Replace_Button.IsEnabled = false;
            }
        }
        private void Replace_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                Replace_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (Core.ReplaceCard(InputNewStudentID))
                {
                    ShowMessage(Core.Student.Name + " 换卡成功", MessageType.Info);
                    ReloadStudent();
                    return;
                }
                else
                {
                    ShowMessage("换卡错误", MessageType.Error);
                    return;
                }
            }
            catch
            {
                ShowMessage("换卡发生错误", MessageType.Error);
                ClearStudent();
                return;
            }
        }
        private void Recover_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Student == null)
            {
                Replace_Button.IsEnabled = false;
                ShowMessage("请先选择学生", MessageType.Warning);
                return;
            }
            try
            {
                if (Core.RecoverCard())
                {
                    ShowMessage(Core.Student.Name + " 回收成功", MessageType.Info);
                    ClearStudent();
                    return;
                }
                else
                {
                    ShowMessage("回收错误", MessageType.Error);
                    return;
                }
            }
            catch
            {
                ShowMessage("回收发生错误", MessageType.Error);
                ClearStudent();
                return;
            }
        }

        private void TodayAnalysis_Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime d = DateTime.Now;
            TodayAnalysis_Data.DataContext = new AnalysisData
            {
                SignInCount = Analysis.TodayCount(),
                SignInEarning = Analysis.SignInEarning(d.Year, d.Month, d.Day),
                ConsumeEarning = Analysis.ConsumeEarning(d.Year, d.Month, d.Day),
                DepositEarning = Analysis.TotalDeposit(d.Year, d.Month, d.Day),
                WithdrawEarning = Analysis.TotalWithdraw(d.Year, d.Month, d.Day),
            };
        }
        private void MonthAnalysis_Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime d = DateTime.Now;
            MonthAnalysis_Data.DataContext = new AnalysisData
            {
                SignInCount = Analysis.Count(d.Year, d.Month),
                SignInEarning = Analysis.SignInEarning(d.Year, d.Month),
                ConsumeEarning = Analysis.ConsumeEarning(d.Year, d.Month),
                DepositEarning = Analysis.TotalDeposit(d.Year, d.Month),
                WithdrawEarning = Analysis.TotalWithdraw(d.Year, d.Month),
            };
        }
        private void Analysis_Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime begin = AnalysisBegin_DatePicker.SelectedDate ?? DateTime.Now;
            DateTime end = AnalysisEnd_DatePicker.SelectedDate ?? DateTime.Now;
            Analysis_Data.DataContext = new AnalysisData
            {
                SignInCount = Analysis.Count(begin, end),
                SignInEarning = Analysis.SignInEarning(begin, end),
                ConsumeEarning = Analysis.ConsumeEarning(begin, end),
                DepositEarning = Analysis.TotalDeposit(begin, end),
                WithdrawEarning = Analysis.TotalWithdraw(begin, end),
            };
        }

        private void CardTypeAdd_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CardType cardType = (CardType)CardTypeEdit_ComboBox.SelectedValue;
                cardType.CostPerLesson = cardType.MonthlyFee / cardType.MonthlyClass;
                Core.AddCardType(cardType);
                ShowMessage("添加卡类型成功", MessageType.Info);
                return;
            }
            catch
            {
                ShowMessage("无法添加卡类型", MessageType.Error);
                return;
            }
        }
        private void CardTypeEdit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CardTypeEdit.IsEnabled)
            {
                try
                {
                    CardType cardType = (CardType)CardTypeEdit_ComboBox.SelectedValue;
                    cardType.CostPerLesson = cardType.MonthlyFee / cardType.MonthlyClass;
                    Core.SaveCardType(cardType);
                    ShowMessage("保存卡类型成功", MessageType.Info);
                }
                catch
                {
                    ShowMessage("保存失败", MessageType.Error);
                }
                LoadCardTypes();
                CardTypeEdit.IsEnabled = false;
                CardTypeEdit_Button.Content = "编辑";
                return;
            }
            else
            {
                CardTypeEdit_Button.Content = "保存";
                CardTypeEdit.IsEnabled = true;
            }
        }

        private void QueryRecord_Button_Click(object sender, RoutedEventArgs e)
        {
            HashSet<string> vs = new HashSet<string>();
            foreach (var c in QueryRecord_CheckBoxGroup.Children)
            {
                try
                {
                    CheckBox checkBox = (CheckBox)c;
                    if (checkBox.IsChecked ?? false)
                        vs.Add((string)checkBox.Content);
                }
                catch { }
            }
            List<Record> records;
            if (QueryRecordAllStudents_CheckBox.IsChecked ?? false)
            {
                if (QueryRecordBegin_DatePicker.IsEnabled)
                {
                    records = Analysis.GetRecords(vs,
                        QueryRecordBegin_DatePicker.SelectedDate ?? DateTime.Now,
                        QueryRecordEnd_DatePicker.SelectedDate ?? DateTime.Now);
                }
                else
                {
                    records = Analysis.GetRecords(vs);
                }
            }
            else
            {
                if (Core.Student == null)
                {
                    ShowMessage("请先选择学生", MessageType.Warning);
                    return;
                }

                if (QueryRecordBegin_DatePicker.IsEnabled)
                {
                    records = Analysis.GetRecords(vs,Core.Student.StudentID,
                        QueryRecordBegin_DatePicker.SelectedDate ?? DateTime.Now,
                        QueryRecordEnd_DatePicker.SelectedDate ?? DateTime.Now);
                }
                else
                {
                    records = Analysis.GetRecords(vs, Core.Student.StudentID);
                }
            }
            QueryRecordCount_Label.Content = records.Count;
            Record_DataGrid.ItemsSource = records;
        }
        private void QueryRecordAllRecords_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var c in QueryRecord_CheckBoxGroup.Children)
            {
                try
                {
                    ((CheckBox)c).IsChecked = true;
                }
                catch { }
            }
        }
        private void QueryRecordAllRecords_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var c in QueryRecord_CheckBoxGroup.Children)
            {
                try
                {
                    ((CheckBox)c).IsChecked = false;
                }
                catch { }
            }
        }

        private void StudentData_Button_Click(object sender, RoutedEventArgs e)
        {
            List<Student> students=Analysis.GetAllStudents();
            StudentDataCount_Label.Content = students.Count();
            StudentData.ItemsSource = students;
        }

        void InitPrinter()
        {
            LoadPrintDocument();
            Printer_ComboBox.ItemsSource = Printer.PrintQueuesName;
            Printer.UsingPrintQueueName = Properties.Settings.Default.SelectedPrinter;
            //
            //if(!Printer.Print("签到",new { Core.Student, DateTime.Now }))
            //{
            //    ShowMessage("小票打印失败", MessageType.Warning);
            //}
            //
        }
        void LoadPrintDocument()
        {
            try
            {
                Printer.LoadXamlDoc("../Resource/SignIn.xaml", "签到");
                Printer.LoadXamlDoc("../Resource/Balance.xaml", "余额");
                Printer.LoadXamlDoc("../Resource/Deposit.xaml", "充值");
                Printer.LoadXamlDoc("../Resource/Apple.xaml", "苹果");
                Printer.LoadXamlDoc("../Resource/Withdraw.xaml", "取款");
                Printer.LoadXamlDoc("../Resource/Consume.xaml", "消费");
            }
            catch
            {
                ShowMessage("打印资源加载错误 可能无法正常打印", MessageType.Error);
            }
        }


        private void SettingsSave_Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Printer_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Printer.UsingPrintQueueName = (string)Printer_ComboBox.SelectedValue;
        }

        private void SettingsReset_Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Core.CloseCore();
        }
    }

    class AnalysisData
    {
        public int SignInCount { get; set; }
        public double SignInEarning { get; set; }
        public double ConsumeEarning { get; set; }
        public double DepositEarning { get; set; }
        public double WithdrawEarning { get; set; }
        public double TotalEarning { get => SignInEarning + ConsumeEarning; }
    }
}
