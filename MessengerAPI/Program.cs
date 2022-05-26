using MessengerAPI;
using MessengerAPI.Interfaces;
using MessengerAPI.Services;
using MessengerAPI.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MessengerAPI.Middleware;
using MessengerAPI.Options;
using MessengerAPI.Contexts;
using Dapper;
using MessengerAPI.SqlTypeHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("MessengerAPI-v1", new OpenApiInfo { Title = "Messenger API", Version="v1" });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ISessionRepository, SessionRepository>();

builder.Services.AddTransient<IChatRepository, ChatRepository>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<IUserChatRepository, UserChatRepository>();
builder.Services.AddTransient<IUserTypeRepository, UserTypeRepository>();

builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IMessageService, MessageService>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ILinkService, LinkService>();
builder.Services.AddTransient<ILinkRepository, LinkRepository>();
builder.Services.AddTransient<IConfirmationCodeRepository, ConfirmationCodeRepository>();

builder.Services.AddTransient<IFileRepository, FileRepository>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddScoped<IServiceContext, ServiceContext>();

builder.Services.AddTransient<IChannelService, ChannelService>();
builder.Services.AddTransient<IChannelLinkRepository, ChatLinkRepository>();

SqlMapper.AddTypeHandler(new DateTimeHandler());

builder.Host.ConfigureServices((host, services) =>
{
    IConfigurationSection section = host.Configuration.GetSection("ConnectionStrings");
    services.Configure<Connections>(section);
    section = host.Configuration.GetSection("Jwt");
    services.Configure<JwtOptions>(section);
    section = host.Configuration.GetSection("Email");
    services.Configure<EmailOptions>(section);
    section = host.Configuration.GetSection("Code");
    services.Configure<CodeOptions>(section);

    services.AddScoped<IServiceContext, ServiceContext>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // укзывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = host.Configuration["Jwt:Issuer"],

            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = host.Configuration["Jwt:Audience"],
            // будет ли валидироваться время существования
            ValidateLifetime = true,

            // установка ключа безопасности
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(host.Configuration["Jwt:Key"])),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(swagger =>
    {
        swagger.SwaggerEndpoint("/swagger/MessengerAPI-v1/swagger.json", "Messanger API v1");
    });
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
