using FastReport.Web;
using Microsoft.EntityFrameworkCore;
using Report_View.Data;
using FastReport.Data;
using FastReport.Utils;

var builder = WebApplication.CreateBuilder(args);

// Register PostgreSQL connection
RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddFastReport(); // เพิ่ม FastReport service

// Add DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// เพิ่ม FastReport middleware
app.UseFastReport();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
