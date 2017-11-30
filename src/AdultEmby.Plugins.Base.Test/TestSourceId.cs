using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.Base.Test
{
    public class TestSourceId : IExternalId
    {
        public string Key=> KeyName;

        public string Name => "TestSource";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie || item is Person;
        }

        public string UrlFormatString => TestSourceConstants.BaseUrl + "{0}";

        public static string KeyName => "TestSource";
    }
}
