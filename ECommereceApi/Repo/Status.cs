namespace ECommereceApi.Repo
{
	public enum Status
	{
		Success,
		Failed,
		NotFound,
        EmailExistsBefore,
        Deleted,
        ExistedBefore
    }

	public enum SortType
	{
        ASC,
        DESC
    }
	public enum UserOrderBy
	{
        Name,
        Email,
        Date
    }
}
