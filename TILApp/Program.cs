using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// --> Configure Services

builder.Services.AddControllers();

builder.Services.AddDbContext<Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AcronymContext")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
       options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
       {
           In = ParameterLocation.Header,
           Name = "Authorization",
           Type = SecuritySchemeType.ApiKey
       }); 
       options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<Context>();

var app = builder.Build();

// --> Middleware Pipeline

app.Services.CreateScope().ServiceProvider.GetRequiredService<Context>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


