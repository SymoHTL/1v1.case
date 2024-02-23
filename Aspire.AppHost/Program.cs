var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServerContainer("sql")
    .WithVolumeMount("sqldata", "/var/opt/mssql/data")
    .AddDatabase("sqldata");

var apiService =
    builder.AddProject<Projects.Aspire_ApiService>("apiservice")
        .WithReference(sql);

var web = builder.AddProject<Projects.Aspire_Web>("web")
    .WithReference(apiService);

builder.Build().Run();