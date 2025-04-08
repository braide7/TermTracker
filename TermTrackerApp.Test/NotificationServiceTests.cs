using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Plugin.LocalNotification;
using TermTrackerApp.Core.Services;
using TermTrackerApp.Core.Services;

namespace TermTrackerApp.Tests.Services
{
    [TestClass]
    public class NotificationServiceTests
    {
        private Mock<ILocalNotificationCenter> _mockNotificationCenter;
        private INotifyService _notificationService;

        [TestInitialize]
        public void Setup()
        {
            _mockNotificationCenter = new Mock<ILocalNotificationCenter>();
            _notificationService = new NotificationService(_mockNotificationCenter.Object);
        }

        [TestMethod]
        public async Task SetNotification_OnSameDay_SetsNotificationForCurrentTime()
        {
            // Arrange
            var date = DateTime.Now.Date;
            var id = 123;
            var title = "Test Title";
            var description = "Test Description";

            NotificationRequest capturedRequest = null;
            _mockNotificationCenter
                .Setup(nc => nc.Show(It.IsAny<NotificationRequest>()))
                .Callback<NotificationRequest>(request => capturedRequest = request)
                .ReturnsAsync(true);

            // Act
            await _notificationService.SetNotification(date, id, title, description);

            // Assert
            _mockNotificationCenter.Verify(nc => nc.Show(It.IsAny<NotificationRequest>()), Times.Once);
            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(id, capturedRequest.NotificationId);
            Assert.AreEqual(title, capturedRequest.Title);
            Assert.AreEqual(description, capturedRequest.Description);
            Assert.AreEqual("Test data", capturedRequest.ReturningData);

            // Check that time is set for current time (with some tolerance for test execution time)
            var now = DateTime.Now;
            var notifyTime = capturedRequest.Schedule.NotifyTime;
            Assert.AreEqual(now.Date, notifyTime.Value.Date);
            Assert.IsTrue((now - notifyTime.Value).TotalMinutes < 2,
                $"Expected time near {now}, but was {notifyTime}");
        }

        [TestMethod]
        public async Task SetNotification_OnFutureDay_SetsNotificationFor7AM()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1).Date;  // Tomorrow
            var id = 456;
            var title = "Future Title";
            var description = "Future Description";

            NotificationRequest capturedRequest = null;
            _mockNotificationCenter
                .Setup(nc => nc.Show(It.IsAny<NotificationRequest>()))
                .Callback<NotificationRequest>(request => capturedRequest = request)
                .ReturnsAsync(true);

            // Act
            await _notificationService.SetNotification(date, id, title, description);

            // Assert
            _mockNotificationCenter.Verify(nc => nc.Show(It.IsAny<NotificationRequest>()), Times.Once);
            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(id, capturedRequest.NotificationId);
            Assert.AreEqual(title, capturedRequest.Title);
            Assert.AreEqual(description, capturedRequest.Description);

            // Check that time is set for 7:00:00 AM
            var notifyTime = capturedRequest.Schedule.NotifyTime;
            Assert.AreEqual(date, notifyTime.Value.Date);
            Assert.AreEqual(7, notifyTime.Value.Hour);
            Assert.AreEqual(0, notifyTime.Value.Minute);
            Assert.AreEqual(0, notifyTime.Value.Second);
        }

        [TestMethod]
        public async Task CancelNotification_WhenNotificationExists_CancelsNotification()
        {
            // Arrange
            int id = 789;
            var pendingNotifications = new List<NotificationRequest>
            {
                new NotificationRequest { NotificationId = id }
            };

            _mockNotificationCenter
                .Setup(nc => nc.GetPendingNotificationList())
                .ReturnsAsync(pendingNotifications);

            _mockNotificationCenter
                .Setup(nc => nc.Cancel(id))
                .ReturnsAsync(true);

            // Act
            await _notificationService.CancelNotification(id);

            // Assert
            _mockNotificationCenter.Verify(nc => nc.GetPendingNotificationList(), Times.Once);
            _mockNotificationCenter.Verify(nc => nc.Cancel(id), Times.Once);
        }

        [TestMethod]
        public async Task CancelNotification_WhenNotificationDoesNotExist_DoesNotCallCancel()
        {
            // Arrange
            int id = 999;
            var pendingNotifications = new List<NotificationRequest>
            {
                new NotificationRequest { NotificationId = 123 }
            };

            _mockNotificationCenter
                .Setup(nc => nc.GetPendingNotificationList())
                .ReturnsAsync(pendingNotifications);

            // Act
            await _notificationService.CancelNotification(id);

            // Assert
            _mockNotificationCenter.Verify(nc => nc.GetPendingNotificationList(), Times.Once);
            _mockNotificationCenter.Verify(nc => nc.Cancel(It.IsAny<int>()), Times.Never);
        }
    }
}