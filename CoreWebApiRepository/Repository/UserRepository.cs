using CoreWebApiRepository.IRepository;
using CoreWebApiRepository.Models;
using Dapper;
using Google.Apis.Util;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace CoreWebApiRepository.Repository
{
    public class UserRepository : IUserRepository
    {
        string _connectionString = "";
        User _oUser = new User();
        List<User> _oUsers = new List<User>();
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CoreWebApiRepository");
        }


        public async Task<string> Delete(User obj)
        {
            string message = string.Empty;
            try
            {
                using (IDbConnection con = new SqlConnection(_connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    var Users = await con.QueryAsync<User>("SP_User", AddParameters(obj, "Delete"),
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

        public async Task<User> Get(int objId)
        {
            _oUser = new User();

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var User = await con.QueryAsync<User>(@"Select * from [User] where UserId={0}", objId);
                if (User != null && User.Count() > 0)
                {
                    _oUser = User.SingleOrDefault();
                }
            }
            return _oUser;
        }

        public async Task<User> GetByUsernamePassword(User user)
        {
            _oUser = new User();

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var users = await con.QueryAsync<User>(string.Format(@"Select * from [dbo].[User] where Username='{0}' and Password='{1}'", user.UserName, user.Password));
                if (users != null && users.Count() > 0)
                {
                    _oUser = users.SingleOrDefault();
                }
            }
            return _oUser;
        }

        public async Task<List<User>> Gets()
        {
            _oUsers = new List<User>();

            using (IDbConnection con = new SqlConnection(_connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                var User = await con.QueryAsync<User>(@"Select * from [User]");
                if (User != null && User.Count() > 0)
                {
                    _oUsers = User.ToList();
                }
            }
            return _oUsers;
        }

        public async Task<User> Save(User obj)
        {
            _oUser = new User();
            try
            {
                var type = obj.UserId == 0 ? "insert" : "update";
                

                using (IDbConnection con = new SqlConnection(_connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    if (type == "insert")
                    {
                        obj.UserId = con.QueryFirstOrDefault<int>(@"SELECT COALESCE(MAX(UserId), 0) + 1 FROM [dbo].[User]");
                        
                    }
                    var User = await con.QueryAsync<User>("sp_user_info", AddParameters(obj, type),
                        commandType: CommandType.StoredProcedure);
                    if (User != null && User.Count() > 0)
                    {
                        _oUser = User.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _oUser= new User();
                _oUser.Message = ex.Message;
            }
            return _oUser;
        }

        private DynamicParameters AddParameters(User oUser, string type)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@userid", oUser.UserId);
            parameters.Add("@username", oUser.UserName);
            parameters.Add("@email", oUser.Email);
            parameters.Add("@password", oUser.Password);
            parameters.Add("@type", type);
            return parameters;

        }
    }
}
