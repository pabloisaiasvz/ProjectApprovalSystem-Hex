using Application.Interfaces;
using Application.Interfaces.IMappers;
using Application.Mappers;
using Application.UseCases;
using Infrastructrure.Command;
using Infrastructrure.Persistence;
using Infrastructrure.Query;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDbContext<TemplateContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IGenericoMapper, GenericoMapper>();

builder.Services.AddScoped<IGeneroCommand,GeneroCommand>();
builder.Services.AddScoped<IGeneroQuery, GeneroQuery>();
builder.Services.AddScoped<IGeneroService, GeneroService>();

builder.Services.AddScoped<IArtistaCommand,ArtistaCommand>();
builder.Services.AddScoped<IArtistaQuery,ArtistaQuery>();
builder.Services.AddScoped<IArtistaService,ArtistaService>();

builder.Services.AddScoped<IAlbumCommand,AlbumCommand>();
builder.Services.AddScoped<IAlbumQuery,AlbumQuery>();
builder.Services.AddScoped<IAlbumService,AlbumService>();
builder.Services.AddScoped<IAlbumMapper,AlbumMapper>();


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
