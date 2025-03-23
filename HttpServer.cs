using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServerSimple;

public class HttpServer
{
    private readonly byte[]? _buffer;
    public HttpServer()
    {
        _buffer = new byte[1024];
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
            socket.Receive(_buffer); 
            StringBuilder builder = new StringBuilder();
            builder.Append(Encoding.ASCII.GetString(_buffer, 0, _buffer.Length));   
            String [] tokens = builder.ToString().Split(' ');

            if (tokens[0] == "GET" && tokens[1] == "/")
            {
                string happyPath = "HTTP/1.1 200 OK\r\n\r\n"; // CRLF sequence
                socket.Send(Encoding.ASCII.GetBytes(happyPath)); 
    
            }else if (tokens[0] == "GET")
            {
                string sadPath = "HTTP/1.1 404 NOT FOUND\r\n\r\n"; // CRLF sequence
                socket.Send(Encoding.ASCII.GetBytes(sadPath)); 
                
            }
            
            socket.Close();
        }
    }
}