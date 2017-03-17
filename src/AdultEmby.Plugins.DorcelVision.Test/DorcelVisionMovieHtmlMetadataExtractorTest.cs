using System;
using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.DorcelVision;
using AdultEmby.Plugins.TestLogging;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.DorcelVision.Test
{
    public class DorcelVisionMovieHtmlMetadataExtractorTest
    {
        [Fact]
        public void ShouldExtractTitle()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            String title = htmlMetadataExtractor.GetTitle(loadHtmlDocument());

            Assert.Equal("La Transporteuse", title);
        }

        [Fact]
        public void ShouldExtractPlot()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            String plot = htmlMetadataExtractor.GetSynopsis(loadHtmlDocument());

            Assert.Equal("Kimber et son garde du corps Jason forment le duo de choc d'une agence sensée régler les problèmes sexuels des couples. A bord de leur bolide, ils partent pour des missions sans réellement savoir ce qui les attend. Entre action, fellations et éjaculations, cette parodie pornographique va vous couper le souffle", plot);
        }

        [Fact]
        public void ShouldExtractGenres()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            List<string> genres = htmlMetadataExtractor.GetGenres(loadHtmlDocument());

            Assert.Equal("Anal", genres[0]);
            Assert.Equal("Cunnilingus", genres[1]);
            Assert.Equal("Levrette", genres[2]);
            Assert.Equal("Levrette debout", genres[3]);
            Assert.Equal("Missionnaire", genres[4]);
        }

        [Fact]
        public void ShouldExtractStudio()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            String studio = htmlMetadataExtractor.GetStudio(loadHtmlDocument());

            Assert.Equal("Fred Coppula Prod", studio);
        }

        [Fact]
        public void ShouldExtractDirector()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            MoviePerson director = htmlMetadataExtractor.GetDirector(loadHtmlDocument());

            Assert.Equal(null, director);
        }

        [Fact]
        public void ShouldExtractReleaseDate()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            DateTime? releaseDate = htmlMetadataExtractor.GetReleaseDate(loadHtmlDocument());

            Assert.Equal(null, releaseDate);
        }

        [Fact]
        public void ShouldExtractProductionYear()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            int? productionYear = htmlMetadataExtractor.GetProductionYear(loadHtmlDocument());

            Assert.Equal(2015, productionYear);
        }

        [Fact]
        public void ShouldExtractSet()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            String set = htmlMetadataExtractor.GetSet(loadHtmlDocument());

            Assert.Equal(null, set);
        }

        [Fact]
        public void ShouldExtractUpc()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            String upc = htmlMetadataExtractor.GetUpcCode(loadHtmlDocument());

            Assert.Equal(null, upc);
        }

        [Fact]
        public void ShouldExtractPrimaryImageUrl()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            String primaryImageUrl = htmlMetadataExtractor.GetPrimaryImageUrl(loadHtmlDocument());

            Assert.Equal("https://www.dorcelvision.com/images/endless3/703161.jpg", primaryImageUrl);
        }

        [Fact]
        public void ShouldExtractActors()
        {
            IHtmlMetadataExtractor htmlMetadataExtractor =
                new DorcelVisionMovieHtmlMetadataExtractor(LogManager());

            List<MoviePerson> actors = htmlMetadataExtractor.GetActors(loadHtmlDocument());

            validateActor("Kimber Délice", "actrices-x/kimber-delice", actors[0]);
            validateActor("Lyna Cypher", "actrices-x/lyna-cypher", actors[1]);
            validateActor("Charlotte", "actrices-x/charlotte", actors[2]);
            validateActor("Rico Simmons", "acteurs-x/rico-simmons", actors[3]);
            validateActor("Pascal St James", "acteurs-x/pascal-st-james", actors[4]);
            validateActor("Josh", "acteurs-x/josh", actors[5]);
            validateActor("Jason StatX", "acteurs-x/jason-statx", actors[6]);
        }

        private void validateActor(string name, string id, MoviePerson actor)
        {
            Assert.Equal(id, actor.Id);
            Assert.Equal(name, actor.Name);
        }

        private IDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieResponse.html");
            return BrowsingContext.New().OpenAsync(m => m.Content(responseStream).Status(200).Address(DorcelVisionConstants.BaseUrl)).Result;
            //Stream responseStream = File.OpenRead(@"TestResponses\MovieResponse.html");
            //var parser = new HtmlParser();
            //return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
