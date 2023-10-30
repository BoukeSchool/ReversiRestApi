using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using ReversiRestApi.Controllers;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ReversiRestApi.DAL
{
    public class SpelAccessLayer : ISpelRepository
    {
        public static string connectionString = "Server=127.0.0.1;Database=ReversiDbContext;User id=sa;Password=Cardio1!;MultipleActiveResultSets=true";
        public void AddSpel(Spel spel)
        {
            SpelTbvJson serializableSpel = new SpelTbvJson(spel);
            var serialize = System.Text.Json.JsonSerializer.Serialize(serializableSpel);
            JObject jobject = JObject.Parse(serialize); 

            string query = "INSERT INTO Spel (Omschrijving, Speler1Token, Speler2Token, Token, AandeBeurt, Bord) values (@Omschrijving, @Speler1Token, @Speler2Token, @Token, @AandeBeurt, @Bord);";
            IDataParameter[] sqlParams = new IDataParameter[]
            {
                new SqlParameter("@Omschrijving", jobject["Omschrijving"].ToString()),
                new SqlParameter("@Speler1Token",jobject["Speler1Token"].ToString()),
                new SqlParameter("@Speler2Token",jobject["Speler2Token"].ToString()),
                new SqlParameter("@Token",jobject["Token"].ToString()),
                new SqlParameter("@Bord",jobject["Bord"].ToString()),
                new SqlParameter("@AandeBeurt",jobject["AandeBeurt"].ToString()),
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (sqlParams != null)
                        {
                            foreach (IDataParameter para in sqlParams)
                            {
                                cmd.Parameters.Add(para);
                            }
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public Spel GetSpel(string spelToken)
        {
            Spel spel = new Spel();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM Spel WHERE Token = @token;"))
                {
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("token", spelToken);
                    
                    using(SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            spel.ID = dr.GetInt32("ID");
                            spel.Omschrijving = dr["Omschrijving"].ToString();
                            spel.Speler1Token = dr["Speler1Token"].ToString();
                            spel.Speler2Token = dr["Speler2Token"].ToString();
                            spel.Token = dr["Token"].ToString();

                            int beurt = dr.GetInt32("AandeBeurt");
                            switch (beurt)
                            {
                                case 0:
                                    spel.AandeBeurt = Kleur.Geen;
                                    break;
                                case 1:
                                    spel.AandeBeurt = Kleur.Wit;
                                    break;
                                case 2:
                                    spel.AandeBeurt = Kleur.Zwart;
                                    break;
                            }

                            Debug.WriteLine(dr["Bord"].ToString().Equals(""));

                            if (!dr["Bord"].ToString().Equals(""))
                            {
                                spel.Bord = JsonConvert.DeserializeObject<Kleur[,]>(dr["Bord"].ToString());
                            }                           
                        }

                        dr.Close();
                    }
                }
            }
            return spel;
        }

        public List<Spel> GetSpellen()
        {
            List<Spel> returnList = new List<Spel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Spel;";

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Spel spel = new Spel();
                        spel.ID = dr.GetInt32("ID");
                        spel.Omschrijving = dr["Omschrijving"].ToString();
                        spel.Speler1Token = dr["Speler1Token"].ToString();
                        spel.Speler2Token = dr["Speler2Token"].ToString();
                        spel.Token = dr["Token"].ToString();
                        returnList.Add(spel);
                    }
                    dr.Close();
                }
            }
            return returnList;
        }

        public void DeleteSpel(string Token)
        {
            string query = "DELETE FROM Spel WHERE Token = @Token";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Token", Token);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void UpdateSpelerStats(string Token)
        {
            string query = "DELETE FROM Spel WHERE Token = @Token";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("Token", Token);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
