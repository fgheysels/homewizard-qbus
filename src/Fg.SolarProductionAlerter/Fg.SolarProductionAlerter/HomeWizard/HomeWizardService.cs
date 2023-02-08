using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fg.SolarProductionAlerter.HomeWizard.Models;

namespace Fg.SolarProductionAlerter.HomeWizard
{
    internal class HomeWizardService
    {
        private readonly HomeWizardDevice _p1Meter;
        private static readonly HttpClient _http = new HttpClient();

        public HomeWizardService(HomeWizardDevice p1Meter)
        {
            _p1Meter = p1Meter;
            _http.BaseAddress = new Uri($"http://{p1Meter.IPAddress}/api/");
        }

        public async Task<CurrentMeasurement> GetCurrentMeasurements()
        {
            var response = await _http.GetAsync("v1/data");

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException(
                    $"Request to retrieve HomeWizard data failed with statuscode {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var measurements = JsonSerializer.Deserialize<CurrentMeasurement>(json);

            if (measurements == null)
            {
                throw new InvalidOperationException("Unable to deserialize response to model");

            }

            return measurements;
        }
    }
}
