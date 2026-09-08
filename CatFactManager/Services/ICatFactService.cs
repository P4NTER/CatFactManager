using System;
using System.Collections.Generic;
using System.Text;
using CatFactManager.Models;

namespace CatFactManager.Services
{
    public interface ICatFactService
    {
        public Task<GenericResponse> AppendFactAsync(CatFact fact, string filePath);
        public Task<GenericResponse> SaveFactsAsync(List<CatFact> catFacts, string filePath);
        public Task<ReadFactsResponse> ReadFactsAsync(string filePath);
    }
}
