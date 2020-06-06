using Ships_System.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ships_System.BL
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Product AddProduct(Product Product);
        Task<Product> UpdateProductAsync(Product Product);
        Task<bool> DeleteProductAsync(int ProductId);
    }
}
