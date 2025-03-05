using Server;
using Server.DataBase;
using Server.DataBase.DbContexts;
using System.Net.Sockets;
using System.Text;


public class Program
{
    int port = 8080;
    TcpListener server;
    static void Main(string[] args)
    {
        Program program = new Program();
        program.Setup(); 
    }
    private void Setup()
    {
        server = new TcpListener(port);
        server.Start();
        Connection();
    }
    private void Connection()
    {
        Thread thread = new Thread(() => HandelClient());
        thread.Start();
    }
    private void HandelClient()
    {     
        using (var client = server.AcceptTcpClient())
        {          
            // Создаем поток для чтения
            using (var stream = client.GetStream())
            {
                // Читаем массив байтов из сокета
                byte[] bytes = new byte[1024];
                int bytesRead;              
                while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    Console.WriteLine("Читанные байты: " + Encoding.UTF8.GetString(bytes, 0, bytesRead));
                }
                DataBase dataBase = new DataBase();
                LoginPassword loginPassword = new LoginPassword(dataBase);
                long a = loginPassword.Login("йЦУКЕ", "1234");
                if (a > 0)
                { Console.WriteLine("Entered"); }
                else { }
            }
        }

    }
}


