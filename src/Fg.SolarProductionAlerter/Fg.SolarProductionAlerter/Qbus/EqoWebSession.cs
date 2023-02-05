﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
                    Psw = password
                }
            };

            var response = await SendRequestAsync<LoginResponseContent>(address, port, HttpMethod.Post,
                                        new KeyValuePair<string, string>("strJSON", JsonSerializer.Serialize(loginData)));

            if (response.Value.Rsp == false)
            {
                throw new Exception("Login to QBUS failed");
            }

            return new EqoWebSession(response.Value.Id);
        }

        private readonly string _sessionCookie;

        public EqoWebSession(string sessionCookie)
        {
                _sessionCookie = sessionCookie;
        }

        private static async Task<EqoWebResponse<TResponse>> SendRequestAsync<TResponse>(string address, int port, HttpMethod method, params KeyValuePair<string, string>[] bodyValues)
        {
            var message = new HttpRequestMessage(method, $"http://{address}:{port}/default.aspx");

            message.Content = new FormUrlEncodedContent(bodyValues);

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