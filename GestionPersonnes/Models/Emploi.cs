namespace GestionPersonnes.Models
{
    public class Emploi
    {
        public int Id { get; set; }
        public required string NomEntreprise { get; set; }
        public required string Poste { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int PersonneId { get; set; }
        public required Personne Personne { get; set; }
    }
}
