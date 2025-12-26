var builder = DistributedApplication.CreateBuilder(args);

// ArvidsonFoto already uses in-memory database in development (appsettings.Development.json)
// No need for SQL Server container when UseInMemoryDatabase is true

// Add the main ArvidsonFoto web application (public-facing website)
var arvidsonFoto = builder
    .AddProject<Projects.ArvidsonFoto>("arvidsonfoto", launchProfileName: "ArvidsonFoto")
    .WithExternalHttpEndpoints();

// Add a second instance for API documentation and Admin panel
// Uses the 'arvidsonfoto-dev-portal' launch profile from launchSettings.json
// which defines https://localhost:5011 and launchUrl /dev
var arvidsonFotoDevPortal = builder
    .AddProject<Projects.ArvidsonFoto>("arvidsonfoto-dev-and-api-portal", launchProfileName: "ArvidsonFoto-dev-portal")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

// Optionally add the LogReader application
// var logReader = builder.AddProject<Projects.ArvidsonFoto_LogReader>("logreader")
//     .WithReference(arvidsonFoto);

builder.Build().Run();
