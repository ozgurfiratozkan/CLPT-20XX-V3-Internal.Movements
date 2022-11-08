using Internal.Services.Movements.Data.Contexts;
using Internal.Services.Movements.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Internal.Services.Movements.IntegrationTests.Utilities
{
	public class DbHelper
	{
		private readonly MovementsDataContext _movementsDb;

		public DbHelper(MovementsDataContext movementsDb)
		{
			_movementsDb = movementsDb;
		}
		/// <summary>
		/// Creates a Product and ProductCustomer in InMemoryDb for a user
		/// </summary>
		public void InitializeDbForTests()
		{
			if(!_movementsDb.Products.Any())
			{
				var products = new List<Product>
				{
					new Product
					{
						ProductId =1,
						ExternalAccount=AccountHelper.CustomerAccount,
						ProductType=Data.Models.Enums.EnumProductType.SavingsRetirement,

					}
				};
				_movementsDb.Products.AddRange(products);
				_movementsDb.SaveChanges();
			}
            if (!_movementsDb.ProductsCustomers.Any())
            {
                var productCustomers = new List<ProductCustomer>
                {
                    new ProductCustomer
                    {
                        ProductCustomerId =1,
                        ProductId=1,
                        CustomerId=1,

                    }
                };
                _movementsDb.ProductsCustomers.AddRange(productCustomers);
                _movementsDb.SaveChanges();
            }
        }
	}
}
