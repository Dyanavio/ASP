using ASP.Data;
using ASP.Services.Identity;
using ASP.Services.Kdf;
using ASP.Services.Random;
using ASP.Services.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ASP
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IRandomService, DefaultRandomService>();
            builder.Services.AddSingleton<ITimeService, SecTimeService>();
            builder.Services.AddSingleton<IIdentityService, IdentityService>();
            builder.Services.AddSingleton<IKdfService, PbKdfService>();
            //builder.Services.AddTransient<ITimeService, MillisecTimeService>(); // One time object. Will be different in controller and Razor
            //builder.Services.AddScoped<ITimeService, MillisecTimeService>(); // - Constant inside of a signle request. Reloading the page created new objects, but they will not change

            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb")));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

           
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            using (var scope = app.Services.CreateScope()) 
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>(); 
                await db.Database.MigrateAsync(); 
            }


            app.Run();
        }
    }
}
