using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SIR.Autenticacion.Api.Notificaciones
{
    public class NotificacionHub : Hub<INotificacionHub>
    {
        private static IHubContext<NotificacionHub, INotificacionHub> hubContext;
        private readonly static MapaConecciones<string> _conecciones = new MapaConecciones<string>();
        public NotificacionHub(IHubContext<NotificacionHub, INotificacionHub> hubContext){
            NotificacionHub.hubContext = hubContext;
        }
        public static void NotificarCliente(string who, string message)
        { 
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(who);
            string usrb64 = System.Convert.ToBase64String(plainTextBytes);
            foreach (var connectionId in _conecciones.ObtenerConeccion(usrb64))
            {
                hubContext.Clients.Client(connectionId).NotificarCliente(message);
            }
        }
        public override Task OnConnectedAsync()
        {            
            string usrb64 = Context.GetHttpContext().Request.Query["dXNy"];
            _conecciones.Agregar(usrb64, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exp)
        {
            string usrb64 = Context.GetHttpContext().Request.Query["dXNy"];
            _conecciones.Quitar(usrb64, Context.ConnectionId);
            return base.OnDisconnectedAsync(exp);
        }        

    }
}