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
        router.RegisterRoute("/post", HandlePost);

        Console.WriteLine("Starting HTTP server");

        //instance of class that listens to incoming connections on address/port
        var server = new TcpListener(IPAddress.Any, 6969);
        //starts listening
        server.Start();

        while (true)
        {
            var buffer = new byte[4096];

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

 private void HandlePost(Socket socket, string[] tokens)
{
    // The initial buffer already has the beginning of the request
    // We need to extract the Content-Length to know how much data to read
    string initialRequest = string.Join(" ", tokens);
    
    
    // Combine the initial tokens with the rest of the request
    string completeRequest = string.Join(" ", tokens);
    Console.WriteLine("=== Full HTTP Request ===");
    Console.WriteLine(completeRequest);
    
    // Split headers and body
    string[] parts = completeRequest.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
    string headers = parts[0];
    string body = parts.Length > 1 ? parts[1] : "";
    
    Console.WriteLine("=== Extracted Headers ===");
    Console.WriteLine(headers);
    
    Console.WriteLine("=== Extracted Body ===");
    Console.WriteLine(body);
    
    // Parse the form data
    Dictionary<string, string> formData = new Dictionary<string, string>();
    if (!string.IsNullOrEmpty(body))
    {
        string[] pairs = body.Split('&');
        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                formData[keyValue[0]] = keyValue[1];
            }
        }
    }
    
    // Build a nicer response showing the parsed data
    StringBuilder responseBodyBuilder = new StringBuilder();
    responseBodyBuilder.Append("<html><body>");
    responseBodyBuilder.Append("<h1>Received Form Data</h1>");
    responseBodyBuilder.Append("<table border='1'>");
    responseBodyBuilder.Append("<tr><th>Key</th><th>Value</th></tr>");
    
    foreach (var item in formData)
    {
        responseBodyBuilder.Append($"<tr><td>{item.Key}</td><td>{item.Value}</td></tr>");
    }
    
    responseBodyBuilder.Append("</table></body></html>");
    string responseBody = responseBodyBuilder.ToString();
    
    // Send response
    string response = "HTTP/1.1 200 OK\r\n"
                      + "Content-Type: text/html\r\n"
                      + $"Content-Length: {responseBody.Length}\r\n"
                      + "\r\n"
                      + responseBody;
    
    socket.Send(Encoding.ASCII.GetBytes(response));
}

}