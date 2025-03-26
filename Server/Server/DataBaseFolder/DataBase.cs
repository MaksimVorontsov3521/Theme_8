using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.DbContexts;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Server.DataBaseFolder
{
    internal class DataBase : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<UserTable> UserTable { get; set; }
        public DbSet<RoleTable> RoleTable { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<LogTable> Logs { get; set; }
        public DbSet<LogAction> LogAction { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<RequiredInPattern> RequiredInPatterns { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string DefaultConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            // Укажите строку подключения к своей базе данных здесь
            optionsBuilder.UseSqlServer(DefaultConnection);
        }
    }
}
