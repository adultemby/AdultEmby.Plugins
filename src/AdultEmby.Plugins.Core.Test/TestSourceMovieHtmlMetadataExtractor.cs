using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using MediaBrowser.Model.Logging;

namespace AdultEmby.Plugins.Core.Test
{
    public class TestSourceMovieHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        public TestSourceMovieHtmlMetadataExtractor(ILogManager logManager)
        {
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return "TITLE";
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            return "SYNOPSIS";
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            return new List<string>()
            {
                "GENRE1",
                "GENRE2"
            };
        }

        public string GetStudio(IDocument htmlDocument)
        {
            return "STUDIO";
        }

        public DateTime? GetReleaseDate(IDocument htmlDocument)
        {
            return null;
        }

        public int? GetProductionYear(IDocument htmlDocument)
        {
            return 2010;
        }

        public MoviePerson GetDirector(IDocument htmlDocument)
        {
            return new MoviePerson()
            {
                Id = "DIRECTOR_ID",
                Name = "DIRECTOR_NAME"                
            };
        }

        public string GetUpcCode(IDocument htmlDocument)
        {
            return "UPC_CODE";
        }

        public string GetPrimaryImageUrl(IDocument htmlDocument)
        {
            
            return "IMAGE_URL";
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            return new List<MoviePerson>()
            {
                new MoviePerson
                    {
                        Id = "ACTOR1_ID",
                        Name ="ACTOR1_NAME"
                    },
                new MoviePerson
                    {
                        Id = "ACTOR2_ID",
                        Name = "ACTOR2_NAME"
                    }
            };
        }

        public string GetSet(IDocument htmlDocument)
        {
            return "SET";
        }
    }
}
