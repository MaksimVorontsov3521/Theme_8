using Microsoft.EntityFrameworkCore;
using Server.DataBase.DbContexts;
using Server.DataBase.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBase
{
    internal class DataBase : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<User> UserTable { get; set; }
        public DbSet<Role> RoleTable { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogAction> LogActions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<RequiredInPattern> RequiredInPatterns { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальный логин пользователя
            modelBuilder.Entity<User>()
                .HasIndex(u => u.LoginUser)
                .IsUnique();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Укажите строку подключения к своей базе данных здесь
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-GE8PRE7;Initial Catalog=T8;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
        }
    }
}
