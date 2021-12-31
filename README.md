# Logging in ASP.NET Core
There are multiple ways to log in ASP.NET Core, as stated on the official [docs](https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/fundamentals/logging/index.md "docs") there are several options out of the box and other third party options.

While I was developing my web application I need to write logs into a SQL database table and I was determined to use one of the well known third party options, until I read the documentation carefully and some of the implementations, as stated in the [documents](https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/fundamentals/logging/index.md#no-asynchronous-logger-methods "documents") about writing to slower stores and by doing more research came to this github [issue](https://github.com/dotnet/AspNetCore.Docs/issues/11801 "issue") where some guidance was provided on ways to implement a database logger

I have created a way to log messages with the implementation of the [ILogger](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger?view=dotnet-plat-ext-6.0 "ILogger") interface fully that has an implementation to write into any SQL implementation of [IDbConnection](https://docs.microsoft.com/en-us/dotnet/api/system.data.idbconnection?view=net-6.0 "IDbConnection")

## Configuration
```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
                webBuilder.ConfigureAppConfiguration(config =>
                {
                      var settings = config.Build();
                }).UseStartup<Startup>();
          }).ConfigureLogging(builder =>
               {
                    builder.ClearProviders();
                    builder.AddConsole();
				   // add the implementation of the custom logger
                    builder.AddEventHandler();
                });
```
## Startup
```csharp
//Add the implementation of the logging store, in this case MSSQL
services.AddSingleton<ILoggingStore>(new MsSqlLogger(connStr));
//Sets the options for the IHostedService that would queue the log messages before the store
//persits them into the database
services.AddSingleton(new WorkerOptions() { Interval = TimeSpan.FromMinutes(1) });
//Initializes the IHostedService
services.AddHostedService<EventHandlerLoggerWorker>();
```
