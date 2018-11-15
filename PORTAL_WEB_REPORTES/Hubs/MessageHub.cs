using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Casuarinas.Helpers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Model;

namespace Casuarinas.Hubs
{
    [HubName("messageHub")]
    public class MessageHub: Hub
    {
        private Notificacion mNotificacion = new Notificacion();

        //Diccionario para almacenar las sesiones
        static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

        public void send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        public void update(string userId)
        {
            try
            {
                if (dic.ContainsKey(userId))
                {
                    int number = mNotificacion.obtenerNumPendMessages(int.Parse(userId));
                    if (number > 0)
                    {
                        Clients.Client(dic[userId]).updateNumberMessages(number);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void getMyNotifications(string userId)
        {
            if (dic.ContainsKey(userId))
            {
                int number = mNotificacion.obtenerNumPendMessages(int.Parse(userId));
                if (number > 0)
                {
                    Clients.Caller.updateNumberMessages(number);
                }
            }
        }

        public void Notify(string id)
        {
            string userId = SessionHelper.GetUser().ToString();
            if (!dic.ContainsKey(userId))
            {
                dic.TryAdd(userId, id);
            }
        }

        public override Task OnDisconnected()
        {
            var name = dic.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
            
            if (name.Key != null)
            {
                string s;
                dic.TryRemove(name.Key, out s);
                return Clients.All.disconnected(name.Key);
            }
            else
            {
                return null;
            }
        }  
    }
}