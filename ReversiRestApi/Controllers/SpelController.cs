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
                if(spel.ID == 0)
                {
                    return NotFound();
                } 
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

        //[HttpGet("Speler2Token/{speler2Token}")]
        //public ActionResult<Spel> GetSpelVanSpeler2Token(string speler2Token)
        //{
        //    try
        //    {
        //        Spel spel = iRepository.GetSpellen().Where(spel => spel.Speler2Token == speler2Token).First();
        //        SpelTbvJson resultaat = new SpelTbvJson(spel);

        //        return Ok(JsonSerializer.Serialize(resultaat));
        //    }
        //    catch
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpGet("IsUserInGame/{spelerToken}")]
        public ActionResult<Spel> GetSpelInGame(string spelerToken)
        {
            try
            {
                Spel spel = iRepository.GetSpellen().Where(spel => spel.Speler2Token == spelerToken || spel.Speler1Token == spelerToken).First();
                SpelTbvJson resultaat = new SpelTbvJson(spel);

                return Ok(resultaat);
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
                Debug.WriteLine("----------------------------------------------------");
                string debugValue = "";
                foreach(var item in spel.Bord)
                {
                    debugValue += item;
                }
                Debug.WriteLine(debugValue);
                spel.DoeZet(spelInfo.RijZet, spelInfo.KolomZet);
                if (spel.Afgelopen())
                {
                    returnValue = "Done";            
                }

                //foreach (kleur item in spel.bord)
                //{
                //    debug.writeline(item);
                //}

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
        public IActionResult PutOpgeven([FromBody] SpelInfoTbvHttpClient spelInfo)
        {
            try
            {
                //hier is spelInfo.Speler2Token de token van de speler die opgeeft
                Spel spel = iRepository.GetSpel(spelInfo.SpelToken);
                //als speler 1 opgeeft
                if (spel.Speler1Token.Equals(spelInfo.Speler2Token))
                {
                    //als er ook al een speler 2 in zit
                    if (!spel.Speler2Token.Equals(String.Empty))
                    {
                        //dan wordt speler 2 verplaats naar de plek van speler 1 en de plek voor de speler 2 token wordt leeggehaald
                        spel.Speler1Token = spel.Speler2Token;
                        spel.Speler2Token = String.Empty;
                    }
                    else
                    {
                        spel.Speler1Token = String.Empty;
                    }
                }
                //als speler 2 opgeeft
                else if (spel.Speler2Token.Equals(spelInfo.Speler2Token))
                {
                    //dan wordt de speler2token leeggehaald
                    spel.Speler2Token = "";

                }

                iRepository.DeleteSpel(spel.Token);

                if (!spel.Speler1Token.Equals(""))
                {
                    iRepository.AddSpel(spel);
                }

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
        public IActionResult DeleteSpelFromSpelerTokenIfExists(string spelerToken)
        {
            try
            {
                Spel spel = iRepository.GetSpellen().Where(spel => spel.Speler1Token == spelerToken || spel.Speler2Token == spelerToken).First();
                //if(spel != null)
                //{
                //    iRepository.DeleteSpel(spel.Token);
                //}
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
