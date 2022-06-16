using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Web.Http.Cors;

namespace ReversiRestApi.Controllers
{

    public class alleSpellen
    {
        public List<SpelTbvJson> Spellen { get; set; }
    }

    
    [Route("api/Spel")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository iRepository;

        public SpelController(ISpelRepository repository)
        {
            iRepository = repository;
        }


        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            return Ok(iRepository.GetSpellen().Where(spel => spel.Speler2Token == "").Select(spel => spel.Omschrijving));
        }


        [HttpGet("Bord/{spelToken}")]
        public ActionResult<string> GetBordVanSpelToken(string spelToken)
        {
            try
            {
                Spel spel = iRepository.GetSpel(spelToken);
                string resultaat = new SpelTbvJson(spel).Bord;

                return Ok(resultaat);
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }


        [HttpGet("Omschrijving")]
        public ActionResult<IEnumerable<Spel>> GetSpelVanSpellenMetWachtendeSpeler()
        {
           
            try
            {
                List<SpelTbvJson> spellen = new List<SpelTbvJson>();

                IEnumerable<Spel> spel = iRepository.GetSpellen().Where(spel => spel.Speler2Token == "");

                foreach (var item in spel)
                {
                    SpelTbvJson resultaat = new SpelTbvJson(item);
                    spellen.Add(resultaat);
                }

                return Ok(spellen);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult PostSpel([FromBody] SpelInfoTbvApi spelInfo)
        {
            Spel nieuwSpel = new Spel();
            nieuwSpel.Speler1Token = spelInfo.SpelerToken;
            nieuwSpel.Omschrijving = spelInfo.SpelOmschrijving;
            iRepository.AddSpel(nieuwSpel);

            return Created(nameof(iRepository.GetSpel), Guid.NewGuid());
        }

        [HttpGet("SpelToken/{spelToken}")]
        public ActionResult<string> GetSpelVanSpelToken(string spelToken)
        {
            try
            {
                Spel spel = iRepository.GetSpel(spelToken);
                SpelTbvJson resultaat = new SpelTbvJson(spel);
                
                return Ok(resultaat);
            }
            catch(Exception e)
            { 
                return NotFound(e);
            }
        }

        [HttpGet("SpelerToken/{speler1Token}")]
        public ActionResult<Spel> GetSpelVanSpeler1Token(string speler1Token)
        {
            try
            {
                Spel spel = iRepository.GetSpellen().Where(spel => spel.Speler1Token == speler1Token).First();
                SpelTbvJson resultaat = new SpelTbvJson(spel);

                return Ok(JsonSerializer.Serialize(resultaat));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("Speler2Token/{speler2Token}")]
        public ActionResult<Spel> GetSpelVanSpeler2Token(string speler2Token)
        {
            try
            {
                Spel spel = iRepository.GetSpellen().Where(spel => spel.Speler2Token == speler2Token).First();
                SpelTbvJson resultaat = new SpelTbvJson(spel);

                return Ok(JsonSerializer.Serialize(resultaat));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPut("Deelnemen")]
        public IActionResult PutDeelnemen([FromBody] SpelInfoTbvHttpClient spelInfo)
        {
            Spel spel = iRepository.GetSpel(spelInfo.SpelToken);
            if(spel != null)
            {
                spel.Speler2Token = spelInfo.Speler2Token;
                iRepository.DeleteSpel(spelInfo.SpelToken);
                iRepository.AddSpel(spel);
            }
            else
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut("Zet")]
        public IActionResult PutZet([FromBody] SpelInfoTbvZet spelInfo)
        {
            try
            {
                string returnValue = "";
                Spel spel = iRepository.GetSpel(spelInfo.SpelToken);                
                spel.DoeZet(spelInfo.RijZet, spelInfo.KolomZet);
                if (spel.Afgelopen())
                {
                    returnValue = "Done";            
                }
                
                foreach (Kleur item in spel.Bord)
                {
                    Debug.WriteLine(item);
                }

                iRepository.DeleteSpel(spelInfo.SpelToken);
                iRepository.AddSpel(spel);

                return Ok(returnValue);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPut("Opgeven")]
        public IActionResult PutOpgeven(string spelToken, int rijZet, int kolomZet)
        {
            try
            {
                Spel spel = iRepository.GetSpel(spelToken);
                spel.DoeZet(rijZet, kolomZet);

                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet("Beurt/{spelToken}")]
        public ActionResult<Kleur> GetBeurt(string spelToken)
        {
            
            try
            {
                Spel spel = iRepository.GetSpel(spelToken);
                SpelTbvJson resultaat = new SpelTbvJson(spel);



                return Ok(JsonSerializer.Serialize(resultaat.AandeBeurt));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteSpelFromSpelerToken(string spelerToken)
        {
            try
            {
                Spel spel = iRepository.GetSpellen().Where(spel => spel.Speler1Token == spelerToken || spel.Speler2Token == spelerToken).First();
                iRepository.DeleteSpel(spel.Token);
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        // ...

    }
}
