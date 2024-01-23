using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Aspire_ApiService>("apiservice");

builder.AddProject<Aspire_Web>("webfrontend").WithReference(apiService);

builder.Build().Run();