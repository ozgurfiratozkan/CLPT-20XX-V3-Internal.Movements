using Internal.Services.Movements.ProxyClients;
using Moq;

namespace Internal.Services.Movements.IntegrationTests.Utilities
{
    public class MoqHelper
    {

        private readonly Mock<IMovementsClient> _movementsMock;

        public MoqHelper(Mock<IMovementsClient> movementsMock)
        {
            _movementsMock = movementsMock;
        }
    }
}