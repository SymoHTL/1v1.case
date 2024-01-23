using Microsoft.EntityFrameworkCore;

namespace Aspire.ApiService.Configuration;

public class ModelDbContext : DbContext {

    public DbSet<Player> Players { get; set; } = null!;
    public ModelDbContext(DbContextOptions<ModelDbContext> options) : base(options) { }
}