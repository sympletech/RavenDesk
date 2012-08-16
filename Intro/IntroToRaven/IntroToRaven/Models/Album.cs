using System.ComponentModel;
using System.Collections.Generic;

namespace IntroToRaven.Models
{
    public class Album
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string AlbumArtUrl { get; set; }
        public int CountSold { get; set; }

        //public int GenreId { get; set; }
        //public int ArtistId { get; set; }
        //public virtual Genre Genre { get; set; }
        public virtual Artist Artist { get; set; }
    }
}