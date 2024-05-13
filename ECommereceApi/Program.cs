using ECommereceApi.Data;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Repo;
using ECommereceApi.Services.classes;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("myPolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ECommerceContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddInterceptors(new SoftDeleteInterceptor());
});

builder.Services.AddAutoMapper(typeof(Program));

#region FileServer
var cloudinaryCredentials = builder.Configuration.GetSection("Cloudinary");
var account = new Account(
    cloudinaryCredentials["CloudName"],
    cloudinaryCredentials["APIKey"],
    cloudinaryCredentials["APISecret"]
    );
builder.Services.AddSingleton(account);
builder.Services.AddScoped<Cloudinary>();
#endregion


builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICartRepo, CartRepo>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("myPolicy");
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
