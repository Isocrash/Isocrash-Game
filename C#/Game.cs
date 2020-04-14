using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raymarcher;
using Isocrash.Net;
using Isocrash.Net.Gamelogic;
using Raymarcher.Rendering;

namespace Isocrash
{
    class Game : Module
    {
        protected internal override void OnCreation()
        {
            return;
            Server.SetServerLogger(ServerLog, ServerLogWarn, ServerLogErr, ServerLogExc);
            Client.SetClientLogger(ClientLog, ClientLogWarn, ClientLogErr, ClientLogExc);

            Task.Run(() => CreateServerAsync());
        }

        private static void CreateServerAsync()
        {
            //Ticket.OnCreation += TicketRender;

            Server.UseAuth = false;
            Server.SetIntegrated(true);
            Server.Start("127.0.0.1", 26666);
        }

        private static void TicketRender(Ticket ticket)
        {
            Chunk c = ticket.Chunk;
            for (int z = 0; z < Chunk.Depth; z++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        if(ItemCache.Get(c[x, y, z]).Identifier != "isocrash:air")
                        {
                            Renderer.volumes.Add(
                            new C_VOLUME()
                            {
                                rotation = new C_QUATERNION(),
                                position = new C_VECTOR3(new Vector3D(ticket.Position.X * Chunk.Width + x, y, ticket.Position.Y + Chunk.Depth + z)),
                                scale = new C_VECTOR3(new Vector3D(1, 1, 1)),
                                type = volumeType.box
                            }
                        );
                        }
                    }
                }
            }      
        }


        #region Loggers
        private static void ServerLog(object obj)
        {
            Log.Print("server info: " + obj);
        }

        private static void ServerLogWarn(object obj)
        {
            Log.Print("server warning: " + obj);
        }

        private static void ServerLogErr(object obj)
        {
            Log.Print("server error: " + obj);
        }

        private static void ServerLogExc(object obj)
        {
            Log.Print("server exception: " + obj);
        }

        private static void ClientLog(object obj)
        {
            Log.Print("client info: " + obj);
        }

        private static void ClientLogWarn(object obj)
        {
            Log.Print("client warning: " + obj);
        }

        private static void ClientLogErr(object obj)
        {
            Log.Print("client error: " + obj);
        }

        private static void ClientLogExc(object obj)
        {
            Log.Print("client exception: " + obj);
        }
        #endregion
    }
}
