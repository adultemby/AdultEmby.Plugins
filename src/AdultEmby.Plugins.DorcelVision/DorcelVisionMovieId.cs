using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionMovieId : IExternalId
    {
        public string Key=> KeyName;

        public string Name => "Dorcel Vision"; 

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }

        public string UrlFormatString => DorcelVisionConstants.BaseUrl + "fr/films/{0}";

        public static string KeyName => "DorcelVisionMovie";
    }
}