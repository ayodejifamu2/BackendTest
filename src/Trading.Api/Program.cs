namespace Trading.Api
{
using Microsoft.EntityFrameworkCore;
using Trading.Infrastructure.Data;
using Trading.Application.Interfaces;
using Trading.Infrastructure.Repositories;
using MediatR;
using Trading.Application.Commands;
using Mapster;
using Trading.Messaging.Service;

public static class Program
{
    private static void ConfigureApi(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddDbContext<TradingDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
             ?? "Data Source=trading.db"));
        builder.Services.AddScoped<ITradeRepository, TradeRepository>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ExecuteTradeCommand).Assembly));
        builder.Services.AddMapster();
        
        builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
        
        builder.Services.AddSingleton<ITradeMessageProducer, RabbitMqTradeMessageProducer>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void ConfigureApp(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureApi(builder);
        var app = builder.Build();
        ConfigureApp(app);
        app.Run();
    }
}
}