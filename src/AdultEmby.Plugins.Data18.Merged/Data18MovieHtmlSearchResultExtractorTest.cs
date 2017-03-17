using System;
using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Data18;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.Test.Logging;

namespace AdultEmby.Plugins.Test.Data18
{
    public class Data18MovieHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractMovie()
        {
            Data18MovieHtmlSearchResultExtractor htmlSearchResultExtractor =
                new Data18MovieHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadHtmlDocument());

            Assert.Equal("movies/1151324", results[0].Id);
            Assert.Equal("Anal Lessons #4", results[0].Name);
            Assert.Equal("http://www.data18.com/movies/1151324", results[0].Url);
            Assert.Equal(new DateTime(2016, 11, 7), results[0].PremiereDate);
            Assert.Equal(2016, results[0].Year);
            Assert.Equal("http://img.data18.com/covers/1/4/29491_front.jpg", results[0].ImageUrl);

        }

        //[Fact]
        public void ShouldExtractContent()
        {
            Data18MovieHtmlSearchResultExtractor htmlSearchResultExtractor =
                new Data18MovieHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadHtmlDocument());

            results.Reverse();

            Assert.Equal("content/124038", results[0].Id);
            Assert.Equal("Anal Lessons", results[0].Name);
            Assert.Equal("http://www.data18.com/content/124038", results[0].Url);
            Assert.Equal("Site: Asshole Fever, Network: 21sextury, Cast: Judith Fox, Imogene", results[0].Overview);
            Assert.Equal(new DateTime(2005, 12, 29), results[0].PremiereDate);
            Assert.Equal("http://94.229.67.74/42/799/24038/hor2.jpg", results[0].ImageUrl);

        }

        private IHtmlDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\Data18\MovieSearchResponse.html");
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
