using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi
{
    public class SpelRepository : ISpelRepository
    {
        // Lijst met tijdelijke spellen
        public List<Spel> Spellen { get; set; }
        public SpelRepository()
        {

        }
        public void AddSpel(Spel spel)
        {
            Spellen.Add(spel);
        }
        public List<Spel> GetSpellen()
        {
            return Spellen;
        }
        public Spel GetSpel(string spelToken)
        {
            return Spellen.Where(spel => spel.Token.Equals(spelToken)).First();
        }

        public void DeleteSpel(string Speltoken)
        {

        }
    }
}
