
using DominosStockOrder.Shared.ViewModels;
using System.Net.Http.Json;

namespace DominosStockOrder.Client.Services
{
    public class ExtraInventoryService : IExtraInventoryService
    {
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<string, float> _extraInventory = [];

        public ExtraInventoryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Clear()
        {
            _extraInventory.Clear();
        }

        public async Task FetchExtraInventoryFor(DateTime date)
        {
            Clear();

            using var scope = _serviceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var results = await httpClient.GetFromJsonAsync<IEnumerable<ExtraInventoryVM>>($"/api/FoodTheo/Extra/{date:s}");

            if (results is null)
                return;

            _extraInventory = results.ToDictionary(x => x.PulseCode, x => x.FoodTheo);
        }

        public float GetExtraInventoryForPulseCode(string pulseCode)
        {
            if (_extraInventory.TryGetValue(pulseCode, out var result))
                return result;

            return 0;
        }
    }
}
