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

namespace GUI
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
            Result = Cancel_Button.Content.ToString();
        }
        public MessageWindow(string Ti, string Title, string Content,
            Color color = new Color(), string Ok = null, string Cancel = null)
        {
            InitializeComponent();

            T.Content = Ti;
            T.Foreground = new SolidColorBrush(color);
            Window.Title = Title;
            Content_Text.Text = Content;
            OK_Button.Content = Ok ?? "确定";
            Cancel_Button.Content = Cancel ?? "取消";
            Result = Cancel_Button.Content.ToString();
        }
        public string Result { get; private set; }

        public Color Color { set => T.Foreground = new SolidColorBrush(value); }
        public string ExTitle { get => T.Content.ToString(); set => T.Content = value; }
        public string WindowTitle { get => Window.Title.ToString(); set => Window.Title = value; }
        public string ContentText { get => Content_Text.Text.ToString(); set => Content_Text.Text = value; }
        public string Ok { get => OK_Button.Content.ToString(); set => OK_Button.Content = value; }
        public string Cancel { get => Cancel_Button.Content.ToString(); set => Cancel_Button.Content = value; }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Result = OK_Button.Content.ToString();
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Result = Cancel_Button.Content.ToString();
            Close();
        }
    }
}
