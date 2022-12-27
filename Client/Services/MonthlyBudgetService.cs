using Cheddar.Shared.Models;
using System.Net.Http.Json;
using System.Net.Http

namespace Cheddar.Client.Services {
    public class MonthlyBudgetService {

        private readonly HttpClient ApiClient;

        public MonthlyBudgetService(HttpClient apiClient)
        {
            ApiClient = apiClient;
        }

        public async Task<MonthlyBudgetModel> GetMonthlyBudget(int month, int year) {
            Console.WriteLine("Entered into monthly budget service GetMonthlyBudget");
            string request = String.Concat("api/GetMonthlyBudget?month=", month.ToString(),"&year=", year.ToString());
            return await ApiClient.GetFromJsonAsync<MonthlyBudgetModel>(request);
        }

        public async Task<MonthlyBudgetModel> GetLatestMonthlyBudget() {
            Console.WriteLine("Entered into monthly budget service GetLatestMonthlyBudget");
            string request = String.Concat("api/GetLatestMonthlyBudget");
            return await ApiClient.GetFromJsonAsync<MonthlyBudgetModel>(request);
        }

        public async Task<List<MonthlyBudgetModel>> GetAllMonthlyBudgetDatesForUser() {
            Console.WriteLine("Entered into monthly budget service GetAllMonthlyBudgets");
            string request = String.Concat("api/GetAllMonthlyBudgets");
            return await ApiClient.GetFromJsonAsync<List<MonthlyBudgetModel>>(request);
        }

        public async Task UpdateMonthlyBudgetForUser(MonthlyBudgetModel monthlyBudgetModelToUpdate) {
            Console.WriteLine("Entered into monthly budget service UpdateMonthlyBudgetForUser");
            string request = String.Concat("api/UpdateMonthlyBudget");
            await ApiClient.PatchAsJsonAsync<MonthlyBudgetModel>(request, monthlyBudgetModelToUpdate);
        }
    }
}
