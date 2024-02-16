using Aspire.ApiService.Services;
using Aspire.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.AddSqlServerDbContext<ModelDbContext>("sqldata");

builder.Services.AddSignalR(o => {
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 1024 * 1024 * 100;
    o.MaximumParallelInvocationsPerClient = 1000;
});
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.AddScoped(c => new HttpClient() {
    BaseAddress = new Uri(builder.Configuration["csgo_url"] ?? throw new Exception("csgo_url not found"))
});

builder.Services.AddCors();

var app = builder.Build();

app.MapHub<MatchmakingHub>("/matchmaking");
app.MapHub<CsgoGameHub>("/csgo");
app.MapHub<VideoHub>("/video");

app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapGet("leaderboard", async (ModelDbContext context) =>
    Results.Ok((object?)await context.LeaderBoards.ToListAsync()));

app.MapGet("ongoingchad", async (ModelDbContext context) =>
    Results.Ok((object?)await context.OngoingChads.ToListAsync()));

if (app.Environment.IsDevelopment()) {
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ModelDbContext>();
    context.Database.EnsureCreated();
}
else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Run();