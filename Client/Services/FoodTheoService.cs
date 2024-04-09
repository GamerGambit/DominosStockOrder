using DominosStockOrder.Shared.ViewModels;

using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace DominosStockOrder.Client.Services
{
    public class FoodTheoService
    {
        private readonly IServiceProvider _serviceProvider;
        public bool Loading { get; private set; }
        private Dictionary<string, WorkingsVM> Workings { get; set; } = [];

        public FoodTheoService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task FetchFoodTheoAsync()
        {
            Workings.Clear();
            Loading = true;

            using var scope = _serviceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var results = await httpClient.GetFromJsonAsync<IEnumerable<WorkingsVM>>("/api/FoodTheo");
            Loading = false;

            if (results is null)
                return;

            foreach (var result in results) {
                Workings.Add(result.PulseCode, result);
            }
        }

        public IEnumerable<WorkingsVM> GetAllWorkings()
        {
            foreach (var working in Workings.Values)
            {
                yield return working;
            }
        }

        public WorkingsVM? GetWorkingsFromPulseCode(string pulseCode)
        {
            if (Workings.TryGetValue(pulseCode, out var workings))
                return workings;

            return null;
        }
    }
}
