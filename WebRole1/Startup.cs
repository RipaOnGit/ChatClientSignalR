using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            GlobalHost.DependencyResolver.Register(
            typeof(ChatHub),
            () => new ChatHub(new DataBaseManager()));

            app.MapSignalR();
        }
    }
}
