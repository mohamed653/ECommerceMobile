namespace ECommereceApi.Services.Interfaces
{
    public interface ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public void Delete()
        {
            IsDeleted = true;
            DateDeleted = DateTime.Now;
        }
        public void UnDelete()
        {
            IsDeleted = false;
        }
    }
}
