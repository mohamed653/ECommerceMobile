using ECommereceApi.IRepo;
using Microsoft.Extensions.Options;
using System.Xml;

namespace ECommereceApi.Repo
{
    public class LanguageRepo : ILanguageRepo
    {
        private readonly string _arResxFilePath = "Resources/Controllers/LanguageController.ar-EG.resx";
        private readonly string _enResxFilePath = "Resources/Controllers/LanguageController.en-US.resx";

        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;
        public LanguageRepo(IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions;
        }
        public void AddOrUpdateValue(string key, string value, string culture)
        {
            string resxFilePath = culture == "ar-EG" ? _arResxFilePath : _enResxFilePath;
            XmlDocument doc = new XmlDocument();
            doc.Load(resxFilePath);
            XmlNode node = doc.SelectSingleNode($"//data[@name='{key}']");
            if (node != null)
            {
                node.SelectSingleNode("value").InnerText = value;
            }
            else
            {
                XmlNode root = doc.DocumentElement;
                XmlElement dataElement = doc.CreateElement("data");
                dataElement.SetAttribute("name", key);
                XmlElement valueElement = doc.CreateElement("value");
                valueElement.InnerText = value;
                dataElement.AppendChild(valueElement);
                root.AppendChild(dataElement);
            }
            doc.Save(resxFilePath);
        }
        public void DeleteValue(string key, string culture)
        {
            string resxFilePath = culture == "ar-EG" ? _arResxFilePath : _enResxFilePath;
            XmlDocument doc = new XmlDocument();
            doc.Load(resxFilePath);
            XmlNode node = doc.SelectSingleNode($"//data[@name='{key}']");
            node?.ParentNode?.RemoveChild(node);
            doc.Save(resxFilePath);
        }
        public string GetValue(string key, string culture)
        {
            string resxFilePath = culture == "ar-EG" ? _arResxFilePath : _enResxFilePath;
            XmlDocument doc = new XmlDocument();
            doc.Load(resxFilePath);
            XmlNode node = doc.SelectSingleNode($"//data[@name='{key}']");
            return node?.SelectSingleNode("value")?.InnerText;
        }
        public IDictionary<string, string> GetAll(string culture)
        {
            string resxFilePath = culture == "ar-EG" ? _arResxFilePath : _enResxFilePath;
            XmlDocument doc = new XmlDocument();
            doc.Load(resxFilePath);
            XmlNodeList elemList = doc.GetElementsByTagName("data");
            var result = new Dictionary<string, string>();
            for (int i = 0; i < elemList.Count; i++)
            {
                XmlNode node = elemList[i];
                string key = node.Attributes["name"].Value;
                result[key] = node.SelectSingleNode("value")?.InnerText;
            }
            return result;
        }
    }
}
