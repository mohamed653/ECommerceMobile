namespace ECommereceApi.IRepo
{
    public interface ILanguageRepo
    {
        string GetValue(string key, string culture);
        void AddOrUpdateValue(string key, string value, string culture);
        void DeleteValue(string key, string culture);
        IDictionary<string, string> GetAll(string culture);
    }
}
