using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using SQLite.CodeFirst;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Student
    {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long StudentID { get; set; }
        public int? CardID { get; set; }
        public int CardTypeID { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int Apple { get; set; }
        public string PhoneNumber { get; set; }
        public char? Sex { get; set; }
        public int? Age { get; set; }
        public DateTime? Birthday { get; set; }
        public string School { get; set; }
        public double TotalMoney { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Remarks { get; set; }

        public virtual CardType CardType { get; set; }

        public override string ToString()//Debug
        {
            return $"{Name}[{StudentID}]:{CardID}{{{Balance},{Apple}}}{LastUpdated}";
        }
    }

    public class Setting//TODO 未完成
    {
        public int ID { get; set; }
        public int C { get; set; }
        public string N { get; set; }
        public double B { get; set; }
        public int A { get; set; }

        public DateTime TimeSpan { get; set; }
    }
    public class CardType
    {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int CardTypeID { get; set; }
        public double MonthlyFee { get; set; }
        public int MonthlyClass { get; set; }
        public double CostPerLesson { get; set; }
        public string TypeName { get; set; }
    }

    public class StudentDBInitializer : SqliteCreateDatabaseIfNotExists<StudentContext>
    {
        public StudentDBInitializer(DbModelBuilder modelBuilder) : base(modelBuilder) { }
        protected override void Seed(StudentContext context) { }
    }
    public class StudentContext : DbContext
    {
        public StudentContext() : base("name=StudentContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new StudentDBInitializer(modelBuilder));
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
    }
}
