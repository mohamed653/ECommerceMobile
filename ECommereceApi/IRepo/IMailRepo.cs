namespace ECommereceApi.IRepo;

public interface IMailRepo
{
    bool TrySendEmail(string email, string code, string subject);
}
