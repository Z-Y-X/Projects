using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Core
{
    public class Analysis
    {
        private double RecordSum(Func<Record,bool> whereFunc, Expression<Func<Record, double?>> sumFunc)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where whereFunc(r)
                            select r;
                sum = query.Sum(sumFunc) ?? 0;
            }
            return sum;
        }
        private int RecordCount(Func<Record, bool> func)
        {
            int count = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where func(r)
                            select r;
                count = query.Count();
            }
            return count;
        }
        private double StudentSum(Func<Student, bool> whereFunc, Expression<Func<Student, double?>> sumFunc)
        {
            double sum = 0;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where whereFunc(s)
                            select s;
                sum = query.Sum(sumFunc) ?? 0;
            }
            return sum;
        }
        private int StudentCount(Func<Student, bool> func)
        {
            int count = 0;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where func(s)
                            select s;
                count = query.Count();
            }
            return count;
        }

        /// <summary>
        /// 统计今日签到人数
        /// </summary>
        /// <returns></returns>
        public int TodayCount()
        {
            DateTime now = DateTime.Now;
            int count = 0;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.LastSignIn.Value.Year == now.Year
                                  && s.LastSignIn.Value.Month == now.Month
                                  && s.LastSignIn.Value.Day == now.Day
                            select s;
                count = query.Count();
            }
            return count;
        }

        /// <summary>
        /// 统计签到人数
        /// </summary>
        /// <returns></returns>
        public int Count(DateTime begin,DateTime end)
        {
            end = end.AddDays(1);
            int count = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                                 && begin <= r.Timestamp && r.Timestamp < end
                            select r;
                count = query.Count();
            }
            return count;
        }
        public int Count(int Year, int Month,int Day)
        {
            int count = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                                 && r.Timestamp.Year == Year
                                 && r.Timestamp.Month == Month
                                 && r.Timestamp.Day == Day
                            select r;
                count = query.Count();
            }
            return count;
        }
        public int Count(int Year, int Month)
        {
            int count = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                                 && r.Timestamp.Year == Year
                                 && r.Timestamp.Month == Month
                            select r;
                count = query.Count();
            }
            return count;
        }
        public int Count(int Year)
        {
            int count = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                                 && r.Timestamp.Year == Year
                            select r;
                count = query.Count();
            }
            return count;
        }
        public int Count()
        {
            int count = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                            select r;
                count = query.Count();
            }
            return count;
        }


        /// <summary>
        /// 统计总签到收入
        /// </summary>
        /// <returns></returns>
        public double SignInEarning(DateTime begin, DateTime end)
        {
            end = end.AddDays(1);
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                                 && begin <= r.Timestamp && r.Timestamp < end
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double SignInEarning(int Year, int Month, int Day)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "签到"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double SignInEarning(int Year, int Month)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "签到"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double SignInEarning(int Year)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.RecordType == "签到"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double SignInEarning()
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "签到"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }

        /// <summary>
        /// 统计总消费收入
        /// </summary>
        /// <returns></returns>
        public double ConsumeEarning(DateTime begin, DateTime end)
        {
            end = end.AddDays(1);
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "消费"//Fixed
                                 && begin <= r.Timestamp && r.Timestamp < end
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double ConsumeEarning(int Year, int Month, int Day)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "消费"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double ConsumeEarning(int Year, int Month)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "消费"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double ConsumeEarning(int Year)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.RecordType == "消费"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double ConsumeEarning()
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "消费"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }

        /// <summary>
        /// 统计总充值金额
        /// </summary>
        /// <returns></returns>
        public double TotalDeposit(DateTime begin, DateTime end)
        {
            end = end.AddDays(1);
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "充值"//Fixed
                                 && begin <= r.Timestamp && r.Timestamp < end
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalDeposit(int Year, int Month, int Day)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "充值"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalDeposit(int Year, int Month)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "充值"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalDeposit(int Year)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.RecordType == "充值"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalDeposit()
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "充值"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }

        /// <summary>
        /// 统计总取款金额
        /// </summary>
        /// <returns></returns>
        public double TotalWithdraw(DateTime begin, DateTime end)
        {
            end = end.AddDays(1);
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "取款"//Fixed
                                 && begin <= r.Timestamp && r.Timestamp < end
                            select r;

                List<Record> records = new List<Record>();
                var quer = from r in db.Records
                            where records.Contains(r)
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalWithdraw(int Year, int Month, int Day)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "取款"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalWithdraw(int Year, int Month)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "取款"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalWithdraw(int Year)
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.Timestamp.Year == Year
                               && r.RecordType == "取款"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }
        public double TotalWithdraw()
        {
            double sum = 0;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.RecordType == "取款"//Fixed
                            select r;
                sum = query.Sum(r => r.Money) ?? 0;
            }
            return sum;
        }

        /// <summary>
        /// 查询学生的全部记录
        /// </summary>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        public Dictionary<string,List<Record>> GetRecords(long StudentID)
        {
            Dictionary<string, List<Record>> dict;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.StudentID == StudentID
                            //group r by r.RecordType into rg
                            select r;
                dict = query.ToList()
                    .GroupBy(o => o.RecordType)
                    .ToDictionary(k => k.Key, y => y.ToList());
            }
            return dict;
        }

        public List<Student> GetAllStudents()
        {
            List<Student> list;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            select s;
                list = query.ToList();
            }
            return list;
        }
        public List<Record> GetAllRecords()
        {
            List<Record> list;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            select r;
                list = query.ToList();
            }
            return list;
        }
    }
}