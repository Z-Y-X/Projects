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
    class Printer
    {
        public Printer()
        {
            print = new PrintDialog();
            printQueue = new LocalPrintServer()
                .GetPrintQueues()
                .FirstOrDefault(p => p.Name == "Microsoft XPS Document Writer");
            print.PrintTicket.PageMediaSize = new PageMediaSize(58.0 * 0.03937 * 96, 100.0 * 0.03937 * 96);
            docs = new Dictionary<string, FlowDocument>();
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
        public List<string> PrintQueuesName
        {
            get
            {
                var q = from p in new LocalPrintServer().GetPrintQueues()
                        select p.Name;
                return q.ToList();
            }
        }
        public string UsingPrintQueueName
        {
            get
            {
                return printQueue.Name;
            }
            set
            {
                var q = from p in new LocalPrintServer().GetPrintQueues()
                        where p.Name == value
                        select p;
                printQueue = q.FirstOrDefault();
                print.PrintQueue = printQueue;
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

        public bool Print(string key, object data = null)
        {
            try
            {
                FlowDocument document = docs[key];
                document.DataContext = data;
                DocumentPaginator paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
                paginator.PageSize = new Size(58.0 * 0.03937 * 96, 100.0 * 0.03937 * 96);
                //A.Dispatcher.BeginInvoke(new PrintDocumentMethod(print.PrintDocument),
                //    System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                //    paginator, key);
                print.PrintDocument(paginator, key);
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