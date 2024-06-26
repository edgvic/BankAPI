using BankAPI.Data;
using BankAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BankingContext>(options =>
    options.UseInMemoryDatabase("BankingDB"));

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    ControllerActionEndpointConventionBuilder controllerActionEndpointConventionBuilder = endpoints.MapControllers();
});

app.Run();