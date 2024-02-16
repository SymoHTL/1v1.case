using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Aspire_ApiService>("apiservice");

builder.Build().Run();