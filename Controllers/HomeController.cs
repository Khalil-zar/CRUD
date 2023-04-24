using Microsoft.AspNetCore.Mvc;
using Project.Models;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public void OnGet() {
           
        }
        public ActionResult Index()
        {
            String connstring = "server=localhost;database=test;uid=root;pwd=";
            using (MySqlConnection connection = new MySqlConnection(connstring))
            {
                connection.Open();
                string query = "SELECT ID, Nom, Prenom FROM student";
                MySqlCommand command = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        int id = (int)reader["ID"];
                        string nom = (string)reader["Nom"];
                        string prenom = (string)reader["Prenom"];
                        Student stud = new Student { Id = id, Name = nom, Prenom = prenom };
                        students.Add(stud);
                    }
                    ViewData["Students"] = students;
                }
            } 
          return View();
        }
        [HttpPost]
        public IActionResult AjouterContact()

        {   string nom = Request.Form["nom"];
            string prenom = Request.Form["prenom"];
            // Insérer les données dans la base de données MySQL
            String connstring = "server=localhost;database=test;uid=root;pwd=";
            using (MySqlConnection connection = new MySqlConnection(connstring))
            {
                connection.Open();

                var commande = new MySqlCommand("INSERT INTO student (Nom, Prenom) VALUES (@Nom, @Prenom)", connection);
                commande.Parameters.AddWithValue("@nom", nom);
                commande.Parameters.AddWithValue("@prenom", prenom);
                commande.ExecuteNonQuery();
                connection.Close();


            }

            // Rediriger vers une autre vue
            return RedirectToAction("Index");
        }
        public IActionResult ListeEtudiants()
        {
            List<Project.Models.Student> etudiants = new List<Project.Models.Student>();
            string connectionString = "server=localhost;database=test;uid=root;pwd="; // Remplacez par votre propre chaine de connexion
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query = "SELECT id, nom, prenom FROM student";
            MySqlCommand command = new MySqlCommand(query, connection);
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Project.Models.Student etudiant = new Project.Models.Student();
                etudiant.Id = reader.GetInt32("id");
                etudiant.Name = reader.GetString("nom");
                etudiant.Prenom = reader.GetString("prenom");
                etudiants.Add(etudiant);
            }
            reader.Close();
            connection.Close();
            return View("ListEtudiant",etudiants);
        }
        public IActionResult Delete(int id)
        {
            using (MySqlConnection connection = new MySqlConnection("server=localhost;database=test;uid=root;pwd="))
            {
                string query = "DELETE FROM student WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        // La suppression a réussi
                    }
                    else
                    {
                        // L'objet à supprimer n'existe pas dans la base de données
                    }
                }
            }
            return RedirectToAction("ListeEtudiants");
        }
        [HttpPost]
        public IActionResult ModifierEtudiant(Project.Models.Student etudiant)
        {
            using (MySqlConnection connection = new MySqlConnection("server=localhost;database=test;uid=root;pwd="))
            {
                string query = "UPDATE student SET nom=@nom, prenom=@prenom WHERE id=@id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", etudiant.Name);
                    command.Parameters.AddWithValue("@prenom", etudiant.Prenom);
                    command.Parameters.AddWithValue("@id", etudiant.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("ListeEtudiants");
        }        
        public IActionResult GetData(int id)
        {
            using (MySqlConnection connection = new MySqlConnection("server=localhost;database=test;uid=root;pwd="))
            {
                string query = "SELECT nom,prenom FROM student WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    Project.Models.Student etudiant = new Project.Models.Student();
                    while (reader.Read())
                    {
                        etudiant.Name = reader.GetString("nom");
                        etudiant.Prenom = reader.GetString("prenom");
                    }
                    reader.Close();
                    connection.Close();
                    ViewBag.Etudiant = etudiant;
                    return View("ModifierEtudiant",etudiant);
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}