using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .AddDatabase("sqldata");

var apiService =
    builder.AddProject<Projects.Aspire_ApiService>("apiservice")
        .WithReference(sql);

builder.Build().Run();