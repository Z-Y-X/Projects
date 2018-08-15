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
        [Key] [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)] public int ID { get; set; }
        public int Card { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int Apple { get; set; }

        public DateTime TimeSpan { get; set; }
        public string Info { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public override string ToString()
        {
            return $"{Name}[{ID}]:{Card}{{{Balance},{Apple}}}{TimeSpan}";
        }
    }
    public class DBInitializer : SqliteCreateDatabaseIfNotExists<StudentContext>
    {
        public DBInitializer(DbModelBuilder modelBuilder) : base(modelBuilder) { }
        protected override void Seed(StudentContext context) { }
    }
    public class StudentContext : DbContext
    {
        public StudentContext() : base("name=StudentContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DBInitializer(modelBuilder));
        }

        public DbSet<Student> Students { get; set; }
    }
}
