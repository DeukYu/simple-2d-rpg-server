using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DB;
using WebServer.Repositories;
using WebServer.Services;
using WebServer.Packet;

namespace WebServer;

public class Program
{
    public static void Main(string[] args)
    {
        var configPath = "../config.json";
        ConfigManager.Instance.LoadConfig(configPath);

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddControllers(options =>
            {
                options.InputFormatters.Add(new ProtobufInputFormatter());
                options.OutputFormatters.Add(new ProtobufOutputFormatter());
            })
            .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        });

        builder.Services.AddDbContext<SharedDB>(options =>
        {
            options.UseMySql(ConfigManager.Instance.SharedDbConfig.GetConnectionConfig(), new MySqlServerVersion(new Version(8, 0, 29)));
        });

        builder.Services.AddDbContext<AccountDB>(options =>
        {
            options.UseMySql(ConfigManager.Instance.AccountDbConfig.GetConnectionConfig(), new MySqlServerVersion(new Version(8, 0, 29)));
        });


        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ISharedRepository, SharedRepository>();

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
    }
}