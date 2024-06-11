using DominosStockOrder.Shared.DTOs;
using DominosStockOrder.Shared.ViewModels;

using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using static System.Net.WebRequestMethods;

namespace DominosStockOrder.Client.Services
{
    public class FoodTheoService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SettingsService _settings;
        public bool Loading { get; private set; }

        // Used to enable/disable the "Initial food theo" input on stock items.
        public bool EnableItemFoodTheoCheck { get; private set; } = true;
        public int NumWorkings { get; private set; } = 0;

        private Dictionary<string, WorkingsVM> Workings { get; set; } = [];

        public FoodTheoService(IServiceProvider serviceProvider, SettingsService settings)
        {
            _serviceProvider = serviceProvider;
            _settings = settings;
        }

        public async Task FetchFoodTheoAsync()
        {
            EnableItemFoodTheoCheck = true;
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

            NumWorkings = _settings.NumFoodTheoWeeks;
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

        public async Task OverrideFoodTheoWithDates(IEnumerable<DateRange> dateRanges)
        {
            EnableItemFoodTheoCheck = false;
            Loading = true;

            using var scope = _serviceProvider.CreateScope();
            using var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
            var res = await httpClient.PostAsJsonAsync("/api/FoodTheo/Custom", dateRanges);
            var results = await res.Content.ReadFromJsonAsync<IEnumerable<FoodTheoVM>>();

            var inStore = Workings.Select(x => KeyValuePair.Create(x.Key, x.Value.EndingInventory)).ToDictionary();

            Workings.Clear();
            var dict = new Dictionary<string, List<ItemWeeklyFoodTheo>>();

            foreach (var foodtheo in results)
            {
                foreach (var itemtheo in foodtheo.ItemTheos)
                {
                    var weeklyTheo = new ItemWeeklyFoodTheo
                    {
                        WeekEnding = foodtheo.WeekEnding,
                        IdealUsage = itemtheo.IdealUsage
                    };

                    if (!dict.TryAdd(itemtheo.PulseCode, [ weeklyTheo ]))
                    {
                        dict[itemtheo.PulseCode].Add(weeklyTheo);
                    }
                }
            }

            foreach (var ( pulseCode, weeklyTheos ) in dict)
            {
                Workings.Add(pulseCode, new WorkingsVM
                {
                    PulseCode = pulseCode,
                    WeeklyFoodTheo = weeklyTheos,
                    EndingInventory = inStore.TryGetValue(pulseCode, out var endingInv) ? endingInv : 0
                });
            }

            if (dateRanges.TryGetNonEnumeratedCount(out var count))
            {
                NumWorkings = count;
            }
            else
            {
                NumWorkings = dateRanges.Count();
            }

            Loading = false;
        }
    }
}
