using Adapters;
using Adapters.Concrete;
using Microsoft.EntityFrameworkCore;
using Services.Abstract;
using Services.Concrete;
using Services.DataBase.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CoreContext>(opt => opt.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ExcelDatabase;Trusted_Connection=True;"));
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IExcelSchemaService, ExcelSchemaService>();
builder.Services.AddScoped<IExcelAdapter, OfficeOpenXmlAdapter>();

builder.Services.AddControllers();
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
