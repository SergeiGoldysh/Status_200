using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Status_200;
using Status_200WebAPI.AuthBasic;
using Status_200WebAPI.Interfaces;
using Status_200WebAPI.UserServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen
    (c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        // Добавляем схему безопасности Basic Auth
        c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            Description = "Basic Authorization header using the Bearer scheme."
        });

        // Требуем авторизацию Basic Auth для каждого запроса
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        new string[] { }
                    }
                });
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddCors();
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}
app.UseCors(builder =>
builder
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()

);
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var c = new System.Net.Http.HttpClient();
c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic");
app.Use(async (context, next) =>
{
    await next.Invoke();
});

app.MapControllers();

app.Run();
