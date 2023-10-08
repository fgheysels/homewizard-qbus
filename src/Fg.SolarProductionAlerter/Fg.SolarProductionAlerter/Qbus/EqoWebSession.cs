using Fg.SolarProductionAlerter.Configuration;
using Fg.SolarProductionAlerter.Qbus.Models;
using System.Text.Json;

namespace Fg.SolarProductionAlerter.Qbus
{
    internal class EqoWebSession
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        internal static async Task<EqoWebSession> CreateSessionAsync(string address, int port, string username, string password)
        {
            var loginData = new
            {
                Type = 1,
                Value = new
                {
                    Usr = username,
                    Psw = password ?? string.Empty
                }
            };

            var response = await SendRequestAsync<LoginResponseContent>(address, port, HttpMethod.Post, sessionCookie: null,
                                        new KeyValuePair<string, string>("strJSON", JsonSerializer.Serialize(loginData)));

            if (response.Value.Rsp == false)
            {
                throw new Exception("Login to QBUS failed");
            }

            return new EqoWebSession(address, port, response.Value.Id, DateTime.UtcNow);
        }

        private readonly string _sessionCookie;
        private readonly string _address;
        private readonly int _port;
        private readonly DateTime _sessionStartTime;

        private EqoWebSession(string address, int port, string sessionCookie, DateTime sessionStartTime)
        {
            _address = address;
            _port = port;
            _sessionCookie = sessionCookie;
            _sessionStartTime = sessionStartTime;
        }

        public TimeSpan SessionLifeTime
        {
            get
            {
                return DateTime.UtcNow - _sessionStartTime;
            }
        }

        private readonly TimeSpan MaxSessionLifeTime = TimeSpan.FromMinutes(20);

        public bool IsExpired
        {
            get
            {
                return SessionLifeTime > MaxSessionLifeTime;
            }
        }

        public async Task<IEnumerable<ControlItem>> GetSolarIndicatorControlItems(QbusConfigurationSettings settings)
        {
            string[] configuredSolarIndicators = settings.SolarIndicators.Split(",");

            var eqoWebControlLists = await GetControlLists();

            var controlItems = eqoWebControlLists.SelectMany(m => m.Items)
                                                 .Where(c => configuredSolarIndicators.Contains(c.Name, StringComparer.OrdinalIgnoreCase))
                                                 .ToArray();

            return controlItems;
        }

        private async Task<IEnumerable<ControlListGroup>> GetControlLists()
        {
            var getControlListsData = new EqoWebRequest<object>()
            {
                Type = 10,
                Value = null
            };

            var response = await SendRequestAsync<ControlListResponseContent>(_address, _port, HttpMethod.Post, _sessionCookie,
                new KeyValuePair<string, string>("strJSON", JsonSerializer.Serialize(getControlListsData)));

            return response.Value.Groups;
        }

        public async Task SetSolarIndicatorAsync(ControlItem solarIndicator, PowerUsageState state)
        {
            int value = 0;

            switch (state)
            {
                case PowerUsageState.Unknown:
                    value = 0; 
                    break;
                case PowerUsageState.NotEnoughProduction:
                    value = 1; 
                    break;
                case PowerUsageState.BreakEven:
                    value = 2;
                    break;
                case PowerUsageState.OverProduction:
                    value = 3;
                    break;
                case PowerUsageState.ExtremeOverProduction:
                    value = 4;
                    break;
            }

            await SetControlItemValueAsync(solarIndicator.Channel, value);
        }

        public async Task SetControlItemValueAsync(int channel, int value)
        {
            var setControlData = new EqoWebRequest<SetControlItemValueContent>()
            {
                Type = 12,
                Value = new SetControlItemValueContent()
                {
                    Channel = channel,
                    Value = new[] { value }
                }
            };

            var response = await SendRequestAsync<object>(
                                    _address,
                                    _port,
                                    HttpMethod.Post,
                                    _sessionCookie,
                                    new KeyValuePair<string, string>("strJSON", JsonSerializer.Serialize(setControlData)));

            if (response.Type != 13)
            {
                throw new InvalidOperationException($"Setting control-item value failed - {response.Type}={response.Value}");
            }
        }

        private static async Task<EqoWebResponse<TResponse>> SendRequestAsync<TResponse>(
            string address,
            int port,
            HttpMethod method,
            string? sessionCookie = null,
            params KeyValuePair<string, string>[] bodyValues)
        {
            var message = new HttpRequestMessage(method, $"http://{address}:{port}/default.aspx");

            message.Content = new FormUrlEncodedContent(bodyValues);

            if (sessionCookie != null)
            {
                message.Headers.Add("Cookie", $"i={sessionCookie}");
            }

            var response = await _httpClient.SendAsync(message);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException("EqoWeb request failed - " + responseContent);
            }

            return JsonSerializer.Deserialize<EqoWebResponse<TResponse>>(responseContent);
        }
    }
}
