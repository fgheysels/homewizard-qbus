using System.Net;
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

            return new EqoWebSession(address, port, response.Value.Id);
        }

        private readonly string _sessionCookie;
        private readonly string _address;
        private readonly int _port;

        public EqoWebSession(string address, int port, string sessionCookie)
        {
            _address = address;
            _port = port;
            _sessionCookie = sessionCookie;
        }

        public async Task<IEnumerable<ControlListGroup>> GetControlLists()
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

        public async Task SetControlItemValue(int channel, int value)
        {
            var setControlData = new EqoWebRequest<SetControlItemValueContent>()
            {
                Type = 13,
                Value = new SetControlItemValueContent()
                {
                    Channel = channel,
                    Value = new[] { value }
                }
            };

            var response = await SendRequestAsync<object>(_address, _port, HttpMethod.Post, _sessionCookie,
                new KeyValuePair<string, string>("strJSON", JsonSerializer.Serialize(setControlData)));
        }

        private static async Task<EqoWebResponse<TResponse>> SendRequestAsync<TResponse>(string address, int port, HttpMethod method, string? sessionCookie = null, params KeyValuePair<string, string>[] bodyValues)
        {
            var message = new HttpRequestMessage(method, $"http://{address}:{port}/default.aspx");

            message.Content = new FormUrlEncodedContent(bodyValues);

            if (sessionCookie != null)
            {
                message.Headers.Add("Cookie", $"i={sessionCookie}");
            }

            var response = await _httpClient.SendAsync(message);

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException("EqoWeb request failed");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<EqoWebResponse<TResponse>>(responseContent);
        }
    }
}
