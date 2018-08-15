using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new StudentContext();
            //db.Settings.Add(new Setting { C = 888 });
            //db.Students.Add(new Student { StudentID = 233, CardTypeID = 1, Name = "Lzy" });
            //db.SaveChanges();
            //Console.WriteLine((from s in db.Settings select s).FirstOrDefault().C);
            Student student = new Student { StudentID = 233 };
            db.Students.Attach(student);
            student.Balance += 999;
            db.SaveChanges();
            Console.WriteLine(student.Name);
            //Console.ReadKey();

            Core core = new Core();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //耗时巨大的代码
            for (int i = 0; i < 100; i++)
            {
                using (var dbr = new RecordContext())
                {
                    Record record = (from r in dbr.Records
                                     where r.RecordID > 1000
                                     select r).FirstOrDefault();
                }

            }
                //core.Record("Test");

            sw.Stop();
            Console.WriteLine("总共花费{0}ms.", sw.Elapsed.TotalMilliseconds);
            Console.ReadKey();

        }
    }
}
