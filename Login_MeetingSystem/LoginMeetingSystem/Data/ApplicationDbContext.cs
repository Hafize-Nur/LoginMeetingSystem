using Microsoft.EntityFrameworkCore;
using LoginMeetingSystem.Models;

namespace LoginMeetingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingDocument> MeetingDocuments { get; set; }
    }

}