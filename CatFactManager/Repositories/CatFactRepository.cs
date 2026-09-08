using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using CatFactManager.Models;

namespace CatFactManager.Repositories
{
    public class CatFactRepository : ICatFactRepository
    {
        public async Task AppendAsync(CatFact catFact, string filePath)
        {
            string serializedCatFact = System.Text.Json.JsonSerializer.Serialize(catFact);
            await System.IO.File.AppendAllTextAsync(filePath, serializedCatFact + Environment.NewLine);
        }

        public async Task SaveAsync(List<CatFact> catFacts, string filePath)
        {
            List<string> serializedCatFacts = new List<string>();
            foreach (CatFact catFact in catFacts)
            {
                string serializedCatFact = System.Text.Json.JsonSerializer.Serialize(catFact);
                serializedCatFacts.Add(serializedCatFact);
            }
            await System.IO.File.WriteAllLinesAsync(filePath, serializedCatFacts);
        }

        public async Task<List<CatFact>?> ReadAsync(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }
            
            string[] lines = await System.IO.File.ReadAllLinesAsync(filePath);
            if (lines.Length == 0)
            {
                return null;
            }

            List<CatFact> catFacts = new List<CatFact>();
            foreach (string line in lines)
            {
                CatFact? catFact = System.Text.Json.JsonSerializer.Deserialize<CatFact>(line);
                if (catFact != null)
                {
                    catFacts.Add(catFact);
                }
            }
            return catFacts;
        }
    }
}
