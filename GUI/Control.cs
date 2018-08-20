using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Printing;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using Core;

namespace GUI
{
    sealed class GUISettings : ApplicationSettingsBase

    {
        [DefaultSettingValue("127.0.0.1")]
        public string Server
        {
            get { return (string)this["Server"]; }
            set { this["Server"] = value; }
        }

        [DefaultSettingValue("21")]
        public int Port
        {
            get { return (int)this["Port"]; }
            set { this["Port"] = value; }
        }
    }
    class Printer
    {
        public Printer()
        {
            print = new PrintDialog();
            printQueue = new LocalPrintServer()
                .GetPrintQueues()
                .FirstOrDefault(p => p.Name == "Microsoft XPS Document Writer");
            print.PrintTicket.PageMediaSize = new PageMediaSize(58.0 * 0.03937 * 96, 100.0 * 0.03937 * 96);
        }

        private PrintDialog print;
        private PrintQueue printQueue;
        private Dictionary<string, FlowDocument> docs;

        public List<PrintQueue> PrintQueues
        {
            get
            {
                return new LocalPrintServer().GetPrintQueues().ToList();
            }
        }
        public PrintQueue UsingPrintQueue
        {
            get
            {
                return printQueue;
            }
            set
            {
                printQueue = value;
                print.PrintQueue = printQueue;
            }
        }

        public bool Print(string key)
        {
            try
            {
                FlowDocument document = docs[key];
                print.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, key);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool LoadRtfDoc(string FileName,string Key)
        {
            try
            {
                using (System.IO.FileStream fs = System.IO.File.Open(FileName, System.IO.FileMode.Open))
                {
                    FlowDocument doc = new FlowDocument();
                    TextRange content = new TextRange(doc.ContentStart, doc.ContentEnd);

                    content.Load(fs, DataFormats.Rtf);
                    docs[Key] = doc;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SaveRtfDoc(string FileName, string Key)
        {
            try
            {
                using (System.IO.FileStream fs = System.IO.File.Open(FileName, System.IO.FileMode.Create))
                {
                    FlowDocument doc = docs[Key];
                    TextRange content = new TextRange(doc.ContentStart, doc.ContentEnd);

                    content.Save(fs, DataFormats.Rtf);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool LoadXamlDoc(string FileName, string Key)
        {
            try
            {
                using (System.IO.FileStream fs = System.IO.File.Open(FileName, System.IO.FileMode.Open))
                {
                    FlowDocument doc = XamlReader.Load(fs) as FlowDocument;
                    docs[Key] = doc;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SaveXamlDoc(string FileName, string Key)
        {
            try
            {
                using (System.IO.FileStream fs = System.IO.File.Open(FileName, System.IO.FileMode.Create))
                {
                    FlowDocument doc = docs[Key];
                    XamlWriter.Save(doc, fs);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class Control
    {
        public class ValueInvaException : Exception
        {
            public ValueInvaException(string message) : base(message)
            {
            }
        }

        Core.Core Core = new Core.Core();
        GUISettings GUISettings = new GUISettings();

        TextBox StudentID_TextBox;
        TextBox CardID_TextBox;
        TextBox Balance_TextBox;
        public long StudentID
        {
            get
            {
                return long.Parse(StudentID_TextBox.Text);
            }
            set
            {
                if (value != 0)
                    StudentID_TextBox.Text = value.ToString();
                else
                    StudentID_TextBox.Text = "";
            }
        }
        public int CardID
        {
            get
            {
                return int.Parse(CardID_TextBox.Text);
            }
            set
            {
                if (value != 0)
                    CardID_TextBox.Text = value.ToString();
                else
                    CardID_TextBox.Text = "";
            }
        }
        public double Balance
        {
            get
            {
                return double.Parse(Balance_TextBox.Text);
            }
            set
            {
                if (value != 0)
                    Balance_TextBox.Text = value.ToString();
                else
                    Balance_TextBox.Text = "";
            }
        }

        public int Apple { get; set; }

        ComboBox CardTypeID_ComboBox;
        public int CardTypeID
        {
            get
            {
                return CardTypeID_ComboBox.SelectedIndex;
            }
            set
            {

            }
        }
        public char Sex { get; set; }
        public int Age { get; set; }

        TextBox StudentName_TextBox;
        TextBox PhoneNumber_TextBox;
        TextBox School_TextBox;
        TextBox Address_TextBox;
        TextBox Remarks_TextBox;
        public string StudentName
        {
            get
            {
                if (string.IsNullOrEmpty(StudentName_TextBox.Text))
                    return null;
                else
                    return StudentName_TextBox.Text;
            }
            set
            {
                StudentName_TextBox.Text = value;
            }
        }
        public string PhoneNumber { get; set; }
        public string School { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }

        public DateTime Birthday { get; set; }

        public double TotalMoney { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LastSignIn { get; set; }

        void FindStudent()
        {
            if (string.IsNullOrEmpty(StudentID_TextBox.Text))
            {
                //TODO
            }
            else
            {
                long studentID = 0;
                try
                {
                    studentID = StudentID;
                    if (studentID > Core.Settings.CardIDMax)
                    {
                        if (Core.FindStudent(StudentID))
                        {
                            //TODO
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (Core.FindStudent((int)StudentID))
                        {
                            //TODO
                        }
                        else
                        {

                        }
                    }
                }
                catch
                {
                    if (Core.FindStudent(StudentID_TextBox.Text))
                    {
                        //TODO
                    }
                    else
                    {

                    }
                }

            }
        }
        void SignIn()
        {
            if (Core.Student != null)
            {
                Core.SignIn();
                //TODO
            }
            else
            {

            }
        }


        void Print()
        {

        }
    }
}