using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Status_200.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status_200
{
    public class AppDbContext:DbContext
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StatusTask> StatusesTask { get; set; }
        public DbSet<UserStatus> UsersStatus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("json1.json")
                .Build();


            string? connectionString = config
                .GetConnectionString("ConnectionString");

            optionsBuilder
                .UseSqlServer(connectionString);
        }
    }
}
