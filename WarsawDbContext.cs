using Microsoft.EntityFrameworkCore;
using WarsawCore.Models;

namespace WarsawCore
{
    public class WarsawDbContext : DbContext
    {
        public WarsawDbContext(DbContextOptions<WarsawDbContext> options): base(options)
        {
            
        }

        public DbSet<Stop> Stops { get; set; }
        
    }
}