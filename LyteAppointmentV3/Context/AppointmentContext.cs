using LyteAppointmentV3.Model;
using Microsoft.EntityFrameworkCore;

namespace LyteAppointmentV3.Context
{
    public class AppointmentContext : DbContext
    {
        public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options) { }

        public DbSet<AppointmentModel> Appointments { get; set; }
    }
}
