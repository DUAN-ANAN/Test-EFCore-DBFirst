using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using test_EFCore.Interface;
using test_EFCore.Models;
using test_EFCore.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Northwind;User Id=sa;Password=andy54088;Trusted_Connection=True;TrustServerCertificate=true;"));

//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

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

app.UseAuthorization();

app.MapControllers();

app.Run();
