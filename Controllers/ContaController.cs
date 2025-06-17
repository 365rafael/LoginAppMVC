using LoginAppMVC.Data;
using LoginAppMVC.Helpers;
using LoginAppMVC.Models;
using LoginAppMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoginAppMVC.Controllers
{
    public class ContaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _email;

        public ContaController(AppDbContext context, EmailService email)
        {
            _context = context;
            _email = email;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Email == email);

            if (usuario != null && PasswordHelper.VerifyPassword(senha, usuario.Senha))
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToAction("Index", "Home");
            }

            TempData["MensagemErro"] = "E-mail ou senha inválidos.";
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(string nome, string email, string senha)
        {
            if (_context.Usuario.Any(u => u.Email == email))
            {
                TempData["MensagemErro"] = "Este e-mail já está cadastrado.";
                return View();
            }

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = PasswordHelper.HashPassword(senha),
                DataCadastro = DateTime.Now,
            };

            _context.Usuario.Add(usuario);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Cadastro realizado com sucesso! Faça seu login.";
            return RedirectToAction("Login");
        }

        public IActionResult EsqueciSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EsqueciSenha(string email)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Email == email);
            if (usuario == null)
            {
                TempData["MensagemErro"] = "E-mail não encontrado.";
                return View();
            }

            // Gerar token
            usuario.TokenRecuperacao = Guid.NewGuid().ToString();
            usuario.TokenExpiracao = DateTime.Now.AddHours(1);
            _context.SaveChanges();

            // Link
            var link = Url.Action("RedefinirSenha", "Conta", new { token = usuario.TokenRecuperacao }, Request.Scheme);

            // Enviar email
            _email.Enviar(
                usuario.Email,
                "Recuperação de Senha",
                $"<p>Olá {usuario.Nome},</p><p>Clique no link abaixo para redefinir sua senha:</p><p><a href='{link}'>Redefinir Senha</a></p><p>Este link é válido por 1 hora.</p>"
            );

            TempData["MensagemSucesso"] = "E-mail enviado! Verifique sua caixa de entrada.";
            return RedirectToAction("Login");
        }

        public IActionResult RedefinirSenha(string token)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.TokenRecuperacao == token);

            if (usuario == null || usuario.TokenExpiracao < DateTime.Now)
            {
                TempData["MensagemErro"] = "Token inválido ou expirado.";
                return RedirectToAction("Login");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public IActionResult RedefinirSenha(string token, string novaSenha)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.TokenRecuperacao == token);

            if (usuario == null || usuario.TokenExpiracao < DateTime.Now)
            {
                TempData["MensagemErro"] = "Token inválido ou expirado.";
                return RedirectToAction("Login");
            }

            usuario.Senha = PasswordHelper.HashPassword(novaSenha);
            usuario.TokenRecuperacao = null;
            usuario.TokenExpiracao = null;

            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Senha redefinida com sucesso. Faça seu login.";
            return RedirectToAction("Login");
        }


    }

}
