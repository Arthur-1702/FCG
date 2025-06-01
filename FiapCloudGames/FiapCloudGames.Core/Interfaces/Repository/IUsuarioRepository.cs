using FiapCloudGames.Core.Entities;

namespace FiapCloudGames.Core.Interfaces.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario GetPorEmail(string email);
        Usuario GetPorEmailESenha(string email, string senha);
        Usuario Login(string email, string senhaTexto);
        decimal ConferirSaldo(int id);
        decimal Depositar(int id, decimal valor);
        decimal Subtrair(int id, decimal valor);        
    }
}
