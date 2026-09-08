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
    public class ReadUnitTests
    {
        [Fact]
        public async Task ReadFactsAsync_WhenNoFacts_ReturnsSuccessWithNullFacts()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            mockRepo.Setup(r => r.ReadAsync(path)).Returns(() => Task.FromResult<List<CatFact>?>(null));
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.ReadFactsAsync(path);

            result.Success.Should().BeTrue();
            result.Facts.Should().BeNull();
            result.Message.Should().Be($"No facts found in {path}.");
            mockRepo.Verify(r => r.ReadAsync(path), Times.Once);
        }

        [Fact]
        public async Task ReadFactsAsync_WhenFactsExist_ReturnsFacts()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            var facts = new List<CatFact>
            {
                new CatFact { Fact = "A cat fact.", Length = 11 }
            };
            mockRepo.Setup(r => r.ReadAsync(path)).Returns(() => Task.FromResult<List<CatFact>?>(facts));
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.ReadFactsAsync(path);

            result.Success.Should().BeTrue();
            result.Facts.Should().NotBeNull().And.HaveCount(1);
            result.Message.Should().Be("Facts read successfully.");
            mockRepo.Verify(r => r.ReadAsync(path), Times.Once);
        }

        [Fact]
        public async Task ReadFactsAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            mockRepo.Setup(r => r.ReadAsync(path)).Returns(() => Task.FromException<List<CatFact>?>(new Exception("read error")));
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.ReadFactsAsync(path);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("read error");
            mockRepo.Verify(r => r.ReadAsync(path), Times.Once);
        }
    }
}
