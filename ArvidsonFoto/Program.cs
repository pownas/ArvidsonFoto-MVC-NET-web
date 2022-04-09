using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ArvidsonFoto;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //.WriteTo.Console() //Kräver nuget paketet: Serilog.Sinks.Console
            .WriteTo.File("logs\\appLog.txt", rollingInterval: RollingInterval.Day) //Bör kanske försöka byta till Serilog DB-loggning...
            .CreateLogger();

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}