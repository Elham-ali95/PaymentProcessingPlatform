using Microsoft.EntityFrameworkCore;
using Paymentprocess.Api.Data;
using Paymentprocess.Api.Interfaces;
using Paymentprocess.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<ICustomers, CustomerService>();
builder.Services.AddScoped<IPayments, PaymentService>();
builder.Services.AddScoped<IRepayments, RepaymentService>();
builder.Services.AddScoped<ITransactions, TransactionServices>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();

