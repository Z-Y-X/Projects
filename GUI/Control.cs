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
}