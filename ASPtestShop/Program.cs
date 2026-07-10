using ASPtestShop.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using ASPtestShop.Data;
using ASPtestShop.Services.Implementations;
using ASPtestShop.Services.Implementations.Admin;
using ASPtestShop.Services.Implementations.PaymentProviders;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.Admin;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Thêm namespace cho JWT Bearer Authentication
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;                // Thêm namespace cho Token Validation
using System.Security.Claims;
using System.Text;

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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // Kiểm tra security_stamp trong JWT có khớp với user hiện tại trong DB không
    // Nếu logout/reset password đã đổi SecurityStamp thì token cũ sẽ bị từ chối
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var tokenSecurityStamp = context.Principal?.FindFirstValue("security_stamp");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenSecurityStamp))
            {
                context.Fail("Token không hợp lệ");
                return;
            }

            var userManager = context.HttpContext.RequestServices
                .GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                context.Fail("User không tồn tại");
                return;
            }

            var currentSecurityStamp = await userManager.GetSecurityStampAsync(user);

            if (tokenSecurityStamp != currentSecurityStamp)
            {
                context.Fail("Token đã hết hiệu lực");
                return;
            }
        }
    };
})
.AddCookie(AdminCookieAuth.Scheme, options =>
{
    options.Cookie.Name = "HShop.Admin.Auth";
    options.LoginPath = "/admin/login";
    options.AccessDeniedPath = "/admin/access-denied";

    options.ExpireTimeSpan = TimeSpan.FromHours(6);
    options.SlidingExpiration = true;

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
//Đăng kí services cho các lớp dịch vụ
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//admin services
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<IAdminUploadService, AdminUploadService>();
builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();

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
app.UseStaticFiles();
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

// =========================================================================
// SEED ROLE ADMIN / CUSTOMER
// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Customer" };

    foreach (var role in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"[SEED ROLE] Created role: {role}");
        }
    }

    // Email tài khoản admin của bạn
    var adminEmail = "admin@gmail.com";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null)
    {
        var isAdmin = await userManager.IsInRoleAsync(adminUser, "Admin");

        if (!isAdmin)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine($"[SEED ROLE] Added {adminEmail} to Admin role");
        }
    }
    else
    {
        Console.WriteLine($"[SEED ROLE] Admin user {adminEmail} not found. Register this account first.");
    }
}
// =========================================================================
app.Run();