using MessengerAPI;
using MessengerAPI.Interfaces;
using MessengerAPI.Services;
using MessengerAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ISignUpService, SignUpService>();
builder.Services.AddTransient<ISignInService, SignInService>();
builder.Services.AddTransient<ISignOutService, SignOutService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IChatRepository, ChatRepository>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<IUserChatRepository, UserGroupRepository>();
builder.Services.AddTransient<IUserTypeRepository, UserTypeRepository>();
builder.Services.AddTransient<ISessionRepository, SessionRepository>();

builder.Host.ConfigureServices((host, services) =>
{
    IConfigurationSection section = host.Configuration.GetSection("ConnectionStrings");
    services.Configure<Connections>(section);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
