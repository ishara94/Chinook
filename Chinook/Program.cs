using Chinook;
using Chinook.Areas.Identity;
using Chinook.Models;
using Chinook.Services.Data;
using Chinook.Services.Data.Interfaces;
using Chinook.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;


CreateHostBuilder(args).Build().Run();

 static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
