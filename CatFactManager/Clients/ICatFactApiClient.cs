using System;
using System.Collections.Generic;
using System.Text;
using CatFactManager.Models;

namespace CatFactManager.Clients
{
    public interface ICatFactApiClient
    {
        public Task<CatFact?> GetCatFactAsync();
    }
}
