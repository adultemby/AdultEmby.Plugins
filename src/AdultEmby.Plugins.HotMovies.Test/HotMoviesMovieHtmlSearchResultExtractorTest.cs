using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.HotMovies;
using AdultEmby.Plugins.TestLogging;

namespace AdultEmby.Plugins.HotMovies.Test
{
    public class HotMoviesMovieHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractMovie()
        {
            HotMoviesMovieHtmlSearchResultExtractor htmlSearchResultExtractor =
                new HotMoviesMovieHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadHtmlDocument());

            Assert.Equal("189246", results[0].Id);
            Assert.Equal("Russian Institute Lesson 14 - Anal Lesson (French)", results[0].Name);
            Assert.Equal("https://www.hotmovies.com/video/189246/Russian-Institute-Lesson-14-Anal-Lesson-French-/", results[0].Url);
            Assert.Equal(2010, results[0].Year);
            Assert.Equal("https://img2.vod.com/image2/cover/189/189246.cover.0.jpg", results[0].ImageUrl);

        }

        private IHtmlDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieSearchResponse.html");
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
