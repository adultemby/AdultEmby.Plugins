using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdMovieId : IExternalId
    {
        public string Key=> KeyName;

        public string Name => "IAFD";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }

        public string UrlFormatString => IafdConstants.BaseUrl + "title.rme/{0}";

        public static string KeyName => "IafdMovie";
    }
}
