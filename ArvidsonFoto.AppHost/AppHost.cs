var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server resource for development
var sqlServer = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)  // Persist data across runs
    .AddDatabase("ArvidsonFotoDb");

// Add the main ArvidsonFoto web application
var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithReference(sqlServer)
    .WithExternalHttpEndpoints();

// Optionally add the LogReader application
// var logReader = builder.AddProject<Projects.ArvidsonFoto_LogReader>("logreader")
//     .WithReference(arvidsonFoto);

builder.Build().Run();
