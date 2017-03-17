using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdPersonId : IExternalId
    {
        public string Key => KeyName;

        public string Name=> "IAFD";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }

        public string UrlFormatString => IafdConstants.BaseUrl + "person.rme/{0}";

        public static string KeyName=> "IafdPerson";
    }
}
