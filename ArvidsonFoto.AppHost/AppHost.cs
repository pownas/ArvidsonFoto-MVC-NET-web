using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Load configuration from the ArvidsonFoto project directory
var appHostBasePath = builder.Environment.ContentRootPath; // AppHost project directory
var arvidsonFotoPath = Path.GetFullPath(Path.Combine(appHostBasePath, "..", "ArvidsonFoto"));

// Build a configuration that reads from the ArvidsonFoto project directory
var configuration = new ConfigurationBuilder()
    .SetBasePath(arvidsonFotoPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

builder.Configuration.AddConfiguration(configuration);

// Add parameters from the ArvidsonFoto configuration
var smtpServer = builder.AddParameterFromConfiguration(
    "smtpServer",
    "SmtpSettings:Server");

var smtpSenderEmail = builder.AddParameterFromConfiguration(
    "smtpSenderEmail",
    "SmtpSettings:SenderEmail");

var smtpRecipientEmail = builder.AddParameterFromConfiguration(
    "smtpRecipientEmail",
    "SmtpSettings:RecipientEmail");

var databaseInMemory = builder.AddParameterFromConfiguration(
    "databaseInMemory",
    "ConnectionStrings:UseInMemoryDatabase");
var useInMemoryDatabase = builder.Configuration.GetValue<bool>(
    "ConnectionStrings:UseInMemoryDatabase");


// Add the main ArvidsonFoto web application (public-facing website)
var arvidsonFoto = builder
    .AddProject<Projects.ArvidsonFoto>("arvidsonfoto", launchProfileName: "ArvidsonFoto")
    .WithEnvironment("SmtpSettings__Server", smtpServer)
    .WithEnvironment("SmtpSettings__SenderEmail", smtpSenderEmail)
    .WithEnvironment("SmtpSettings__RecipientEmail", smtpRecipientEmail)
    .WithEnvironment("ConnectionStrings__UseInMemoryDatabase", databaseInMemory)
    .WithExternalHttpEndpoints();

// Only add the SQL Server connection string if not using in-memory database
if (useInMemoryDatabase.Equals(false))
{
    var databaseConnectionString = builder.AddParameterFromConfiguration(
    "databaseConnectionString",
    "ConnectionStrings:DefaultConnection");

    arvidsonFoto
        .WithEnvironment("ConnectionStrings__DefaultConnection", databaseConnectionString);
}

// Add a second instance for API documentation and Admin panel
// Uses the 'arvidsonfoto-dev-portal' launch profile from launchSettings.json
// which defines https://localhost:5011 and launchUrl /dev
var arvidsonFotoDevPortal = builder
    .AddProject<Projects.ArvidsonFoto>("arvidsonfoto-dev-and-api-portal", launchProfileName: "ArvidsonFoto-dev-portal")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.Build().Run();
