using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Moq;
using CatFactManager.Models;
using CatFactManager.Repositories;

namespace CatFactService.UnitTests
{
    public class AppendUnitTests
    {
        [Fact]
        public async Task AppendFactAsync_Succeeds_ReturnsSuccess()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            var fact = new CatFact { Fact = "A cat fact.", Length = 11 };
            mockRepo.Setup(r => r.AppendAsync(fact, path)).Returns(() => Task.CompletedTask);
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.AppendFactAsync(fact, path);

            result.Success.Should().BeTrue();
            result.Message.Should().Be("Fact saved successfully.");
            mockRepo.Verify(r => r.AppendAsync(fact, path), Times.Once);
        }

        [Fact]
        public async Task AppendFactAsync_WhenRepositoryThrows_ReturnsFailure()
        {
            var path = "path.txt";
            var mockRepo = new Mock<ICatFactRepository>();
            var fact = new CatFact { Fact = "A cat fact.", Length = 11 };
            mockRepo.Setup(r => r.AppendAsync(fact, path)).Returns(() => Task.FromException(new InvalidOperationException("disk full")));
            var service = new CatFactManager.Services.CatFactService(mockRepo.Object);

            var result = await service.AppendFactAsync(fact, path);

            result.Success.Should().BeFalse();
            result.Message.Should().Contain("disk full");
            mockRepo.Verify(r => r.AppendAsync(fact, path), Times.Once);
        }
    }
}
