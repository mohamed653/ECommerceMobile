using ECommereceApi.Data;
using ECommereceApi.IRepo;
using ECommereceApi.Models;
using ECommereceApi.Repo;
using ECommereceApi.Services.classes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.OpenApi.Models;
using System.Text.Unicode;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Globalization;
using ECommereceApi.Extensions;
using ECommereceApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

var webHostEnvironment = builder.Services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v2", new OpenApiInfo
//    {
//        Title = "Mobile ECommerce API",
//        Version = "v1",
//        Description = "Mobile ECommerce ASP.NET Core Web API",
//        Contact = new OpenApiContact
//        {
//            Name = "Mohamed_Hamed",
//            Email = "mohamedHamed@gmail.com"
//        }

//    });
//    c.IncludeXmlComments(webHostEnvironment.WebRootPath + "\\mydoc.xml");
//});


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

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(o =>
//{
//    o.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:secretkey"))),
//    };
//});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT token in this format: Bearer {token}",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.IncludeXmlComments(webHostEnvironment.WebRootPath + "\\mydoc.xml");
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


//File Server Service
builder.Services.AddScoped<IFileCloudService, FileCloudService>();


//builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IOfferRepo, OfferRepo>();
builder.Services.AddScoped<IWebInfoRepo, WebInfoRepo>();

builder.Services.AddScoped<IWishListRepo, WishListRepo>();
builder.Services.AddScoped<IUserManagementRepo, UserManagementRepo>();
builder.Services.AddScoped<IMailRepo, MailRepo>();


builder.Services.AddScoped<ICartRepo, CartRepo>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("myPolicy");
app.UseAuthentication();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
