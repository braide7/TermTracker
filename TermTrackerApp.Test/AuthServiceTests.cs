using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using TermTrackerApp.Core.Services;
using TermTrackerApp.Core.Models;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp.Test
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<DatabaseService> _mockDatabaseService;
        private Mock<IUserRepository> _mockUserRepo;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize mocks
            _mockDatabaseService = new Mock<DatabaseService>("test.db");
            _mockUserRepo = new Mock<IUserRepository>();

            // Inject mock service into AuthService
            _authService = new AuthService(_mockUserRepo.Object);
        }

        [TestMethod]
        public async Task RegisterUser_UserDoesNotExist_ShouldRegisterSuccessfully()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetUser("testuser")).ReturnsAsync((User)null);
            _mockUserRepo.Setup(repo => repo.AddUser(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUser("testuser", "testpass");

            // Assert
            Assert.IsTrue(result);
            _mockUserRepo.Verify(repo => repo.AddUser(It.Is<User>(u => u.Username == "testuser")), Times.Once);
        }

        [TestMethod]
        public async Task RegisterUser_UserAlreadyExists_ShouldFail()
        {
            // Arrange
            var existingUser = new User { Username = "testuser", PasswordHash = "hashed" };
            _mockUserRepo.Setup(repo => repo.GetUser("testuser")).ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterUser("testuser", "testpass");

            // Assert
            Assert.IsFalse(result);
            _mockUserRepo.Verify(repo => repo.AddUser(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task LoginUser_CorrectCredentials_ShouldSucceed()
        {
            // Arrange
            string plainPassword = "testpass";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var user = new User { Id = 1, Username = "testuser", PasswordHash = hashedPassword };

            _mockUserRepo.Setup(repo => repo.GetUser("testuser")).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginUser("testuser", "testpass");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task LoginUser_InvalidPassword_ShouldFail()
        {
            // Arrange
            string correctPassword = "testpass";
            string wrongPassword = "wrongpass";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var user = new User { Id = 1, Username = "testuser", PasswordHash = hashedPassword };

            _mockUserRepo.Setup(repo => repo.GetUser("testuser")).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginUser("testuser", wrongPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task LoginUser_UserNotFound_ShouldFail()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetUser("unknown")).ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginUser("unknown", "testpass");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task LogoutUser_ShouldClearUserId()
        {
            // Act
            var result = await _authService.LogoutUser();

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(_authService.CurrentUserId);
        }
    }
}
