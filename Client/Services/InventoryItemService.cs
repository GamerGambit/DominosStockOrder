using DominosStockOrder.Shared.ViewModels;

using System.Net.Http.Json;

namespace DominosStockOrder.Client.Services
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, InventoryItemVM> itemDict = [];

        public InventoryItemService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task FetchInventoryDataAsync()
        {
            itemDict.Clear();

            using var scope = _serviceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

            var results = await httpClient.GetFromJsonAsync<IEnumerable<InventoryItemVM>>("/api/InventoryItems");

            if (results is null)
                return;

            foreach (var item in results)
            {
                AddItemToDictionary(item);
            }
        }

        public async Task<InventoryItemVM?> GetInventoryItemFromPulseCodeAsync(string pulseCode)
        {
            if (!itemDict.TryGetValue(pulseCode, out var item))
            {
                return await FetchInventoryItemFromPulseCode(pulseCode);
            }

            return item;
        }

        private async Task<InventoryItemVM?> FetchInventoryItemFromPulseCode(string pulseCode)
        {

            using var scope = _serviceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

            var result = await httpClient.GetFromJsonAsync<InventoryItemVM?>($"/api/InventoryItems/{pulseCode}");

            if (result is null)
                return null;

            AddItemToDictionary(result);
            return result;
        }

        private void AddItemToDictionary(InventoryItemVM item)
        {
            if (item is null)
                return;

            itemDict.Add(item.Code, item);
        }
    }
}
