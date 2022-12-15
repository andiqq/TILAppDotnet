using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --> Configure Services

builder.Services.AddControllers();

builder.Services.AddDbContext<AcronymContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AcronymContext")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --> Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();


