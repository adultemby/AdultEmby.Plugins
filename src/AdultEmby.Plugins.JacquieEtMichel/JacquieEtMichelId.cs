using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace AdultEmby.Plugins.JacquieEtMichel
{
    public class JacquieEtMichelId : IExternalId
    {
        public string Key=> KeyName; 

        public string Name => "Jacquie Et Michel";

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }

        public string UrlFormatString => JacquieEtMichelConstants.BaseUrl + "videos/show/{0}.html";

        public static string KeyName => "JacquieEtMichel";
    }
}