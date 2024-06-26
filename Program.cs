using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Models;
using BankAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BankingContext>(options =>
    options.UseInMemoryDatabase("BankingDB"));

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllers();

var app = builder.Build();

// Add data seeding
using (var scope = app.Services.CreateScope())
{

    // Get the BankingContext instance with mocked data
    var context = scope.ServiceProvider.GetRequiredService<BankingContext>();

    var user = new User { Name = "Edgar Domenech", Email = "edgar.domenech@test.com" };
    context.Users.Add(user);

    var card = new Card { Number = "1234567812345678", Type = "Debit", Limit = 500m, Balance = 1000m, IsActive = false };
    context.Cards.Add(card);

    var account = new Account { IBAN = "ES1234567890123456789012", Balance = 1000m, User = user, Card = card, PIN = "" };
    context.Accounts.Add(account);

    var transaction1 = new Transaction { AccountId = account.Id, Amount = 200m, Type = "Deposit", Date = DateTime.Now };
    var transaction2 = new Transaction { AccountId = account.Id, Amount = 100m, Type = "Withdrawal", Date = DateTime.Now };
    context.Transactions.AddRange(transaction1, transaction2);

    context.SaveChanges();
}

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
