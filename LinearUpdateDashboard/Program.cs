// <copyright file="MarketsController.cs" company="Comcast">
// Copyright (c) Comcast. All Rights Reserved.
// </copyright>

using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

    var serilogConfigSection = builder.Configuration.GetSection("Serilog");
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            connectionString: serilogConfigSection.GetSection("ConnectionStrings:LogSQLDatabase").Value,
            tableName: serilogConfigSection.GetSection("TableName").Value,
            appConfiguration: serilogConfigSection,
            autoCreateSqlTable: true,
            columnOptionsSection: serilogConfigSection.GetSection("ColumnOptions"),
            schemaName: serilogConfigSection.GetSection("SchemaName").Value));

    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<LinearDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("LinearSQLDatabase")));

    builder.Services.AddScoped<DbContext, LinearDbContext>();

    var spotStatusConfigSection = builder.Configuration.GetSection("SpotStatusCheckerDirPaths");
    var spotStatusConfig = spotStatusConfigSection.Get<SpotStatusDirectoryConfiguration>();
    builder.Services.Configure<SpotStatusDirectoryConfiguration>(spotStatusConfigSection);

    builder.Services.AddRazorPages()
        .AddRazorRuntimeCompilation();

    builder.Services.AddApplicationInsightsTelemetry();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Market/Error");

        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Markets}/{action=Index}/{name?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception has occurred");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}