namespace LoginAppMVC.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; } = false; // 🔑 Define se é administrador
        public string? TokenRecuperacao { get; set; }
        public DateTime? TokenExpiracao { get; set; }

    }
}
