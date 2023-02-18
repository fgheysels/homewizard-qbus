using Fg.SolarProductionAlerter.HomeWizard.Models;
using System.Text.Json;

namespace Fg.SolarProductionAlerter.HomeWizard
{
    public interface IHomeWizardService
    {
        Task<Measurement> GetCurrentMeasurements();
    }

    internal class HomeWizardService : IHomeWizardService
    {
        private static readonly HttpClient _http = new HttpClient();

        public HomeWizardService(HomeWizardDevice p1Meter)
        {
            _http.BaseAddress = new Uri($"http://{p1Meter.IPAddress}/api/");
        }

        public async Task<Measurement> GetCurrentMeasurements()
        {
            var response = await _http.GetAsync("v1/data");

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException(
                    $"Request to retrieve HomeWizard data failed with statuscode {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var measurement = JsonSerializer.Deserialize<Measurement>(json);

            if (measurement == null)
            {
                throw new InvalidOperationException("Unable to deserialize response to model");

            }

            return measurement;
        }
    }
}
