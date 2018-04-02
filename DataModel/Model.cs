using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class DataContext : DbContext {
        public DbSet<ListItem> Items { get; set; }
        public DbSet<TempState> State { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=list.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // 约束
            modelBuilder.Entity<ListItem>().Property(m => m.Title).IsRequired();
        }
    }

    public class ListItem {
        [Key]
        public int ListId { get; set; }
        public string Title { get; set; }
        public string Des { get; set; }
        public Boolean IsCheck { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public byte[] Icon { get; set; }
        public void Update(ListItem newDate) {
            Title = newDate.Title;
            Des = newDate.Des;
            IsCheck = newDate.IsCheck;
            DueDate = newDate.DueDate;
            Icon = newDate.Icon;
        }
    }
    public class TempState {
        [Key]
        public int ListId { get; set; }
        public string Title { get; set; }
        public string Des { get; set; }
        public int ListIndex { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public byte[] Icon { get; set; }

    }
}
