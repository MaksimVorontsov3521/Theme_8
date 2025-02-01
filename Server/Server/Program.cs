using Server;
using System.Net.Sockets;


public class Program
{
    int port = 8080;
    TcpListener server;
    static void Main(string[] args)
    {
        Program program = new Program();
        program.Setup();
        program.Connection();    
    }

    private void Setup()
    {
        server = new TcpListener(port);
        server.Start();       
    }
    private void Connection()
    {
        Thread thread = new Thread(() => HandelClient());
        Console.WriteLine("123");
        thread.Start();
    }

    private void HandelClient()
    {     
        using (var client = server.AcceptTcpClient())
        {
            Connection();
            // Создаем поток для чтения
            using (var stream = client.GetStream())
            {
                // Читаем массив байтов из сокета
                byte[] bytes = new byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    Console.WriteLine("Читанные байты: " + BitConverter.ToString(bytes, 0, bytesRead));
                }
            }

        }
    }
}


