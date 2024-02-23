using Aspire.ApiService.Services;
using Aspire.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOptions();

builder.Services.AddLogging();

builder.Services.AddHttpContextAccessor();
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapHub<MatchmakingHub>("/matchmaking");
app.MapHub<CsgoGameHub>("/csgo");
app.MapHub<VideoHub>("/video");

app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapGet("api/leaderboard", async (ModelDbContext context) =>
    Results.Ok((object?)await context.LeaderBoards.OrderByDescending(l => l.SkippedOthers).ToListAsync()));

app.MapGet("api/ongoingchad", async (ModelDbContext context) =>
    Results.Ok((object?)await context.OngoingChads.ToListAsync()));

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ModelDbContext>();

await context.OngoingChads.ExecuteDeleteAsync();
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    
    
    context.Database.EnsureCreated();
}
else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Run();