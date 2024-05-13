namespace ECommereceApi.Repo
{
	public enum Status
	{
		Success,
		Failed,
		NotFound,
        EmailExistsBefore
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
