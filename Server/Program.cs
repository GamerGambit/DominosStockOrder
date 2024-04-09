using DominosStockOrder.Server.Hubs;
using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.PulseApi;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SendGrid.Extensions.DependencyInjection;

namespace DominosStockOrder.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();
            builder.Services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("PulseApiHttpClient", (services, client) => {
                var config = services.GetRequiredService<IConfiguration>();
                var pulseUrl = config.GetValue("PulseApiUrl", "https://pulseapi");
                client.BaseAddress = new Uri(pulseUrl!);
            });
            builder.Services.AddDbContext<StockOrderContext>();
            builder.Services.AddSingleton<IInventoryUpdaterService, InventoryUpdaterService>();
            builder.Services.AddSingleton<IPendingOrdersCacheService, PendingOrdersCacheService>();
            builder.Services.AddHostedService<ConsolidatedInventoryRunnerService>();
            builder.Services.AddSingleton<IConsolidatedInventoryService, ConsolidatedInventoryService>();
            builder.Services.AddSingleton<IPulseApiClient>(services => {

                return new PulseApiClient(services.GetRequiredService<IHttpClientFactory>().CreateClient("PulseApiHttpClient"));
            });
            builder.Services.AddSendGrid(options => options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));
            builder.Services.AddSingleton<ISavedOrderCacheService, SavedOrderCacheService>();
            builder.Services.AddSingleton<Status>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<HubOptions>(options =>
            {
                options.MaximumReceiveMessageSize = 65535;
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://purchasing.dominos.com.au")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials();
                });
            });

            var app = builder.Build();
            app.UseResponseCompression();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StockOrderContext>();
                context.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseAuthorization();
            app.UseCors();

            app.MapRazorPages();
            app.MapControllers();
            app.MapHub<PurchasingHub>("/purchasinghub");
            app.MapFallbackToFile("index.html");

            await app.Services.GetRequiredService<IConsolidatedInventoryService>().FetchWeeklyFoodTheoAsync();
            // Since this container restarts every morning at 6am (cronjob), get the ending inventory from yesterday.
            await app.Services.GetRequiredService<IConsolidatedInventoryService>().FetchEndingInventoryAsync(DateTime.Now.AddDays(-1));

            await app.RunAsync();
        }
    }
}
