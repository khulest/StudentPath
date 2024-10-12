using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using StudentPath.Models;

namespace StudentPath.DB
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Student> students { get; set; }
        public DbSet<ClassAttendence> classAttendence { get; set; }
        public DbSet<BusAttendence> busAttendence { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<ClassNotification> classNotifications { get; set; }

    }
}
