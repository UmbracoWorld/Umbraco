﻿using Service.Showcase.Presentation.Endpoints;
using Serilog;
using Service.Showcase.Presentation.Extensions;


var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureBuilder();

var app = builder
    .Build()
    .ConfigureApplication();

_ = app.MapEndpoints();

try
{
    Log.Information("Starting host");
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}