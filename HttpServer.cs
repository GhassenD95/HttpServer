using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServerSimple;

public class HttpServer
{
    private const string BaseDirectory = "C:/Users/GhassenDhaoui/RiderProjects/HttpServerSimple/public";

    public HttpServer()
    {
        Router router = new Router();
        router.RegisterRoute("/", HandleRoot);
        router.RegisterRoute("/public/text.txt", HandleFiles);

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
        // Extract the file path from the request (e.g., /public/text.txt -> text.txt)
        string requestPath = tokens[1];
        string fileName = requestPath.Substring("/public/".Length); // Remove "/public/" from the path

        // Construct the full file path and normalize it to use forward slashes
        string filePath = Path.Combine(BaseDirectory, fileName).Replace('\\', '/');
        Console.WriteLine($"Serving file: {filePath}");

        if (File.Exists(filePath))
        {
            // Read the file content
            string content = File.ReadAllText(filePath);
            string contentType = GetContentType(filePath); // Determine the content type based on the file extension

            // Build the HTTP response
            string response = $"HTTP/1.1 200 OK\r\n"
                              + $"Content-Type: {contentType}\r\n"
                              + $"Content-Length: {content.Length}\r\n"
                              + "\r\n"
                              + content;

            // Send the response
            socket.Send(Encoding.ASCII.GetBytes(response));
        }
        else
        {
            // File not found
            string response = "HTTP/1.1 404 Not Found\r\n\r\n";
            socket.Send(Encoding.ASCII.GetBytes(response));
        }
    }

    private string GetContentType(string path)
    {
        string ext = Path.GetExtension(path);
        return ext switch
        {
            ".txt" => "text/plain",
            ".html" or ".htm" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".json" => "application/json",
            _ => "application/octet-stream" // Default for unknown file types
        };
    }
}