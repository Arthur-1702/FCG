using FiapCloudGames.Core.Entities;
using FiapCloudGames.Core.Interfaces.Repository;
using FiapCloudGames.Core.Utils;

namespace FiapCloudGames.Infrastructure.Repository;

public class UsuarioRepository(ApplicationDbContext context) : EFREpository<Usuario>(context), IUsuarioRepository
{
    public Usuario? GetPorEmail(string email) =>
        _dbSet.FirstOrDefault(entity => entity.Email == email);

    public Usuario? GetPorEmailESenha(string email, string senha) =>
        _dbSet.FirstOrDefault(entity => entity.Email == email &&
            entity.Senha == senha);

    public decimal ConferirSaldo(int id)
    {
        Usuario usuario = _dbSet.FirstOrDefault(entity => entity.Id == id);
        return (decimal)usuario.Saldo;
    }

    public decimal Depositar(int id, decimal valor)
    {
        Usuario usuario = GetPorId(id);
        usuario.Saldo = usuario.Saldo + valor;
        _dbSet.Update(usuario);
        _context.SaveChanges();
        return (decimal)usuario.Saldo;
    }

    public decimal Subtrair(int id, decimal valor)
    {
        Usuario usuario = GetPorId(id);
        usuario.Saldo = usuario.Saldo - valor;
        _dbSet.Update(usuario);
        _context.SaveChanges();
        return (decimal)usuario.Saldo;
    }

    public Usuario? Login(string email, string senhaTexto)
    {
        var usuario = _context.Set<Usuario>().FirstOrDefault(u => u.Email == email);

        if (usuario == null)
            return null;

        bool senhaValida = PasswordHelper.VerificarSenha(senhaTexto, usuario.Senha!);

        return senhaValida ? usuario : usuario;
    }
}