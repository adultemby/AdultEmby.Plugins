using System.Collections.Generic;
using System.IO;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.JacquieEtMichel;
using AdultEmby.Plugins.TestLogging;
using AngleSharp;
using AngleSharp.Dom;

namespace AdultEmby.Plugins.JacquieEtMichel.Test
{
    public class JacquieEtMichelMovieHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractMovie()
        {
            JacquieEtMichelMovieHtmlSearchResultExtractor htmlSearchResultExtractor =
                new JacquieEtMichelMovieHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadDocument());

            Assert.Equal("2298/honey-chaleur-beninoise", results[0].Id);
            Assert.Equal("Honey, chaleur béninoise !", results[0].Name);
            Assert.Equal("http://www.jacquieetmicheltv.net/videos/show/2298/honey-chaleur-beninoise.html", results[0].Url);
            //Assert.Equal(2017, results[0].Year);
            Assert.Equal("http://m.tv1.cdn.jetm-tech.net/cache/e2/af/e2afa50ca4ecc1bea997ffc35fef91bc.jpg", results[0].ImageUrl);

        }

        private IDocument loadDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieSearchResponse.html");
            return BrowsingContext.New().OpenAsync(m => m.Content(responseStream).Status(200).Address(JacquieEtMichelConstants.BaseUrl)).Result;
            //Stream responseStream = File.OpenRead(@"TestResponses\MovieSearchResponse.html");
            //var parser = new HtmlParser();
            //return parser.Parse(document);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
