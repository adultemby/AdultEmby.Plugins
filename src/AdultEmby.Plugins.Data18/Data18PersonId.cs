using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.Data18
{
    public class Data18PersonId : IExternalId
    {
        public string Key => KeyName;

        public string Name => "Data 18";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }

        public string UrlFormatString => Data18Constants.BaseUrl + "{0}";

        public static string KeyName => "Data18Person";
    }
}
