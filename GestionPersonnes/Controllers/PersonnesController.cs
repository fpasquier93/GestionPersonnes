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

        [HttpPost]
        public async Task<IActionResult> AjouterPersonne([FromBody] Personne personne)
        {
            var age = DateTime.Now.Year - personne.DateNaissance.Year;
            if (age > 150)
            {
                return BadRequest("La personne doit avoir moins de 150 ans.");
            }

            _context.Personnes.Add(personne);
            await _context.SaveChangesAsync();
            return Ok(personne);
        }

        [HttpPost("{id}/emplois")]
        public async Task<IActionResult> AjouterEmploi(int id, [FromBody] Emploi emploi)
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

        [HttpGet]
        public async Task<IActionResult> GetPersonnes()
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

            return Ok(result);
        }

        [HttpGet("entreprise/{nomEntreprise}")]
        public async Task<IActionResult> GetPersonnesParEntreprise(string nomEntreprise)
        {
            var personnes = await _context.Personnes
                .Include(p => p.Emplois)
                .Where(p => p.Emplois.Any(e => e.NomEntreprise == nomEntreprise))
                .ToListAsync();

            return Ok(personnes);
        }

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
