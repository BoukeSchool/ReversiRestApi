using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi
{
    public interface ISpelRepository
    {
        void AddSpel(Spel spel);
        public List<Spel> GetSpellen();
        Spel GetSpel(string spelToken);

        void DeleteSpel(string spelToken);

    }
}
