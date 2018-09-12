using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;
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
            print.PrintTicket.PageBorderless = PageBorderless.Borderless;
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
                document.PagePadding = new Thickness(0);
                DocumentPaginator paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
                //paginator.PageSize = new Size(58.0 * 0.03937 * 96, 100.0 * 0.03937 * 96);
                paginator.PageSize = new Size(48.0 * 0.03937 * 96, 100.0 * 0.03937 * 96);
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

    class Backuper
    {
        private string backupDirectory;
        private string backupFile;
        private int maxSize;
        private bool enable;
        private bool autoBackup;
        private DispatcherTimer timer = new DispatcherTimer();
        public DateTime LastBackup { get; private set; } = DateTime.Now;
        public TimeSpan LastBackupInterval
        {
            get
            {
                return DateTime.Now - LastBackup;
            }
        }
        public TimeSpan BackupInterval { get; private set; }
        public int Size
        {
            get
            {
                return (from s in Directory.GetFiles(backupDirectory)
                        where s.EndsWith(".bak")
                        select s).Count();
            }
        }
        public bool IsEnable
        {
            get
            {
                return enable;
            }
            set
            {
                if (autoBackup)
                    timer.IsEnabled = value;
                enable = value;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Backup();
        }

        public static string DefaultDirectory = "..\\";
        public Backuper(string BackupFile, string BackupDirectory, TimeSpan MinBackupInterval, int MaxSize = 10, bool AutoBackup = false)
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    Directory.CreateDirectory(BackupDirectory);
                }
            }
            catch
            {
                BackupDirectory = DefaultDirectory;
                if (!Directory.Exists(BackupDirectory))
                {
                    Directory.CreateDirectory(BackupDirectory);
                }
            }

            backupFile = Path.GetFullPath(BackupFile);
            backupDirectory = Path.GetFullPath(BackupDirectory);
            BackupInterval = MinBackupInterval;
            maxSize = MaxSize;
            autoBackup = AutoBackup;

            if (AutoBackup)
            {
                timer.Interval = MinBackupInterval + MinBackupInterval;
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            enable = true;
        }

        public bool Backup(bool Force = false)
        {
            if (!enable || ((!Force && LastBackupInterval < BackupInterval) || !FileChanged()))
                return false;

            try
            {
                File.Copy(backupFile,
                    Path.Combine(backupDirectory, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".bak"));
                DeleteUnnecessaryBackup();
                LastBackup = DateTime.Now;
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void DeleteUnnecessaryBackup()
        {
            string[] files = Directory.GetFiles(backupDirectory);
            List<string> list = new List<string>();
            foreach (string s in files)
            {
                if (s.EndsWith(".bak"))
                {
                    list.Add(s);
                }
            }
            list.Sort();
            int count = list.Count();
            for (int i = 0; i < count - maxSize; i++)
            {
                try
                {
                    File.Delete(list[i]);
                    list.RemoveAt(i);
                }
                catch { }
            }
        }

        private DateTime fileLastWriteTime;
        private bool FileChanged()
        {
            FileInfo fileInfo = new FileInfo(backupFile);
            if (fileInfo.LastWriteTime != fileLastWriteTime)
            {
                fileLastWriteTime = fileInfo.LastWriteTime;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}