using Aspire.ApiService.Configuration;
using Aspire.ApiService.Services;
using Aspire.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddDbContextFactory<ModelDbContext>(options =>
    options.UseInMemoryDatabase("Aspire.PlayerDatabase"));

builder.Services.AddSignalR();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddScoped(c => new HttpClient() {
    BaseAddress = new Uri(builder.Configuration["csgo_url"] ?? throw new Exception("csgo_url not found"))
});
builder.Services.AddScoped<CsgoService>();

builder.Services.AddCors();

var app = builder.Build();

app.MapHub<MatchmakingHub>("/matchmaking");
app.MapHub<CsgoGameHub>("/csgo");
app.MapHub<GameHub>("/3dgame");

app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.Run();