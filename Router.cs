using System.Net.Sockets;
using System.Text;

namespace HttpServerSimple;

public class Router
{
    private Dictionary<string, Action<Socket, string[]>> _routes = new();

    public void RegisterRoute(string route, Action<Socket, string[]> handler)
    {
        _routes[route] = handler;
    }

    public void HandleRequest(Socket socket, string[] tokens)
    {
        string route = tokens[1];
        
        if (_routes.ContainsKey(route))
        {
            _routes[route](socket, tokens);
        }
        else
        {
            string response = "HTTP/1.1 404 Not Found\r\n\r\n";
            socket.Send(Encoding.ASCII.GetBytes(response));
        }
        
    }
    
}