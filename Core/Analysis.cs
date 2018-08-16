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
            return StudentCount(s => s.LastSignIn.Value.Year == now.Year
                                  && s.LastSignIn.Value.Month == now.Month
                                  && s.LastSignIn.Value.Day == now.Day);
        }

        /// <summary>
        /// 统计签到人数
        /// </summary>
        /// <returns></returns>
        public int Count(int Year, int Month,int Day)
        {
            return RecordCount(r => r.RecordType == "签到"//Fixed
                                 && r.Timestamp.Year == Year
                                 && r.Timestamp.Month == Month
                                 && r.Timestamp.Day == Day);
        }
        public int Count(int Year, int Month)
        {
            return RecordCount(r => r.RecordType == "签到"//Fixed
                                 && r.Timestamp.Year == Year
                                 && r.Timestamp.Month == Month);
        }
        public int Count(int Year)
        {
            return RecordCount(r => r.RecordType == "签到"//Fixed
                                 && r.Timestamp.Year == Year);
        }
        public int Count()
        {
            return RecordCount(r => r.RecordType == "签到"//Fixed
                                                            );
        }


        /// <summary>
        /// 统计总签到收入
        /// </summary>
        /// <returns></returns>
        public double SignInEarning(int Year, int Month, int Day)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "签到"//Fixed
                               ,
                               r => r.Money);
        }
        public double SignInEarning(int Year, int Month)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "签到"//Fixed
                               ,
                               r => r.Money);
        }
        public double SignInEarning(int Year)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.RecordType == "签到"//Fixed
                               ,
                               r => r.Money);
        }
        public double SignInEarning()
        {
            return RecordSum(r => r.RecordType == "签到"//Fixed
                               ,
                               r => r.Money);
        }

        /// <summary>
        /// 统计总消费收入
        /// </summary>
        /// <returns></returns>
        public double ConsumeEarning(int Year, int Month, int Day)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "消费"//Fixed
                               ,
                               r => r.Money);
        }
        public double ConsumeEarning(int Year, int Month)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "消费"//Fixed
                               ,
                               r => r.Money);
        }
        public double ConsumeEarning(int Year)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.RecordType == "消费"//Fixed
                               ,
                               r => r.Money);
        }
        public double ConsumeEarning()
        {
            return RecordSum(r => r.RecordType == "消费"//Fixed
                               ,
                               r => r.Money);
        }

        /// <summary>
        /// 统计总充值金额
        /// </summary>
        /// <returns></returns>
        public double TotalDeposit(int Year, int Month, int Day)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "充值"//Fixed
                               ,
                               r => r.Money);
        }
        public double TotalDeposit(int Year, int Month)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "充值"//Fixed
                               ,
                               r => r.Money);
        }
        public double TotalDeposit(int Year)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.RecordType == "充值"//Fixed
                               ,
                               r => r.Money);
        }
        public double TotalDeposit()
        {
            return RecordSum(r => r.RecordType == "充值"//Fixed
                               ,
                               r => r.Money);
        }

        /// <summary>
        /// 统计总取款金额
        /// </summary>
        /// <returns></returns>
        public double TotalWithdraw(int Year, int Month, int Day)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.Timestamp.Day == Day
                               && r.RecordType == "取款"//Fixed
                               ,
                               r => r.Money);
        }
        public double TotalWithdraw(int Year, int Month)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.Timestamp.Month == Month
                               && r.RecordType == "取款"//Fixed
                               ,
                               r => r.Money);
        }
        public double TotalWithdraw(int Year)
        {
            return RecordSum(r => r.Timestamp.Year == Year
                               && r.RecordType == "取款"//Fixed
                               ,
                               r => r.Money);
        }
        public double TotalWithdraw()
        {
            return RecordSum(r => r.RecordType == "取款"//Fixed
                               ,
                               r => r.Money);
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

        public List<Student> GetStudents(Func<Student, bool> func)
        {
            List<Student> list;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where func(s)
                            select s;
                list = query.ToList();
            }
            return list;
        }
        public List<Record> GetRecords(Func<Record, bool> func)
        {
            List<Record> list;
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where func(r)
                            select r;
                list = query.ToList();
            }
            return list;
        }
    }
}