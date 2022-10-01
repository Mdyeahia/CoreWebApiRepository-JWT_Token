using CoreWebApiRepository.IRepository;
using CoreWebApiRepository.Models;
using Dapper;
using Google.Apis.Util;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace CoreWebApiRepository.Repository
{
    public class ProductRepository : IProductRepository
    {
        string _connectionString = "";
        Product _oProduct = new Product();
        List<Product> _oProducts = new List<Product>();
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CoreWebApiRepository");
        }


        public async Task<string> Delete(Product obj)
        {
            string message = string.Empty;
            try
            {
                using (IDbConnection con = new SqlConnection(_connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    var Products = await con.QueryAsync<Product>("SP_Product", AddParameters(obj, "Delete"),
                        commandType: CommandType.StoredProcedure);
                    message = "Deleted";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public async Task<Product> Get(int objId)
        {
            _oProduct = new Product();

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var Product = await con.QueryAsync<Product>(@"Select * from [Product] where ProductId={0}", objId);
                if (Product != null && Product.Count() > 0)
                {
                    _oProduct = Product.SingleOrDefault();
                }
            }
            return _oProduct;
        }


        public async Task<List<Product>> Gets()
        {
            _oProducts = new List<Product>();

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var products = await con.QueryAsync<Product>(@"Select p.Name,c.Name CategoryName,p.Price,p.Description from [dbo].Product p inner join [dbo].Category c on c.categoryid=p.categoryid
");
                if (products != null && products.Count() > 0)
                {
                    _oProducts = products.ToList();
                }
            }
            return _oProducts;
        }

        public string  DataSaveOrUpdate(Product obj)
        {
            
            try
            {
                var type = obj.ProductId == 0 ? "insert" : "update";
                

                using (IDbConnection con = new SqlConnection(_connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    if (type == "insert")
                    {
                        obj.ProductId = con.QueryFirstOrDefault<int>(@"SELECT COALESCE(MAX(ProductId), 0) + 1 FROM [dbo].[Product]");
                        
                    }
                   con.Query<Product>("sp_Product_info", AddParameters(obj, type),
                        commandType: CommandType.StoredProcedure);
                 
                    return "Product Save Or Update Success";
                    
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
          
        }

        private DynamicParameters AddParameters(Product oProduct, string type)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@productid", oProduct.ProductId);
            parameters.Add("@categoryid", oProduct.CategoryId);
            parameters.Add("@name", oProduct.Name);
            parameters.Add("@price", oProduct.Price);
            parameters.Add("@description", oProduct.Description);
            parameters.Add("@type", type);
            return parameters;

        }

        public List<Category> AllCategory()
        {
            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                return (List<Category>)con.Query<List<Category>>(@"select * from [dbo].[Category]");
            }
        }
    }
}
