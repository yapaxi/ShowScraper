using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic.TVMaze
{
    public class ShowDatabase : IShowDatabase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        public async Task<IReadOnlyList<Show>> GetAllShows()
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://api.tvmaze.com/shows"));

            var stringContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Show[]>(stringContent);
        }
    }
}
