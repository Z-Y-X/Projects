using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Core
{
    public class Core
    {
        public Core()
        {
            LoadSettings();
        }

        private Student student;

        #region Setting
        public Setting Settings { get; private set; } = null;
        /// <summary>
        /// 上次正常关闭
        /// </summary>
        public bool LastNormalClosed { get; private set; } = true;
        /// <summary>
        /// 软件初次启动
        /// </summary>
        public bool FirstStartup { get; private set; } = false;
        /// <summary>
        /// 加载设置
        /// </summary>
        public void LoadSettings()
        {
            using (var db = new StudentContext())
            {
                var query = from s in db.Settings
                            select s;
                Settings = query.FirstOrDefault();
                if (Settings == null)
                {
                    LastNormalClosed = true;
                    FirstStartup = true;

                    CardType cardType = new CardType
                    {
                        CardTypeID = 1,
                        CostPerLesson = 0,
                        MonthlyClass = 0,
                        MonthlyFee = 0,
                        TypeName = "默认请修改"
                    };
                    db.CardTypes.Add(cardType);

                    Settings = new Setting();
                    db.Settings.Add(Settings);
                    db.SaveChanges();
                }
                else
                {
                    LastNormalClosed = Settings.Closed;
                    FirstStartup = false;

                    Settings.Closed = false;
                    Settings.LastStartup = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }

        public void CloseCore()
        {
            using (var db = new StudentContext())
            {
                db.Settings.Attach(Settings);
                Settings.Closed = true;
                db.SaveChanges();
            }
        }

        //private void DefaultCardType()
        //{
        //    using (var db = new StudentContext())
        //    {
        //        var query = from c in db.CardTypes
        //                    select c;
        //        if (query.Count() == 0)
        //        {
        //            CardType cardType = new CardType
        //            {
        //                CardTypeID = 1,
        //                CostPerLesson = 0,
        //                MonthlyClass = 0,
        //                MonthlyFee = 0,
        //                TypeName = "默认请修改"
        //            };
        //            db.CardTypes.Add(cardType);
        //            db.SaveChanges();
        //        }
        //    }
        //}
        public void AddCardType(CardType cardType)
        {
            using (var db = new StudentContext())
            {
                db.CardTypes.Add(cardType);
                db.SaveChanges();
            }
        }
        public void AddCardType(List<CardType> cardTypes)
        {
            using (var db = new StudentContext())
            {
                db.CardTypes.AddRange(cardTypes);
                db.SaveChanges();
            }
        }
        public void SaveCardType(CardType cardType)
        {
            using (var db = new StudentContext())
            {
                db.CardTypes.Attach(cardType);
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(cardType, EntityState.Modified);
                db.SaveChanges();
            }
        }
        public List<CardType> GetCardTypes()
        {
            List<CardType> list;
            using (var db = new StudentContext())
            {
                var query = from c in db.CardTypes
                            select c;
                list = query.ToList();
            }
            return list;
        }
        #endregion

        #region Student
        public Student Student
        {
            get
            {
                return student;
            }
            set
            {
                if (value == null)
                {
                    HasReward = false;
                }
                student = value;
            }
        }
        /// <summary>
        /// 当前学生应该得到奖励
        /// </summary>
        public bool HasReward { get; private set; }
        /// <summary>
        /// 当前学生已经签过到了
        /// </summary>
        public bool HasSignIn
        {
            get
            {
                if (student == null || student.LastSignIn == null)
                    return false;

                DateTime dt = student.LastSignIn.Value;
                DateTime now = DateTime.Now;
                return dt.Year == now.Year
                    && dt.Month == now.Month
                    && dt.Day == now.Day;
            }
        }

        /// <summary>
        /// 按StudentID尝试查找学生
        /// </summary>
        /// <param name="StudentID">卡序列号</param>
        /// <returns></returns>
        public bool FindStudent(long StudentID)
        {
            using (var db =new StudentContext())
            {
                var query = from s in db.Students
                          where s.StudentID == StudentID
                          select s;
                Student = query.FirstOrDefault();
                if ((student?.CardType?.CostPerLesson ?? 1) < 0)
                    throw new Exception("Cost Error");//确保CardType已经加载
            }
            return (student != null);
        }
        /// <summary>
        /// 按CardID尝试查找学生
        /// </summary>
        /// <param name="CardID">卡表号</param>
        /// <returns></returns>
        public bool FindStudent(int CardID)
        {
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.CardID == CardID
                            select s;
                Student = query.FirstOrDefault();
                if ((student?.CardType?.CostPerLesson ?? 1) < 0)
                    throw new Exception("Cost Error");//确保CardType已经加载
            }
            return (student != null);
        }
        /// <summary>
        /// 按Name尝试查找学生
        /// </summary>
        /// <param name="Name">卡表号</param>
        /// <returns></returns>
        public bool FindStudent(string Name)
        {
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.Name == Name
                            select s;
                Student = (query.Count() == 1) ?
                           query.FirstOrDefault()
                                           : null;
                if ((student?.CardType?.CostPerLesson ?? 1) < 0)
                    throw new Exception("Cost Error");//确保CardType已经加载
            }
            return (student != null);
        }

        public void SignIn()
        {
            if (student == null)
            {
                //TODO
                return;
            }
            using (var db = new StudentContext())
            {
                db.Students.Attach(student);

                student.Balance -= student.CardType.CostPerLesson;
                student.Apple += 1;
                student.LastSignIn = DateTime.Now;

                HasReward = student.Apple % 10 == 0;

                db.SaveChanges();
                Record("签到", student, student.CardType.CostPerLesson);//Fixed
            }
        }

        private void MoneyAdd(Student student,double money)
        {
            using (var db = new StudentContext())
            {
                db.Students.Attach(student);
                student.Balance += money;
                db.SaveChanges();
            }
        }
        public bool DepositMoney(double Money)
        {
            if (student == null || Money <= 0)
                return false;

            MoneyAdd(student, Money);
            Record("充值", student, Money);//Fixed
            return true;
        }
        public bool DepositMoneyByMonth(int Month)
        {
            if (student == null || Month <= 0)
                return false;

            double money = Month * student.CardType.MonthlyFee;
            MoneyAdd(student, money);
            Record("充值", student, money, $"充值{Month}月");//Fixed
            return true;
        }
        public bool DepositMoneyByLesson(int Lesson)
        {
            if (student == null || Lesson <= 0)
                return false;

            double money = Lesson * student.CardType.CostPerLesson;
            MoneyAdd(student, money);
            Record("充值", student, money, $"充值{Lesson}节课");//Fixed
            return true;
        }

        /// <summary>
        /// 卡中取款
        /// </summary>
        /// <param name="Money">金额</param>
        /// <returns></returns>
        public bool WithdrawMoney(double Money)
        {
            if (student == null)
                return false;

            if (student.Balance >= Money)
            {
                using (var db = new StudentContext())
                {
                    db.Students.Attach(student);

                    student.Balance -= Money;

                    db.SaveChanges();
                    Record("取款", student, Money);//Fixed
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 用卡消费
        /// </summary>
        /// <param name="Money">金额</param>
        /// <returns></returns>
        public bool ConsumeMoney(double Money)
        {
            if (student == null)
                return false;

            if (student.Balance >= Money)
            {
                using (var db = new StudentContext())
                {
                    db.Students.Attach(student);

                    student.Balance -= Money;

                    db.SaveChanges();
                    Record("消费", student, Money);//Fixed
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 回收卡
        /// </summary>
        /// <returns></returns>
        public bool RecoverCard()
        {
            if (student == null)
                return false;

            using (var db = new StudentContext())
            {
                db.Students.Attach(student);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            Record("回收", student);//Fixed
            return true;
        }

        public bool HasContainStudent(long StudentID)
        {
            int count = 0;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.StudentID == StudentID
                            select s;
                count = query.Count();
            }
            return count != 0;
        }
        public bool HasContainStudent(string Name)
        {
            int count = 0;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.Name == Name
                            select s;
                count = query.Count();
            }
            return count != 0;
        }
        public bool HasContainStudent(int CardID)
        {
            int count = 0;
            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.CardID == CardID
                            select s;
                count = query.Count();
            }
            return count != 0;
        }

        /// <summary>
        /// 更换卡
        /// </summary>
        /// <param name="NewStudentID">新卡号</param>
        /// <returns></returns>
        public bool ReplaceCard(long NewStudentID)
        {
            if (student == null)
                return false;

            using (var db = new StudentContext())
            {
                var query = from s in db.Students
                            where s.StudentID == NewStudentID
                            select s;
                if (query.Count() != 0)//确保不会因重复而误删除
                    return false;

                db.Students.Attach(student);
                db.Students.Remove(student);
                db.SaveChanges();

                student.StudentID = NewStudentID;

                db.Students.Add(student);
                db.SaveChanges();
            }
            Record("换卡", student);//Fixed
            return true;
        }

        /// <summary>
        /// 保存当前学生信息（仅在发生外部编辑时使用）
        /// </summary>
        public void SaveStudent()
        {
            using (var db = new StudentContext())
            {
                student.CardType = null;
                db.Students.Attach(student);
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(student, EntityState.Modified);
                db.SaveChanges();
                if (student.StartDate != new DateTime(630835470000000000))
                {
                    Record("编辑", student);//Fixed
                }                                       // 我知道这段代码在我出生的瞬间不能正确执行
                else                                    // 在我的代码里每一个新人与我同时诞生
                {                                       // 但你开始的时间不能是我的生日
                    student.StartDate = DateTime.Now;   // 我会让你重新开始
                    Record("开卡", student);//Fixed
                    db.SaveChanges();
                }
            }
        }

        public void NewStudent(long StudentID)
        {
            using (var db = new StudentContext())
            {
                Student = new Student { StudentID = StudentID };
                student.StartDate = new DateTime(630835470000000000);
                student.CardTypeID = 1;
                db.Students.Add(student);
                db.SaveChanges();
                //Record("开卡", student);//Fixed
            }
        }

        /// <summary>
        /// 清除当前学生
        /// </summary>
        public void ClearStudent()
        {
            Student = null;
        }

        public void ReloadStudent()
        {
            if (student != null)
                FindStudent(student.StudentID);
        }

        public bool AddApple(int n)
        {
            if (student == null)
                return false;

            using (var db = new StudentContext())
            {
                db.Students.Attach(student);
                student.Apple += n;
                db.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 检查是否应该奖励（不判断null）
        /// </summary>
        /// <returns>应该奖励</returns>
        //private bool CheckReward()//TODO
        //{
        //    return student.Apple % 10 == 0;
        //}
        #endregion

        #region Record
        /// <summary>
        /// 增加一条学生记录
        /// </summary>
        /// <param name="RecordType">记录类型</param>
        /// <param name="Student">产生记录的学生</param>
        /// <param name="Money">金额</param>
        /// <param name="Remarks">附加说明</param>
        private void Record(string RecordType, Student Student = null, double? Money = null, string Remarks = null)
        {
            Record record = new Record
            {
                Timestamp = DateTime.Now,
                RecordType = RecordType,
                Remarks = Remarks,
                Money = Money
            };
            if (Student != null)
            {
                record.StudentID = Student.StudentID;
                record.CardTypeID = Student.CardTypeID;
                record.Name = Student.Name;
                record.Balance = Student.Balance;
                record.Apple = Student.Apple;

            }
            using (var db = new RecordContext())
            {
                db.Records.Add(record);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 增加一条系统记录
        /// </summary>
        /// <param name="RecordType">记录类型</param>
        /// <param name="Remarks">附加说明</param>
        public void Record(string RecordType, string Remarks = null)
        {
            Record record = new Record
            {
                Timestamp = DateTime.Now,
                RecordType = RecordType,
                Remarks = Remarks
            };
            using (var db = new RecordContext())
            {
                db.Records.Add(record);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 修改记录中的学生ID号
        /// </summary>
        /// <param name="StudentID">原卡号</param>
        /// <param name="NewStudentID">新卡号</param>
        public void RecordChangeStudentID(long StudentID,long NewStudentID)
        {
            using (var db = new RecordContext())
            {
                var query = from r in db.Records
                            where r.StudentID == StudentID
                            select r;
                foreach(var r in query)
                {
                    r.StudentID = NewStudentID;
                }
                db.SaveChanges();
            }
        }
        #endregion
    }
}