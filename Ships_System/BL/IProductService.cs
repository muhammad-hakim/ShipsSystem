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
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        Product AddProduct(Product Product);
        Product UpdateProduct(Product Product);
        bool DeleteProduct(int ProductId);
        bool CheckUniqueness(Product product);
        bool CanDelete(int productId);
    }
}
