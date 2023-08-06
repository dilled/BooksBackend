﻿using BackendDeveloperTask.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendDeveloperTask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> books { get; set; }
    }
}
