using Airline.Web.Services;
using Airline.Web.Data;        
using Airline.Web.Models;       
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen; 
using Swashbuckle.AspNetCore.SwaggerUI;  

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к Базе Данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AirlineDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Настройка Identity (Авторизация)
builder.Services.AddDefaultIdentity<AirlineUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AirlineDbContext>();

// 3. Контроллеры и Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 4. Swagger (API документация)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFlightService, FlightService>();
var app = builder.Build();

// 5. Настройка среды выполнения
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Важно для авторизации
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    await DbSeeder.InitializeAsync(app.Services);
}

app.Run();