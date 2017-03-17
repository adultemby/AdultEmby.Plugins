using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionPersonId : IExternalId
    {
        public string Key => KeyName;

        public string Name => "DorcelVision";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }

        public string UrlFormatString => DorcelVisionConstants.BaseUrl + "/{0}";

        public static string KeyName => "DorcelVisionPerson";
    }
}
