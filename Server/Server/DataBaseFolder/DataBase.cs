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
        public DbSet<Client> Client { get; set; }
        public DbSet<UserTable> UserTable { get; set; }
        public DbSet<RoleTable> RoleTable { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<Folder> Folder { get; set; }
        public DbSet<LogTable> LogTable { get; set; }
        public DbSet<LogAction> LogAction { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Pattern> Pattern { get; set; }
        public DbSet<RequiredInPattern> RequiredInPattern { get; set; }
        public DbSet<DepartmentFolder> DepartmentFolder { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentFolder>()
    .HasKey(df => df.DepartmentFolderID);

            modelBuilder.Entity<DepartmentFolder>()
                .HasOne(df => df.Department)
                .WithMany(d => d.DepartmentFolders)
                .HasForeignKey(df => df.DepartmentID);

            modelBuilder.Entity<DepartmentFolder>()
                .HasOne(df => df.Folder)
                .WithMany(f => f.DepartmentFolders)
                .HasForeignKey(df => df.FolderID);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string DefaultConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            // Укажите строку подключения к своей базе данных здесь
            optionsBuilder.UseSqlServer(DefaultConnection);
        }
    }
}
