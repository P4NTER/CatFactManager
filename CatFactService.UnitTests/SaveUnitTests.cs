using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Moq;
using CatFactManager.Models;
using CatFactManager.Repositories;

namespace CatFactService.UnitTests
{
    public class SaveUnitTests
    {
        [Fact]
        public async Task SaveFactsAsync_Succeeds_ReturnsSuccess()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            var facts = new List<CatFact>
            {
                new CatFact { Fact = "A cat fact.", Length = 11 }
            };
            mockRepo.Setup(r => r.SaveAsync(facts, path)).Returns(() => Task.CompletedTask);
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.SaveFactsAsync(facts, path);

            result.Success.Should().BeTrue();
            result.Message.Should().Be("Facts saved successfully.");
            mockRepo.Verify(r => r.SaveAsync(facts, path), Times.Once);
        }

        [Fact]
        public async Task SaveFactsAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            var facts = new List<CatFact>();
            mockRepo.Setup(r => r.SaveAsync(facts, path)).Returns(() => Task.FromException(new Exception("io error")));
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.SaveFactsAsync(facts, path);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("io error");
            mockRepo.Verify(r => r.SaveAsync(facts, path), Times.Once);
        }
    }
}
