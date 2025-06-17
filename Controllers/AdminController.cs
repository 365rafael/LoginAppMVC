using LoginAppMVC.Data;
using LoginAppMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginAppMVC.Controllers
{
    [Authorize] // Protege o acesso apenas para usuários logados (pode melhorar com roles futuramente)
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _db.Usuario.OrderBy(u => u.Nome).ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var usuario = await _db.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _db.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _db.Usuario.Update(usuario);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Excluir(int id)
        {
            var usuario = await _db.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            _db.Usuario.Remove(usuario);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirConfirmed(int id)
        {
            var usuario = await _db.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            _db.Usuario.Remove(usuario);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
