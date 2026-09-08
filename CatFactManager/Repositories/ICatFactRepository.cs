using System;
using System.Collections.Generic;
using System.Text;
using CatFactManager.Models;

namespace CatFactManager.Repositories
{
    public interface ICatFactRepository
    {
        public Task AppendAsync(CatFact catFact, string filePath);
        public Task SaveAsync(List<CatFact> catFacts, string filePath);
        public Task<List<CatFact>?> ReadAsync(string filePath);
    }
}
