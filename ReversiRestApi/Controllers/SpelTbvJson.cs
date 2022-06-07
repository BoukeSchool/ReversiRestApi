using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;

namespace ReversiRestApi.Controllers
{
    public class SpelTbvJson
    {
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }

        public string Bord { get; set; }
        public Kleur AandeBeurt { get; set; }

        public SpelTbvJson(Spel spel)
        {
            ID = spel.ID;
            Omschrijving = spel.Omschrijving;
            Token = spel.Token;
            Speler1Token = spel.Speler1Token;
            Speler2Token = spel.Speler2Token;

            Bord = JsonConvert.SerializeObject(spel.Bord); 

            AandeBeurt = spel.AandeBeurt;

        }
    }
}
