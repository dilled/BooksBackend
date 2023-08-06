using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BooksBackendTests
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IConfiguration? Configuration { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config => 
            { 
                Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
                config.AddConfiguration(Configuration);
            });
            builder.ConfigureServices(services =>
            {
                //    Remove the original registration of AppDbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BackendDeveloperTask.Data.AppDbContext>)
                );

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                //    Register the DbContextFactory for TestAppDbContext

                services.AddDbContextFactory<BackendDeveloperTask.Data.AppDbContext> (options =>
                    options.UseSqlite(Configuration.GetConnectionString("SQLiteConnectionString")));
            });
        }
    }
}
