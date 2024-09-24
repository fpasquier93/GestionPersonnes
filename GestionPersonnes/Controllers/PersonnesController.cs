using GestionPersonnes.Data;
using GestionPersonnes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPersonnes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonnesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonnesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fonction qui ajoute une personne à la BD seulement si 
        /// elle a moins de 150 ans.
        /// </summary>
        /// <param name="personne"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AjouterPersonne(Personne personne)
        {
            var age = personne.Age;
            if (age > 150)
            {
                return BadRequest("La personne doit avoir moins de 150 ans.");
            }

            _context.Personnes.Add(personne);
            await _context.SaveChangesAsync();
            return Ok(personne);
        }

        /// <summary>
        /// Fonction qui sert à ajouter un emplois à une personne.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="emploi"></param>
        /// <returns></returns>
        [HttpPost("{id}/emplois")]
        public async Task<IActionResult> AjouterEmploi(int id, Emploi emploi)
        {
            var personne = await _context.Personnes.FindAsync(id);
            if (personne == null)
            {
                return NotFound("Personne introuvable.");
            }

            emploi.PersonneId = id;
            _context.Emplois.Add(emploi);
            await _context.SaveChangesAsync();
            return Ok(emploi);
        }

        /// <summary>
        /// Fonction qui retourne la liste des personnes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPersonnes()
        {
            var personnes = await _context.Personnes
                .Include(p => p.Emplois)
                .OrderBy(p => p.Nom)
                .ToListAsync();

            var result = personnes;

            return Ok(result);
        }

        /// <summary>
        /// Fonction qui retourne par ordre alphabétique de leurs nom puis prénom les personnes 
        /// ainsi que leur âge et emploi(s) actuel(s).
        /// </summary>
        /// <returns></returns>
        [HttpGet("current")]
        public async Task<IActionResult> GetPersonnesCurrentjobs()
        {
            var personnes = await _context.Personnes
                .Include(p => p.Emplois)
                .OrderBy(p => p.Nom)
                .ToListAsync();

            var result = personnes.Select(p => new
            {
                p.Nom,
                p.Prenom,
                p.Age,
                EmploisActuels = p.Emplois.Where(e => !e.DateFin.HasValue)
            });

            return Ok(result.OrderBy(p => p.Nom).ThenBy(p => p.Prenom));
        }

        /// <summary>
        /// Fonction qui renvoient toutes les personnes ayant travaillé pour une entreprise donnée.
        /// </summary>
        /// <param name="nomEntreprise">Entreprise</param>
        /// <returns></returns>
        [HttpGet("entreprise/{nomEntreprise}")]
        public async Task<IActionResult> GetPersonnesParEntreprise(string nomEntreprise)
        {
            var personnes = await _context.Personnes
                .Include(p => p.Emplois)
                .Where(p => p.Emplois.Any(e => e.NomEntreprise == nomEntreprise))
                .ToListAsync();

            return Ok(personnes);
        }

        /// <summary>
        /// Fonction qui renvoient tous les emplois d'une personne entre deux plages de dates.
        /// </summary>
        /// <param name="id">id de la personne concerné</param>
        /// <param name="debut">date de début</param>
        /// <param name="fin">date de fin</param>
        /// <returns></returns>
        [HttpGet("{id}/emplois")]
        public async Task<IActionResult> GetEmploisParDates(int id, [FromQuery] DateTime debut, [FromQuery] DateTime fin)
        {
            var personne = await _context.Personnes
                .Include(p => p.Emplois)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personne == null)
            {
                return NotFound("Personne introuvable.");
            }

            var emplois = personne.Emplois
                .Where(e => e.DateDebut >= debut && (e.DateFin == null || e.DateFin <= fin))
                .ToList();

            return Ok(emplois);
        }
    }
}
