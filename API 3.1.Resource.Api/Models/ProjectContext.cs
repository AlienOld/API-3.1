using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<T_sensor> T_sensors { get; set; }
        public DbSet<H_sensor> H_sensors { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
