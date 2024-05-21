namespace ECommereceApi.IRepo
{
    public interface IGenericRepo<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> Add(T entity);
        Task<T> Delete(int id);
        Task Save();
    }
}
