using DominosStockOrder.Shared.ViewModels;
using System.Net.Http.Json;

namespace DominosStockOrder.Client.Services
{
    public class SettingsService
    {
        public int NumFoodTheoWeeks { get; set; }

        private readonly IServiceProvider _serviceProvider;

        public SettingsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task FetchFromServer()
        {
            var scope = _serviceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var settings = await httpClient.GetFromJsonAsync<SettingsVM>("api/Settings");

            if (settings == null)
                throw new Exception("Settings is null");

            NumFoodTheoWeeks = settings.NumFoodTheoWeeks;
        }
    }
}
