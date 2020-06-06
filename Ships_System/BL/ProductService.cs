using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public class ProductService:IProductService
    {
        private readonly UnitOfWork unitOfWork;

        public ProductService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Product AddProduct(Product product)
        {
            return unitOfWork.Products.Add(product);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            return await unitOfWork.Products.DeleteAsync(productId);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await unitOfWork.Products.GetAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            return await unitOfWork.Products.UpdateAsync(product.ProductId, product);
        }

    }
}
