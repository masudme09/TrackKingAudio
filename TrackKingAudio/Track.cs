using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackKingAudio
{
    public class Track
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string AudioLoc { get; set; }
        public string Product { get; set; }

        public Track()
        {

        }

        public Track(int id, string Name,  string Category, string Product, string Brand, string AudioLoc)
        {
            this.id = id;
            this.Name = Name;
            this.Category = Category;
            this.Product = Product;
            this.Brand = Brand;
            this.AudioLoc = AudioLoc;
        }
    }
}
