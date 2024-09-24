namespace GestionPersonnes.Models
{
    public class Personne
    {
        public int Id { get; set; }
        public required string Prenom { get; set; }
        public required string Nom { get; set; }
        public DateTime DateNaissance { get; set; }
        public List<Emploi> Emplois { get; set; } = new List<Emploi>();

        public int Age => DateTime.Now.Year - DateNaissance.Year;
    }
}
