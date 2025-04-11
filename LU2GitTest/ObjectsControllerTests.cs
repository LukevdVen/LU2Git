using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ObjectsControllerTests
{
    private Mock<IObjectRepository> _mockRepo;
    private ObjectsController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IObjectRepository>();
        _controller = new ObjectsController(_mockRepo.Object);
    }

    [TestMethod]
    public async Task GetObjects_ObjectsExist_ReturnsOkResult()
    {
        // Arrange
        var environmentId = 1;
        var objects = new List<ObjectDTO>
        {
            new ObjectDTO { EnvironmentId = environmentId, PrefabId = "Prefab1", PositionX = 0, PositionY = 0, ScaleX = 1, ScaleY = 1, RotationZ = 0 }
        };
        _mockRepo.Setup(repo => repo.GetObjectsByEnvironment(environmentId)).ReturnsAsync(objects);

        // Act
        var result = await _controller.GetObjects(environmentId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public async Task GetObjects_NoObjects_ReturnsOkEmptyList()
    {
        // Arrange
        var environmentId = 1;
        var objects = new List<ObjectDTO>(); 
        _mockRepo.Setup(repo => repo.GetObjectsByEnvironment(environmentId)).ReturnsAsync(objects);

        // Act
        var result = await _controller.GetObjects(environmentId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedList = okResult.Value as List<ObjectDTO>;
        Assert.AreEqual(0, returnedList.Count);
    }

    [TestMethod]
    public async Task SaveObjects_ValidObjects_ReturnsCreatedAtAction()
    {
        // Arrange
        var objectDTOs = new List<ObjectDTO>
        {
            new ObjectDTO { EnvironmentId = 1, PrefabId = "Prefab1", PositionX = 0, PositionY = 0, ScaleX = 1, ScaleY = 1, RotationZ = 0 }
        };
        _mockRepo.Setup(repo => repo.Add(It.IsAny<ObjectDTO>())).ReturnsAsync(objectDTOs[0]);

        // Act
        var result = await _controller.SaveObjects(objectDTOs);

        // Assert
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtActionResult);
        Assert.AreEqual(201, createdAtActionResult.StatusCode);
    }

    [TestMethod]
    public async Task SaveObjects_InvalidObjects_ReturnsBadRequest()
    {
        // Arrange
        var objectDTOs = new List<ObjectDTO>(); // Empty list, invalid
        _mockRepo.Setup(repo => repo.Add(It.IsAny<ObjectDTO>())).ReturnsAsync((ObjectDTO)null);

        // Act
        var result = await _controller.SaveObjects(objectDTOs);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteObjectsByEnvironment_Success_ReturnsOk()
    {
        // Arrange
        var environmentId = 1;
        _mockRepo.Setup(repo => repo.DeleteObjectsByEnvironment(environmentId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteObjectsByEnvironment(environmentId);

        // Assert
        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteObjectsByEnvironment_NotFound_ReturnsNotFound()
    {
        // Arrange
        var environmentId = 1;
        _mockRepo.Setup(repo => repo.DeleteObjectsByEnvironment(environmentId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteObjectsByEnvironment(environmentId);

        // Assert
        var notFoundResult = result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }
}
