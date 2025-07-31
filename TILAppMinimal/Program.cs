using Microsoft.EntityFrameworkCore;
using TILApp.Data;
using TILAppMinimal.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var i = new
{
    error = "Cannot delete user",
    message = "User has associated acronyms. Delete or reassign them first.",
    details = "This user cannot be deleted because they have acronyms associated with their account."
};

// Add services to the container.
builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AcronymContext")));

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

app.UseHttpsRedirection();

app.MapAcronymEndpoints();

app.MapCategoryEndpoints();

app.MapUserEndpoints();

app.Run();

public abstract partial class Program;