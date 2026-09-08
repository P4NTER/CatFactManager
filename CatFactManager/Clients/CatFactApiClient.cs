using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CatFactManager.Models;

namespace CatFactManager.Clients
{
    public class CatFactApiClient : ICatFactApiClient
    {
        private readonly HttpClient _client;

        public CatFactApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<CatFact?> GetCatFactAsync()
        {
            var response = await _client.GetAsync("https://catfact.ninja/fact");
            CatFact? catFact = await response.Content.ReadFromJsonAsync<CatFact>();
            return catFact;
        }
    }
}
