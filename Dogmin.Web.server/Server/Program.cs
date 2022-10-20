using Dogmin.Web.server.Server.Data;
using Dogmin.Web.server.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Dogmin.Web.server.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            _ = builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));
            _ = builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            _ = builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            _ = builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            _ = builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            _ = builder.Services.AddControllersWithViews();
            _ = builder.Services.AddRazorPages();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();

                using IServiceScope scope = app.Services.CreateScope();
                ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _ = dbContext.Database.EnsureCreated();
                // use context

            }
            else
            {
                _ = app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();
            _ = app.UseBlazorFrameworkFiles();
            _ = app.UseStaticFiles();
            _ = app.UseRouting();
            _ = app.UseIdentityServer();
            _ = app.UseAuthentication();
            _ = app.UseAuthorization();
            _ = app.MapRazorPages();
            _ = app.MapControllers();
            _ = app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}