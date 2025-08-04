using Final_project.Filter;
using Final_project.Hubs;
using Final_project.MapperConfig;
using Final_project.Models;
using Final_project.Repository;
using Final_project.Services.Background;
using Final_project.Services.CustomerService;
using Final_project.Services.DeviceService;
using Final_project.Services.EmailService;
using Final_project.Services.SmsService;
using Final_project.Services.TwoFactorService;
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
            builder.Services.AddScoped<ICustomerServiceService, CustomerServiceService>();
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

                // Email confirmation settings
                option.SignIn.RequireConfirmedEmail = true;
                option.User.RequireUniqueEmail = true;

                // Token settings
                option.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                option.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
                option.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;

            }).AddEntityFrameworkStores<AmazonDBContext>()
              .AddDefaultTokenProviders(); // THIS IS THE KEY FIX!

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
            //=================Email Virification======================

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            // Configure token lifespan (optional)
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24); // Email confirmation tokens expire in 24 hours
            });

            //=================End Email Virification=================

            //================= 2fv Two Factor Verification () ======================

            builder.Services.AddScoped<ISmsService, EgyptSmsService>();
            builder.Services.AddScoped<IDeviceService,EnhancedDeviceService>();
            builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

            // Add background service for cleaning expired codes
            builder.Services.AddHostedService <ExpiredCodesCleanupService>();
            //ExpiredCodesCleanupService
            //=================End Email Virification=================

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Add this for serving static files
            app.UseRouting();
            app.UseSession();

            // CRITICAL: Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapHub<CustomerServiceHub>("/customerServiceHub");
            app.MapHub<SellerOrdersHub>("/sellerOrdersHub");
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