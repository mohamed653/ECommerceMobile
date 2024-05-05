using CloudinaryDotNet;
using ECommereceApi.Data;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Repo;
using ECommereceApi.Services.classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"))
    .AddInterceptors(new SoftDeleteInterceptor());
});


#region FileServer

var cloudinaryCredentials = builder.Configuration.GetSection("Cloudinary");
var account = new Account(
    cloudinaryCredentials["CloudName"],
    cloudinaryCredentials["ApiKey"],
    cloudinaryCredentials["ApiSecret"]
);

builder.Services.AddSingleton(account);
builder.Services.AddScoped<Cloudinary>();
#endregion

#region Localization Service
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SupportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("ar-EG")
    };
    options.SupportedCultures = options.SupportedCultures;
    options.SupportedUICultures = options.SupportedCultures;
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: "ar-EG", uiCulture: "ar-EG");

});

builder.Services.AddScoped<ILanguageRepo, LanguageRepo>();
#endregion


builder.Services.AddAutoMapper(typeof(Program));

// IOC Container Services
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("myPolicy");
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
