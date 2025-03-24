using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServerSimple;

public class HttpServer
{
    public HttpServer()
    {
        Router router = new Router();
        router.RegisterRoute("/", HandleRoot);
        
        var buffer = new byte[1024];
        Console.WriteLine("Starting HTTP server");
        
        //instance of class that listens to incoming connections on address/port
        var server = new TcpListener(IPAddress.Any, 6969);
        //starts listening
        server.Start();
        
        while (true)
        {
            //Accepts client request
            var socket = server.AcceptSocket();
            //read request data first 
            socket.Receive(buffer);
            var tokens = ParseRequest(buffer);
            //
            //
            router.HandleRequest(socket, tokens);            
            socket.Close();
        }
    }

    private static string[] ParseRequest(byte[] buffer)
    {
        var builder = new StringBuilder();
        builder.Append(Encoding.ASCII.GetString(buffer, 0, buffer.Length));   
        return builder.ToString().Split(' ');
    }
    
    
    private void HandleRoot(Socket socket, string[] tokens)
    {
        string response = "HTTP/1.1 200 OK\r\n"
                          + "Content-Type: text/html\r\n"
                          + "\r\n"
                          + "<html><body><h1>Welcome to My Server</h1></body></html>";
        socket.Send(Encoding.ASCII.GetBytes(response));
    }

    private void HandleFiles(Socket socket, string[] tokens)
    {
        
    }

}