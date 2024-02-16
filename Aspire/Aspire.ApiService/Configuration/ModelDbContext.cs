namespace Aspire.ApiService.Configuration;

public class ModelDbContext : DbContext {
    public DbSet<LeaderBoard> LeaderBoards { get; set; }
    public DbSet<OngoingChad> OngoingChads { get; set; }
    public DbSet<Player> Players { get; set; } = null!;
    public ModelDbContext(DbContextOptions<ModelDbContext> options) : base(options) { }
}