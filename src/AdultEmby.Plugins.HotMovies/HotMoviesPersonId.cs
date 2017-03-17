using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesPersonId : IExternalId
    {
        public string Key => KeyName;

        public string Name=> "HotMovies";

        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }

        public string UrlFormatString => HotMoviesConstants.BaseUrl + "/porn-star/{0}";

        public static string KeyName=> "HotMoviesPerson";
    }
}
