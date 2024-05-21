using ECommereceApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Repo
{
    public class GenericRepo<T>:IGenericRepo<T> where T : class
    {
        private readonly ECommerceContext _context;
        public GenericRepo(ECommerceContext context)
        {
            _context = context;
        }
        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<T> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return entity;
            }
            _context.Set<T>().Remove(entity);
            return entity;
        }

        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
