using ForesterAPI.Data;
using ForesterAPI.Data.Connection;
using ForesterAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//managers
builder.Services.AddSingleton(typeof(FileApplicationManager));
builder.Services.AddSingleton(typeof(PictureManager));
builder.Services.AddSingleton(typeof(SQLManager));

//databases
builder.Services.AddSingleton(typeof(AccountDatabase));
builder.Services.AddSingleton(typeof(ApplicationDatabase));
builder.Services.AddSingleton(typeof(FileDatabase));

//other
builder.Services.AddSingleton(typeof(RequirementChecker));
builder.Services.AddSingleton(typeof(FileConfigurator));

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
        };
    });



builder.Services.AddAuthorization();

//JSON
builder.Services.AddControllers(options =>
    options.AllowEmptyInputInBodyModelBinding = true
    ).AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
