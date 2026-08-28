using Microsoft.Extensions.Configuration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Load configuration from the ArvidsonFoto project directory
string appHostBasePath = builder.Environment.ContentRootPath; // AppHost project directory
string arvidsonFotoPath = Path.GetFullPath(Path.Combine(appHostBasePath, "..", "ArvidsonFoto"));

// Build a configuration that reads from the ArvidsonFoto project directory
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(arvidsonFotoPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

builder.Configuration.AddConfiguration(configuration);

// Add parameters from the ArvidsonFoto configuration
Aspire.Hosting.ApplicationModel.IResourceBuilder<Aspire.Hosting.ApplicationModel.ParameterResource> smtpServer = builder.AddParameterFromConfiguration(
    "smtpServer",
    "SmtpSettings:Server");

Aspire.Hosting.ApplicationModel.IResourceBuilder<Aspire.Hosting.ApplicationModel.ParameterResource> smtpSenderEmail = builder.AddParameterFromConfiguration(
    "smtpSenderEmail",
    "SmtpSettings:SenderEmail");

Aspire.Hosting.ApplicationModel.IResourceBuilder<Aspire.Hosting.ApplicationModel.ParameterResource> smtpRecipientEmail = builder.AddParameterFromConfiguration(
    "smtpRecipientEmail",
    "SmtpSettings:RecipientEmail");

Aspire.Hosting.ApplicationModel.IResourceBuilder<Aspire.Hosting.ApplicationModel.ParameterResource> databaseInMemory = builder.AddParameterFromConfiguration(
    "databaseInMemory",
    "ConnectionStrings:UseInMemoryDatabase");
bool useInMemoryDatabase = builder.Configuration.GetValue<bool>(
    "ConnectionStrings:UseInMemoryDatabase");


// Add the main ArvidsonFoto web application (public-facing website)
Aspire.Hosting.ApplicationModel.IResourceBuilder<Aspire.Hosting.ApplicationModel.ProjectResource> arvidsonFoto = builder
    .AddProject<Projects.ArvidsonFoto>("arvidsonfoto", launchProfileName: "ArvidsonFoto")
    .WithEnvironment("SmtpSettings__Server", smtpServer)
    .WithEnvironment("SmtpSettings__SenderEmail", smtpSenderEmail)
    .WithEnvironment("SmtpSettings__RecipientEmail", smtpRecipientEmail)
    .WithEnvironment("ConnectionStrings__UseInMemoryDatabase", databaseInMemory)
    .WithExternalHttpEndpoints();

// Only add the SQL Server connection string if not using in-memory database
if (!useInMemoryDatabase)
{
    Aspire.Hosting.ApplicationModel.IResourceBuilder<Aspire.Hosting.ApplicationModel.ParameterResource> databaseConnectionString = builder.AddParameterFromConfiguration(
    "databaseConnectionString",
    "ConnectionStrings:DefaultConnection");

    arvidsonFoto
        .WithEnvironment("ConnectionStrings__DefaultConnection", databaseConnectionString);
}

// Add a second instance for API documentation and Admin panel
// Uses the 'arvidsonfoto-dev-portal' launch profile from launchSettings.json
// which defines https://localhost:5011 and launchUrl /dev
builder
    .AddProject<Projects.ArvidsonFoto>("arvidsonfoto-dev-and-api-portal", launchProfileName: "ArvidsonFoto-dev-portal")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder.Build().Run();