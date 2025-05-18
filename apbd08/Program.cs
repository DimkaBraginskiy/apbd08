using apbd08.Repositories;
using apbd08.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Adding controllers to the Services and Scoped Services (our custom Services)
        builder.Services.AddControllers();
        builder.Services.AddScoped<TripRepository>();
        builder.Services.AddScoped<ClientRepository>();
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<ITripService, TripService>();
        
        // 2. Adding our own services
        
        builder.Services.AddAuthorization();

        // 4. OpenApi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}