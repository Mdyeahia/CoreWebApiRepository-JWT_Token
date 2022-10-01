using CoreWebApiRepository.Models;

namespace CoreWebApiRepository.IRepository
{
    public interface IProductRepository
    {
        string DataSaveOrUpdate(Product obj);
        Task<Product> Get(int objId);
        Task<List<Product>> Gets();
        Task<string> Delete(Product obj);

    }
}
