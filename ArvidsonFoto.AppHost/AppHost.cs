var builder = DistributedApplication.CreateBuilder(args);

// ArvidsonFoto already uses in-memory database in development (appsettings.Development.json)
// No need for SQL Server container when UseInMemoryDatabase is true

// Add the main ArvidsonFoto web application
var arvidsonFoto = builder.AddProject<Projects.ArvidsonFoto>("arvidsonfoto")
    .WithExternalHttpEndpoints();

// Optionally add the LogReader application
// var logReader = builder.AddProject<Projects.ArvidsonFoto_LogReader>("logreader")
//     .WithReference(arvidsonFoto);

builder.Build().Run();
