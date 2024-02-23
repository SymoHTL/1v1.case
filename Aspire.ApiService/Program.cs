using Aspire.ApiService.Services;
using Aspire.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOptions();

builder.Services.AddLogging();

builder.Services.AddHttpContextAccessor();

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ModelDbContext>("sqldata");

builder.Services.AddProblemDetails();


builder.Services.AddSignalR(o => {
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 1024 * 1024 * 100;
    o.MaximumParallelInvocationsPerClient = 1000;
});


builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapHub<VideoHub>("/video");

app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseExceptionHandler();

app.MapDefaultEndpoints();


using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ModelDbContext>();

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


public class YourDbContext(DbContextOptions<YourDbContext> options) : DbContext(options) {
}