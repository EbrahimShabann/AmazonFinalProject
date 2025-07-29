using Final_project.Filter;
using Final_project.MapperConfig;
using Final_project.Models;
using Final_project.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Threading.Tasks;

namespace Final_project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //==================SignalR Services=========================
            builder.Services.AddSignalR();
            //==================Filter Handel Exiptions==================
            //===========Remove comment Whern Deploying==================
            //builder.Services.AddControllersWithViews();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new HandelAnyErrorAttribute());
            });

            //Stripe payment
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

            //==================HttpClient====================
            builder.Services.AddHttpClient();
            //==================SessionnConfiguration====================
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //FOR GOOGLE ALSO 
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
            });

            //======================Injection============================
            builder.Services.AddScoped<UnitOfWork>();
            //======================SQLInjection=========================

            builder.Services.AddDbContext<AmazonDBContext>(
                options => options
                .UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //====================UserManagerInjection===================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredLength = 4;
                option.Password.RequireUppercase = false;

            }).AddEntityFrameworkStores<AmazonDBContext>();
            //======================EndInjection=========================

            //======================Automapper===========================
            //builder.Services.AddAutoMapper(typeof(mapperConfig));
            //=================Google Authentication=====================

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });
            //======================EndBuilder=========================

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();
            app.MapStaticAssets();
            //app.MapHub<>("");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Landing}/{action=Index}/{id?}")
                .WithStaticAssets();


            using (var scope = app.Services.CreateScope())
            {
                await DbSeeder.SeedDefaultData(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}