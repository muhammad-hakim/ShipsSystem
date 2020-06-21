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

        public bool CanDelete(int productId)
        {
            return unitOfWork.TripsLoads.Get().FirstOrDefault(l => l.ProductId == productId) == null;
        }

        public bool CheckUniqueness(Product product)
        {
            if (product.ProductId == 0) //adding
            {
                if (unitOfWork.Products.Get().FirstOrDefault(a => a.Name == product.Name) != null)
                    return false;
            }
            else //editing
            {
                var result = unitOfWork.Products.Get().Where(a => a.Name == product.Name);
                if (result.Count() > 1)
                    return false;
                else
                if (result.Count() == 1 && result.FirstOrDefault(a => a.ProductId == product.ProductId) == null)
                    return false;
            }
            return true;
        }

        public bool DeleteProduct(int productId)
        {
            return  unitOfWork.Products.Delete(productId);
        }

        public List<Product> GetAllProducts()
        {
            return  unitOfWork.Products.Get();
        }

        public Product GetProductById(int id)
        {
            return  unitOfWork.Products.GetById(id);
        }

        public Product UpdateProduct(Product product)
        {
            return  unitOfWork.Products.Update(product.ProductId, product);
        }
    }
}
