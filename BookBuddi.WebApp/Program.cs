using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookBuddi.Data;
using BookBuddi.Data.Models;
using BookBuddi.Data.Interfaces;
using BookBuddi.Data.Repositories;
using BookBuddi.Services.Interfaces;
using BookBuddi.Services.Services;
using BookBuddi.Services.Manager;
using BookBuddi.Services;
using BookBuddi.Services.Configuration;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Configure Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// Identity
builder.Services.AddIdentity<Admin, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Admin/AccessDenied";
    options.SlidingExpiration = true;
});

// Add AutoMapper
var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add HttpContextAccessor for SessionManager
builder.Services.AddHttpContextAccessor();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IBorrowTransactionRepository, BorrowTransactionRepository>();
builder.Services.AddScoped<IFineRepository, FineRepository>();
builder.Services.AddScoped<IBookRequestRepository, BookRequestRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

// Register Managers
builder.Services.AddScoped<PasswordManager>();
builder.Services.AddScoped<SessionManager>();

// Register Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IBorrowingService, BorrowingService>();
builder.Services.AddScoped<IFineService, FineService>();
builder.Services.AddScoped<IBookRequestService, BookRequestService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IRatingService, RatingService>();

// Register Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailNotificationBackgroundService>();

// Register File Upload Service
builder.Services.AddScoped<IFileUploadService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new FileUploadService(env.WebRootPath);
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<Admin>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var passwordManager = services.GetRequiredService<PasswordManager>();

        await DbInitializer.Initialize(context, userManager, roleManager, passwordManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Use Hangfire Dashboard (accessible only in Development or for admins)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
});

// Schedule recurring background jobs
RecurringJob.AddOrUpdate<EmailNotificationBackgroundService>(
    "send-due-reminders",
    service => service.SendDueRemindersAsync(),
    Cron.Daily(9)); // Run daily at 9 AM

RecurringJob.AddOrUpdate<EmailNotificationBackgroundService>(
    "send-overdue-alerts",
    service => service.SendOverdueAlertsAsync(),
    Cron.Daily(10)); // Run daily at 10 AM

RecurringJob.AddOrUpdate<EmailNotificationBackgroundService>(
    "send-membership-expiry-reminders",
    service => service.SendMembershipExpiryRemindersAsync(),
    Cron.Daily(8)); // Run daily at 8 AM

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();

// Hangfire Dashboard Authorization Filter
public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In development, allow access to everyone
        // In production, you should check if the user is an admin
        var httpContext = context.GetHttpContext();
        return httpContext.Request.Host.Host.Contains("localhost") ||
               httpContext.Session.GetString("UserRole") == "Admin";
    }
}
