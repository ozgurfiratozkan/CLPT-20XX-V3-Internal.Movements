using Internal.Services.Movements.Business.Manager;
using Internal.Services.Movements.Data.Contexts;
using Internal.Services.Movements.IntegrationTests.Utilities;
using Internal.Services.Movements.ProxyClients;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Internal.Services.Movements.IntegrationTests
{
    public class IntegrationTest : IClassFixture<TestStartup<Program>>
    {

        private readonly TestStartup<Program> _factory;
        private readonly Utilities.MoqHelper _moq;
        private readonly HttpClient _client;
        private ExternalMovementsManager _externalManager;

        public IntegrationTest(TestStartup<Program> factory)
        {
            _factory = factory;
            _moq = new Utilities.MoqHelper(factory.MovementMock);
            _client = _factory.CreateClient();
            _externalManager = new ExternalMovementsManager();

        }
        #region Tests
        /// <summary>
        /// This test case verifies that only Fiscal transactions are fetched
        /// Verifies that Receiver is the account
        /// Verifies that Amount is positive
        /// Verifies the total number of transactions with the type of FiscalTransfer
        /// Verifies that MovementId is one of a kind
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Fetch_Fiscal_Transactions_Only()
        {
            //Set parameters
            var pageNumber = 1;
            var pageSize = 50;
            var account = AccountHelper.CustomerAccount;
            var internalFilterType = Data.Models.Enums.EnumMovementType.FiscalTransfer;
            //Execute Fetch
            var result = FetchMovements(internalFilterType, pageNumber, pageSize, account, null, AccountHelper.FiscalTransferAccount, null, null, null);
            //Verify
            Assert.True(result.Result.Movements.All(x => (x.AccountFrom == AccountHelper.FiscalTransferAccount) && x.AccountTo == account), "Receiver Account Is not the user account Or Sender is user Account");
            Assert.True(result.Result.Movements.All(x => (x.Amount >= 0)), $"Amount is not positive for movement");
            Assert.True(result.Result.Movements.Count == 10, "Unexpected number of movements received.");//Can be removed if there is a chance to change external database to prevent additional adjustments
            Assert.True(result.Result.Movements.DistinctBy(x => x.MovementId).Count() == 10, "Same MovementId is detected.");//Can be removed if there is a chance to change external database to prevent adjustments
        }
        /// <summary>
        /// This test case verifies that only Interest transactions are fetched
        /// Verifies that Movement type is Interest
        /// Verifies that Either To or From account is user account
        /// Verifies the total number of transactions with the type of Interest
        /// Verifies that MovementId is one of a kind
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Fetch_Interest_Transactions_Only()
        {
            //Set parameters
            var pageNumber = 1;
            var pageSize = 50;
            var account = AccountHelper.CustomerAccount;
            var internalFilterType = Data.Models.Enums.EnumMovementType.Interest;
            //Execute Fetch
            var result = FetchMovements(internalFilterType, pageNumber, pageSize, account, EnumMovementType.Interest, null, null, null, null);
            //Verify
            Assert.True(result.Result.Movements.All(x => (x.AccountFrom == account) || x.AccountTo == account), "User Account Is not Receiver or Sender");
            Assert.True(result.Result.Movements.All(x => (x.MovementType == EnumMovementType.Interest)), $"Movement Type is not interest");
            Assert.True(result.Result.Movements.Count == 30, "Unexpected number of movements received.");//Can be removed if there is a chance to change external database to prevent adjustments
            Assert.True(result.Result.Movements.DistinctBy(x => x.MovementId).Count() == 30, "Same MovementId is detected.");//Can be removed if there is a chance to change external database to prevent adjustments

        }
        /// <summary>
        /// This test case verifies that only Incoming transactions are fetched
        /// Verifies that Receiver is the account
        /// Verifies that Amount is positive
        /// Verifies the total number of transactions with the type of Income
        /// Verifies that MovementId is one of a kind
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Fetch_Incoming_Transactions_Only()
        {
            //Set parameters
            var pageNumber = 1;
            var pageSize = 50;
            var account = AccountHelper.CustomerAccount;
            var internalFilterType = Data.Models.Enums.EnumMovementType.Incoming;
            //Execute Fetch
            var result = FetchMovements(internalFilterType, pageNumber, pageSize, account, null, null, null, 0, null);
            //Verify
            Assert.True(result.Result.Movements.All(x => (x.AccountFrom != account) && x.AccountTo == account), "Receiver Account Is not the user account Or Sender is user Account");
            Assert.True(result.Result.Movements.All(x => (x.Amount >= 0)), $"Amount is not positive for movement");
            Assert.True(result.Result.Movements.Count == 50, "Unexpected number of movements received.");//Can be removed if there is a chance to change external database to prevent adjustments
            Assert.True(result.Result.Movements.DistinctBy(x => x.MovementId).Count() == 50, "Same MovementId is detected.");//Can be removed if there is a chance to change external database to prevent adjustments
        }
        #endregion Tests
        #region Common
        /// <summary>
        /// Mocks the external API and returns the movements based on filterType, pageSize, pageNumber, etc.
        /// </summary>
        /// <param name="filterType">Internal Movement type to be filtered</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="account">External Account Name</param>
        /// <param name="movementType">External Movement Type</param>
        /// <param name="accountFrom">Sender IBAN</param>
        /// <param name="accountTo">Receiver IBAN</param>
        /// <param name="amountFrom">Amount sent from an account</param>
        /// <param name="amountTo">Amount sent to an account</param>
        /// <returns>Result that includes list of movements</returns>
        private async Task<PagedMovements>? FetchMovements(Data.Models.Enums.EnumMovementType? filterType, int? pageNumber, int? pageSize, string account, EnumMovementType? movementType, string accountFrom, string accountTo, decimal? amountFrom, decimal? amountTo)
        {
            //Mock Ext API and Result
            _factory.MovementMock.Setup(x => x.GetMovementsAsync(pageNumber, pageSize, account, movementType, accountFrom, accountTo, amountFrom, amountTo))
                .Returns(_externalManager.GetMovements(pageNumber, pageSize, account, movementType, accountFrom, accountTo, amountFrom, amountTo));
            //Prepare DB
            var builder = new DbContextOptionsBuilder<MovementsDataContext>().UseInMemoryDatabase("InMemoryDbForTesting");
            var configured = builder.IsConfigured;
            MovementsDataContext db = new MovementsDataContext(builder.Options);
            db.Database.EnsureCreated();
            //Get Movements from Mocked API
            MovementsManager _manager = new MovementsManager(_factory.MovementMock.Object, db);
            var result = _manager.GetMovements(1, filterType, 1, (int)pageSize);
            return result.Result;
        }
        #endregion Common

    }
}