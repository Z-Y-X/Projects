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
    public class Record
    {
        [Key] public int RecordID { get; set; }
        public DateTime Timestamp { get; set; }
        public long? StudentID { get; set; }
        public int? CardTypeID { get; set; }
        public string RecordType { get; set; }
        public string Name { get; set; }
        public double? Money { get; set; }
        public double? Balance { get; set; }
        public int? Apple { get; set; }
        public string Remarks { get; set; }
    }
    public class RecordDBInitializer : SqliteCreateDatabaseIfNotExists<RecordContext>
    {
        public RecordDBInitializer(DbModelBuilder modelBuilder) : base(modelBuilder) { }
        protected override void Seed(RecordContext context) { }
    }

    public class RecordContext : DbContext
    {
        public RecordContext() : base("name=RecordContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new RecordDBInitializer(modelBuilder));
        }

        public DbSet<Record> Records { get; set; }
    }
}
