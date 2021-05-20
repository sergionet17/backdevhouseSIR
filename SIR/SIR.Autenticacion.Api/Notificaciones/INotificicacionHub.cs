using System.Threading.Tasks;

namespace SIR.Autenticacion.Api.Notificaciones{
    public interface INotificacionHub{
        Task NotificarCliente(string mensaje);
    }
}