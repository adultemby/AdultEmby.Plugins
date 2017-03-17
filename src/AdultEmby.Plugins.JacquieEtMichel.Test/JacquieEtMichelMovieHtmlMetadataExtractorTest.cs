using System;
using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.JacquieEtMichel;
using AdultEmby.Plugins.TestLogging;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.JacquieEtMichel.Test
{
    public class JacquieEtMichelMovieHtmlMetadataExtractorTest
    {
        [Fact]
        public void ShouldExtractTitle()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            String title = htmlMetadataExtractor.GetTitle(loadHtmlDocument());

            Assert.Equal("Alessandra, 22ans, prof de fitness russe !", title);
        }

        [Fact]
        public void ShouldExtractPlot()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            String plot = htmlMetadataExtractor.GetSynopsis(loadHtmlDocument());

            Assert.Equal("Venant tout droit de Moscou, Alessandra est professeur de fitness dans la capitale russe : elle fréquente donc assidûment les salles de sport. C'est par ce biais qu'elle a rencontré l'ami Rick, qui lui a proposé qu'elle lui prépare une séance personnalisée... qui va bien évidemment déraper par la suite !", plot);
        }

        [Fact]
        public void ShouldExtractGenres()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            List<string> genres = htmlMetadataExtractor.GetGenres(loadHtmlDocument());

            Assert.Equal("69", genres[0]);
            Assert.Equal("Blonde", genres[1]);
            Assert.Equal("Débutante", genres[2]);
        }

        [Fact]
        public void ShouldExtractStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(loadHtmlDocument());

            Assert.Equal("Jacquie Et Michel", studio);
        }

        [Fact]
        public void ShouldExtractDirector()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            MoviePerson director = htmlMetadataExtractor.GetDirector(loadHtmlDocument());

            Assert.Equal(null, director);
        }

        [Fact]
        public void ShouldExtractReleaseDate()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            DateTime? releaseDate = htmlMetadataExtractor.GetReleaseDate(loadHtmlDocument());

            Assert.Equal(new DateTime(2016, 10, 19), releaseDate);
        }

        [Fact]
        public void ShouldExtractProductionYear()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            int? productionYear = htmlMetadataExtractor.GetProductionYear(loadHtmlDocument());

            Assert.Equal(2016, productionYear);
        }

        [Fact]
        public void ShouldExtractSet()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetSet(loadHtmlDocument());

            Assert.Equal(null, set);
        }

        [Fact]
        public void ShouldExtractUpc()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            String upc = htmlMetadataExtractor.GetUpcCode(loadHtmlDocument());

            Assert.Equal(null, upc);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            String primaryImageUrl = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Equal("http://m.tv1.cdn.jetm-tech.net/cache/4a/db/4adb0409c4db46aa539e1c74cc8f06f5.jpg", primaryImageUrl);
        }

        [Fact]
        public void ShouldExtractActors()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new JacquieEtMichelMovieHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(loadHtmlDocument());

            Assert.Equal(0, actors.Count);
        }

        private IHtmlDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieResponse.html");
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
