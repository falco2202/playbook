var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Presentation>("api");

builder.Build().Run();
