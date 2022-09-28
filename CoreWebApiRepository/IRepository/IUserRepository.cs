using CoreWebApiRepository.Models;

namespace CoreWebApiRepository.IRepository
{
    public interface IUserRepository
    {
        Task<User> Save(User obj);
        Task<User> Get(int objId);
        Task<List<User>> Gets();
        Task<User> GetByUsernamePassword(User user);
        Task<string> Delete(User obj);

    }
}
