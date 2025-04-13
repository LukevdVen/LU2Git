using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


[TestClass]
public class Environment2DControllerTests
{
    private Mock<IEnvironmentRepository> _mockRepo;
    private Environment2DController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IEnvironmentRepository>();
        _controller = new Environment2DController(_mockRepo.Object);
    }

    [TestMethod]
    public async Task GetAll_UserWorldsExist_ReturnsOkResult()
    {
        // Arrange
        var userName = "testUser";
        var environments = new List<Environment2D>
        {
            new Environment2D { Id = 1, Naam = "World1", UserName = userName },
            new Environment2D { Id = 2, Naam = "World2", UserName = userName }
        };
        _mockRepo.Setup(repo => repo.GetAll(userName)).ReturnsAsync(environments);

        // Act
        var result = await _controller.GetAll(userName);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedList = okResult.Value as List<Environment2D>;
        Assert.AreEqual(2, returnedList.Count);
    }

    [TestMethod]
    public async Task GetAll_NoUserWorlds_ReturnsNoContent()
    {
        // Arrange
        var userName = "testUser";
        var environments = new List<Environment2D>();
        _mockRepo.Setup(repo => repo.GetAll(userName)).ReturnsAsync(environments);

        // Act
        var result = await _controller.GetAll(userName);

        // Assert
        var noContentResult = result.Result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task GetById_EnvironmentExists_ReturnsOkResult()
    {
        // Arrange
        var environmentId = 1;
        var environment = new Environment2D { Id = environmentId, Naam = "World1", UserName = "testUser" };
        _mockRepo.Setup(repo => repo.GetById(environmentId)).ReturnsAsync(environment);

        // Act
        var result = await _controller.GetById(environmentId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedEnvironment = okResult.Value as Environment2D;
        Assert.AreEqual(environmentId, returnedEnvironment.Id);
    }

    [TestMethod]
    public async Task GetById_EnvironmentNotFound_ReturnsNotFound()
    {
        // Arrange
        var environmentId = 1;
        _mockRepo.Setup(repo => repo.GetById(environmentId)).ReturnsAsync((Environment2D)null);

        // Act
        var result = await _controller.GetById(environmentId);

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [TestMethod]
    public async Task Add_InvalidEnvironment_ReturnsBadRequest()
    {
        // Arrange
        var environment = new Environment2D { Id = 1, Naam = "", UserName = "testUser" }; 
        _controller.ModelState.AddModelError("Name", "Name is required.");

        // Act
        var result = await _controller.Add(environment);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task Delete_EnvironmentExists_ReturnsOk()
    {
        // Arrange
        var environmentId = 1;
        _mockRepo.Setup(repo => repo.Delete(environmentId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(environmentId);

        // Assert
        var okResult = result as OkResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }
}
