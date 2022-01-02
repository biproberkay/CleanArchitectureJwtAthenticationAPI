using API.Extensions;
using Application.Settings;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
var settings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();

#region CustumizedServices are addding here
builder.Services.AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddIdentity(settings.IdentitySettings);
builder.Services.AddDeveloperServices();
builder.Services.AddRepositories();
builder.Services.AddJwtAuthentication(settings.JwtSettings);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
