using App.Domains.Core.Api;
using App.Domains.Network.Impl;
using App.Domains.Network.Impl.Messages;
using NSubstitute;
using NUnit.Framework;

namespace App.Tests.Domains.Network
{
    public class NetworkServiceTests
    {
        [Test]
        public void SendMessage_WhenSocketIsNotConnected_LogsError()
        {
            // Arrange
            ILogger logger = Substitute.For<ILogger>();
            using NetworkService service = new NetworkService(logger);

            // Act
            service.SendMessage(new MessageWrapper
            {
                Type = (int)MessageType.Ready,
                Data = "{}",
            });

            // Assert
            logger.Received(1).LogError("Web socket is not connected");
        }
    }
}
