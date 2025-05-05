using Microsoft.EntityFrameworkCore;
using Server.DataBaseFolder.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataBaseFolder.Querys
{
    internal class DBClient
    {
        static public async Task<bool> IsNew(string clientName)
        {
            using (var context = new DataBase())
            {
                var client = context.Client
                    .FirstOrDefault(c => c.ClientName == clientName);
                if (client == null)
                {return true;}
                else
                {return false;}               
            }
        }
        static public async Task<string> ISAvailable(string[] strings)
        {
            using (var context = new DataBase())
            {
                var client = context.Client
                    .FirstOrDefault(c => c.INN == strings[1]); 
                if (client != null)
                { return "Повторяется INN"; }

                client = context.Client
                    .FirstOrDefault(c => c.Email == strings[2]); 
                if (client != null)
                { return "Повторяется Email"; }

                client = context.Client
                   .FirstOrDefault(c => c.OGRN == strings[3]); 
                if (client != null)
                { return "Повторяется OGRN"; }

                client = context.Client
                   .FirstOrDefault(c => c.KPP == strings[4]);
                if (client != null)
                { return "Повторяется KPP"; }

                return null;
            }
        }
        static public Client FindClient(string[] strings)
        {
            using (var context = new DataBase())
            {
                var client = context.Client.FirstOrDefault(c => c.INN == strings[1] && c.Email == strings[2] &&
                c.OGRN == strings[3] && c.KPP == strings[4]);
                return client;
            }
        }

        static public Client FindClientByName(string ClientName)
        {
            using (var context = new DataBase())
            {
                var client = context.Client.FirstOrDefault(c => EF.Functions.Like(c.ClientName,"%"+ClientName+"_"));
                return client;
            }
        }

        static public void UpdateClientName(Client client, string name)
        {
            client.ClientName = name;
            using (var context = new DataBase())
            {
                context.Client.Update(client);
                context.SaveChanges();
            }
        }

       static public void NewClient(string[] strings)
        { 
            Client client = new Client();
            client.ClientName = strings[0];
            client.INN = strings[1];
            client.Email = strings[2];
            client.OGRN = strings[3];
            client.KPP = strings[4];

            using (var context = new DataBase())
            {
                context.Client.Add(client);
                context.SaveChanges();
            }
        }

    }
}
