using DominosStockOrder.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DominosStockOrder.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton<FoodTheoService>();
            builder.Services.AddSingleton<IInventoryItemService, InventoryItemService>();
            builder.Services.AddSingleton<IFinalOrderCalculatorService, FinalOrderCalculatorService>();
            builder.Services.AddSingleton<IExtraInventoryService, ExtraInventoryService>();
            builder.Services.AddSingleton<ITransferService, TransferService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var app = builder.Build();

            await app.Services.GetRequiredService<FoodTheoService>().FetchFoodTheoAsync();
            await app.Services.GetRequiredService<IInventoryItemService>().FetchInventoryDataAsync();
            await app.Services.GetRequiredService<SettingsService>().FetchFromServer();

            await app.RunAsync();
        }
    }
}
