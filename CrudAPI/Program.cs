using CrudAPI.Context;
using CrudAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// A�adir servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSql")));

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TareaService>();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Comenta o elimina esta l�nea para desactivar HTTPS
    // app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();