using ASPtestShop.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Thêm namespace cho JWT Bearer Authentication
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;                // Thêm namespace cho Token Validation
using System.Text;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Implementations;
using ASPtestShop.Services.Implementations.PaymentProviders;
using ASPtestShop.Services.PaymentProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hshop"));
});
// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
// Cấu hình JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
//Đăng kí services cho các lớp dịch vụ
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Payment providers
builder.Services.AddScoped<IPaymentProvider, CodPaymentProvider>();
// tạm thời chưa dùng
//builder.Services.AddScoped<IPaymentProvider, MomoPaymentProvider>();
//builder.Services.AddScoped<IPaymentProvider, ZaloPayPaymentProvider>();
//builder.Services.AddScoped<IPaymentProvider, VnPayPaymentProvider>();
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// =========================================================================
// ĐOẠN CODE TEST KẾT NỐI DATABASE (THÊM VÀO ĐÂY)

// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Gọi DbContext ra để kiểm tra
        var context = services.GetRequiredService<AppDbContext>();

        if (context.Database.CanConnect())
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("[SUCCESS] KET NOI TOI SQL SERVER (Hshop) OK!");
            Console.WriteLine("====================XIN CHÀO==================\n");

            Console.WriteLine("==================================================\n");
        }
        else
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("[ERROR] KHONG THE KET NOI TOI DATABASE HSHOP!");
            Console.WriteLine("Vui long kiem tra lai Connection String hoac Service SQL!");
            Console.WriteLine("==================================================\n");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("\n==================================================");
        Console.WriteLine($"[FATAL] LOI KHI KET NOI DB: {ex.Message}");
        Console.WriteLine("==================================================\n");
    }
}
// =========================================================================

app.Run();