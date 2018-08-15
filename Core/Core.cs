using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public class Core
    {
        private Student student;

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
        public bool HasReward { get; private set; }

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
        /// 检查是否应该奖励（不判断null）
        /// </summary>
        /// <returns>应该奖励</returns>
        //private bool CheckReward()//TODO
        //{
        //    return student.Apple % 10 == 0;
        //}

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
    }
}