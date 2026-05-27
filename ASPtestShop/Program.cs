using ASPtestShop.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<Hshop2023Context>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("Hshop"));
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
        var context = services.GetRequiredService<Hshop2023Context>(); 
        
        if (context.Database.CanConnect())
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("[SUCCESS] KET NOI TOI SQL SERVER (Hshop) OK!");
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