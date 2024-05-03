using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ECommereceApi.Services.classes
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if(eventData.Context is null)
            {
                return result;
            }
            foreach(var entry in eventData.Context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if(entry == null || entry.State != EntityState.Deleted || !(entry.Entity is ISoftDeletable))
                {
                    continue;
                }
                entry.State = EntityState.Modified;
                entry.Entity.Delete();
            }
            return base.SavingChanges(eventData, result);
        }
    }
}
