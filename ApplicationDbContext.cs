using System;
using Microsoft.EntityFrameworkCore;
using YMCAProject.Models;

namespace YMCAProject;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Staff> Staff { get; set; }

}
