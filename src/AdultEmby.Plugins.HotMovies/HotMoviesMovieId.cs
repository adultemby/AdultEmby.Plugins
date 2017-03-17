using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesMovieId : IExternalId
    {
        public string Key=> KeyName;

        public string Name => "HotMovies";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }

        public string UrlFormatString => HotMoviesConstants.BaseUrl + "video/{0}";

        public static string KeyName => "HotMoviesMovie";
    }
}
