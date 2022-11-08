using Internal.Services.Movements.ProxyClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Internal.Services.Movements.IntegrationTests.Utilities
{
    /// <summary>
    /// Manages the External API 
    /// </summary>
    internal class ExternalMovementsManager
    {
        /// <summary>
        /// Checks the parameters and retrieves filtered movements
        /// </summary>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="account">External Account Name</param>
        /// <param name="movementType">Movement Type</param>
        /// <param name="accountFrom">Sender IBAN</param>
        /// <param name="accountTo">Receiver IBAN</param>
        /// <param name="amountFrom">Amount sent from an account</param>
        /// <param name="amountTo">Amount sent to an account</param>
        /// <returns>Movements based on the filter</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<PagedMovements> GetMovements(int? pageNumber, int? pageSize, string? account, EnumMovementType? movementType, string? accountFrom, string? accountTo, decimal? amountFrom, decimal? amountTo)
        {
            if (pageNumber == null || pageSize == null)
            {
                throw new ArgumentNullException($"pageNumber or pageSize can not be null");
            }
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), nameof(pageSize), $"pageNumber or pageSize has not been set");
            }
            if (pageSize > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"pageSize can not be higher than 50");
            }
            var pagedMovements = new PagedMovements
            {
                PageSize = (int)pageSize,
                PageNumber = (int)pageNumber,
                Movements = getTestMovements((int)pageNumber, (int)pageSize, account, movementType, accountFrom, accountTo, amountFrom, amountTo)
            };
            return pagedMovements;
        }
        #region Private members
        /// <summary>
        /// Returns the movements based on the filter parameters
        /// </summary>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="account">External Account Name</param>
        /// <param name="movementType">Movement Type</param>
        /// <param name="accountFrom">Sender IBAN</param>
        /// <param name="accountTo">Receiver IBAN</param>
        /// <param name="amountFrom">Amount sent from an account</param>
        /// <param name="amountTo">Amount sent to an account</param>
        /// <returns>movements that are matching with the filter parameters</returns>
        private List<Movement> getTestMovements(int pageNumber, int pageSize, string? account, EnumMovementType? movementType, string? accountFrom, string? accountTo, decimal? amountFrom, decimal? amountTo)
        {
            var movements = allTestMovements();

            if (!string.IsNullOrWhiteSpace(account))
            {
                movements = movements.Where(x => x.Account == account);
            }

            if (movementType.HasValue)
            {
                movements = movements.Where(x => x.MovementType == movementType);
            }

            if (!string.IsNullOrWhiteSpace(accountFrom))
            {
                movements = movements.Where(x => x.AccountFrom == accountFrom);
            }

            if (!string.IsNullOrWhiteSpace(accountTo))
            {
                movements = movements.Where(x => x.AccountTo == accountTo);
            }

            if (amountFrom.HasValue)
            {
                movements = movements.Where(x => x.Amount >= amountFrom.Value);
            }

            if (amountTo.HasValue)
            {
                movements = movements.Where(x => x.Amount <= amountTo.Value);
            }

            var skip = (pageNumber - 1) * pageSize;

            return movements.Skip(skip).Take(pageSize).ToList();
        }
        /// <summary>
        /// Produces list of movements for testing
        /// </summary>
        /// <returns>List of movements</returns>
        private IEnumerable<Movement> allTestMovements()
        {
            var movements = new List<Movement>();

            var customerAccount = "NL54FAKE0062046111";
            var customerNominatedAccount = "NL96NMFK0208212218";
            var interestAccount = "SystemFakeInterestAccount";
            var feeAccount = "SystemFakeFeeAccount";
            var taxAccount = "SystemFakeTaxAccount";
            var fiscalTransferAccount = "NL54FAKE0326806738";
            var unknownAccount = "NL54FAKE0123456789";
            for (int i = 0; i < 10; i++)
            {
                var newMovements = new List<Movement>
                {
                    // System interest movement
                    new Movement
                    {
                        MovementId = i * 9 + 1000,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Interest,
                        Amount = (decimal)0.42 + i,
                        AccountFrom = interestAccount,
                        AccountTo = customerAccount
                    },
                    // System fee movement
                    new Movement
                    {
                        MovementId = i * 9 + 1001,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Fee,
                        Amount = (decimal)-0.59 - i,
                        AccountFrom = customerAccount,
                        AccountTo = feeAccount
                    },
                    // System tax movement
                    new Movement
                    {
                        MovementId = i * 9 + 1002,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Tax,
                        Amount = (decimal)-200.77 - i,
                        AccountFrom = customerAccount,
                        AccountTo = taxAccount
                    },
                    // Fiscal transfer movement
                    new Movement
                    {
                        MovementId = i * 9 + 1003,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Unknown,
                        Amount = (decimal)17000 + i,
                        AccountFrom = fiscalTransferAccount,
                        AccountTo = customerAccount
                    },
                    // Incoming movement
                    new Movement
                    {
                        MovementId = i * 9 + 1004,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Interest,
                        Amount = (decimal)500 + i,
                        AccountFrom = customerNominatedAccount,
                        AccountTo = customerAccount
                    },
                    // Outgoing movement
                    new Movement
                    {
                        MovementId = i * 9 + 1005,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Interest,
                        Amount = (decimal)-700 - i,
                        AccountFrom = customerAccount,
                        AccountTo = customerNominatedAccount
                    },
                    // Incoming System fee movement(Unexpected)
                    new Movement
                    {
                        MovementId = i * 9 + 1006,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Fee,
                        Amount = (decimal)+0.59 + i,
                        AccountFrom = feeAccount,
                        AccountTo = customerAccount
                    },
                    // Incoming System tax movement (Unexpected)
                    new Movement
                    {
                        MovementId = i * 9 + 1007,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Tax,
                        Amount = (decimal)+200.77 + i,
                        AccountFrom = taxAccount,
                        AccountTo = customerAccount
                    },
                    // Outgoing Unknown Movement 
                    new Movement
                    {
                        MovementId = i * 9 + 1008,
                        Account = customerAccount,
                        MovementType = EnumMovementType.Unknown,
                        Amount = (decimal)-17000 - i,
                        AccountFrom = customerAccount,
                        AccountTo = unknownAccount
                    }
                };
                movements.AddRange(newMovements);
            }

            return movements;
        }
        #endregion Private members
    }
}
