using Newtonsoft.Json.Linq;
using ShowScraper.BusinessLogic.Contracts;
using ShowScraper.BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowScraper.BusinessLogic
{
    public class ShowService : IShowService
    {
        private readonly IStorageProvider _storageProvider;

        public ShowService(IStorageProvider storageProvider)
        {
            this._storageProvider = storageProvider;
        }

        public Task<JArray> GetShows(int page) => _storageProvider.GetShows(page);
    }
}
