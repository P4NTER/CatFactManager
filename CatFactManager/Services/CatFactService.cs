using System;
using System.Collections.Generic;
using System.Text;
using CatFactManager.Models;
using CatFactManager.Repositories;

namespace CatFactManager.Services
{
    public class GenericResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ReadFactsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CatFact>? Facts { get; set; }
    }

    public class CatFactService : ICatFactService
    {

        private readonly ICatFactRepository _catFactRepository;

        public CatFactService(ICatFactRepository catFactRepository)
        {
            _catFactRepository = catFactRepository;
        }

        public async Task<GenericResponse> AppendFactAsync(CatFact fact, string path)
        {
            try
            {
                await _catFactRepository.AppendAsync(fact, path);
                var response = new GenericResponse
                {
                    Success = true,
                    Message = "Fact saved successfully.",
                };
                return response;
            }
            catch (Exception ex)
            {
                var response = new GenericResponse
                {
                    Success = false,
                    Message = $"Error occurred during saving to file: {ex.Message}"
                };
                return response;
            }
        }

        public async Task<GenericResponse> SaveFactsAsync(List<CatFact> catFacts, string path)
        {
            try
            {
                await _catFactRepository.SaveAsync(catFacts, path);
                return new GenericResponse
                {
                    Success = true,
                    Message = "Facts saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = $"Error occurred while saving facts: {ex.Message}"
                };
            }
        }

        public async Task<ReadFactsResponse> ReadFactsAsync(string path)
        {
            try
            {
                List<CatFact>? facts = await _catFactRepository.ReadAsync(path);
                if (facts == null)
                {
                    return new ReadFactsResponse
                    {
                        Success = true,
                        Message = $"No facts found in {path}.",
                        Facts = null
                    };
                }
                return new ReadFactsResponse
                {
                    Success = true,
                    Message = "Facts read successfully.",
                    Facts = facts
                };
            }
            catch (Exception ex)
            {
                return new ReadFactsResponse
                {
                    Success = false,
                    Message = $"Error occurred while reading facts: {ex.Message}"
                };
            }
        }
    }
}
