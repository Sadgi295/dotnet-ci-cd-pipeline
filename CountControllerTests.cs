using Moq;
using Microsoft.AspNetCore.Mvc;

namespace CounterAPI.Tests
{
    [TestClass]
    public class CountControllerTests
    {
        [TestMethod]
        public void Get_ShouldReturnIncrementedValue()
        {
            // Arrange
            var mockCounterService = new Mock<ICounterService>();
            mockCounterService.Setup(service => service.IncrementAndGet()).Returns(1);
            var controller = new CountController(mockCounterService.Object);

            // Act
            var result = controller.Get() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        public void Get_MultipleCalls_ShouldReturnIncrementedValues()
        {
            // Arrange
            var mockCounterService = new Mock<ICounterService>();
            mockCounterService.SetupSequence(service => service.IncrementAndGet())
                .Returns(1)
                .Returns(2)
                .Returns(3);
            var controller = new CountController(mockCounterService.Object);

            // Act & Assert
            var result1 = controller.Get() as OkObjectResult;
            Assert.IsNotNull(result1);
            Assert.AreEqual(200, result1.StatusCode);
            Assert.AreEqual(1, result1.Value);

            var result2 = controller.Get() as OkObjectResult;
            Assert.IsNotNull(result2);
            Assert.AreEqual(200, result2.StatusCode);
            Assert.AreEqual(2, result2.Value);

            var result3 = controller.Get() as OkObjectResult;
            Assert.IsNotNull(result3);
            Assert.AreEqual(200, result3.StatusCode);
            Assert.AreEqual(3, result3.Value);
        }
    }
}
