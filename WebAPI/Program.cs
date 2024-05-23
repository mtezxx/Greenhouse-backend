using System.Net;
using System.Net.Mail;
using System.Text;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Auth;
using Domain.Entity;
using DotNetEnv;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.TraversePath().Load();

builder.Services.AddTransient<SmtpClient>(_ =>
{
    var client = new SmtpClient("bulk.smtp.mailtrap.io", 587)
    {
        Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("EMAIL_USERNAME"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD")),
        EnableSsl = true
    };
    return client;
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddScoped<IMeasurementLogic, MeasurementLogic>();
builder.Services.AddScoped<IThresholdLogic, ThresholdLogic>();
builder.Services.AddScoped<IThresholdDao, ThresholdDao>();
builder.Services.AddScoped<IEmailLogic, EmailLogic>();
builder.Services.AddScoped<IEmailDao, EmailDao>();
builder.Services.AddScoped<IAuthDao, AuthDao>();
builder.Services.AddDbContext<EfcContext>();
builder.Services.AddScoped<IMeasurementDao<Temperature>, MeasurementDao<Temperature>>();
builder.Services.AddScoped<IMeasurementDao<Humidity>, MeasurementDao<Humidity>>();
builder.Services.AddScoped<IMeasurementDao<Light>, MeasurementDao<Light>>();
builder.Services.AddScoped<INotificationDao, NotificationDao>();
builder.Services.AddScoped<INotificationLogic, NotificationLogic>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IDeviceStatusDao, DeviceStatusDao>();
builder.Services.AddScoped<IDeviceStatusLogic, DeviceStatusLogic>();
AuthorizationPolicies.AddPolicies(builder.Services);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EfcContext>();
    db.Database.Migrate();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());
//app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();