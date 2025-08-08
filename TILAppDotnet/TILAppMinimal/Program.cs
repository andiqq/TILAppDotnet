using Microsoft.EntityFrameworkCore;
using TILApp.Data;
using TILApp.Models;
using TILAppMinimal.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AcronymContext")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<Context>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("Account").MapIdentityApi<User>();

app.UseHttpsRedirection();

app.MapAcronymEndpoints();

app.MapCategoryEndpoints();

app.MapUserEndpoints();

app.Run();

public abstract partial class Program;