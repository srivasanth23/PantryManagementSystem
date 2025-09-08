using Microsoft.EntityFrameworkCore;
using PantryManagementSystem.Data;
using PantryManagementSystem.Repositories;
using PantryManagementSystem.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// For swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Registering Database
builder.Services.AddDbContext<PantryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbString")));



// Register Repositories
builder.Services.AddScoped<IPantryItemRepository, PantryItemRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable Swagger for all enviroments
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthorization();
app.MapControllers();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
