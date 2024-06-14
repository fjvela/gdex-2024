using AutoFixture;
using GloboTicket.Catalog;
using Moq;
using Microsoft.Extensions.Logging;
using GloboTicket.Catalog.Controllers;
using Microsoft.AspNetCore.Mvc;
using GloboTicket.Catalog.Repositories;

namespace unittests
{
    [TestClass]
    public class EventControllerTests
    {
        private readonly Mock<IEventRepository> _mockRepo;
        private readonly Mock<ILogger<EventController>> _mockLogger;
        private readonly EventController _controller;
        private readonly Fixture _fixture;

        public EventControllerTests()
        {
            _mockRepo = new Mock<IEventRepository>();
            _mockLogger = new Mock<ILogger<EventController>>();
            _controller = new EventController(_mockRepo.Object, _mockLogger.Object);
            _fixture = new Fixture();
        }

        [TestMethod]
        public async Task GetEvents_ReturnsOkResult_WithListOfEvents()
        {
            // Arrange
            var testEvents = _fixture.CreateMany<Event>(2);
            _mockRepo.Setup(repo => repo.GetEvents()).ReturnsAsync(testEvents);

            // Act
            var result = await _controller.GetEvents();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as IEnumerable<Event>;
            Assert.AreEqual(2, returnValue.ToList().Count);
        }

        [TestMethod]
        public async Task GetEvent_ReturnsNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetEventById(It.IsAny<Guid>())).ReturnsAsync((Event)null);

            // Act
            var result = await _controller.GetEvent(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetEvent_ReturnsOkResult_WithEvent_WhenEventExists()
        {
            // Arrange
            var testEvent = _fixture.Create<Event>();
            _mockRepo.Setup(repo => repo.GetEventById(testEvent.EventId)).ReturnsAsync(testEvent);

            // Act
            var result = await _controller.GetEvent(testEvent.EventId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as Event;
            Assert.AreEqual(testEvent.Name, returnValue.Name);
        }
    }
}