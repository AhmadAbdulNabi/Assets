using Assets.DataAccess;
using System.Data;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Register database connection
builder.Services.AddTransient<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register data access services
builder.Services.AddTransient<DBHelper>();  // ✅ هذا السطر مهم
builder.Services.AddTransient<AssetRepository>();
builder.Services.AddTransient<AssetAdditionRepository>();
builder.Services.AddTransient<JournalEntryRepository>();
builder.Services.AddTransient<AccountRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.MapDefaultControllerRoute();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
