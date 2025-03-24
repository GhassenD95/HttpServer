HTTP Server Simple
==================

A lightweight, custom HTTP server implementation in C# designed to demonstrate the fundamentals of HTTP communication and socket programming.

Project Overview
----------------

HTTP Server Simple is a minimalist web server built from scratch using C#'s socket programming capabilities. This project was created to understand the inner workings of HTTP servers without relying on high-level frameworks like ASP.NET.

The server can handle basic HTTP requests including:

*   Serving static HTML content

*   Serving files from a specified directory

*   Processing form submissions via POST requests


Key Features
------------

*   **Custom TCP Socket Implementation**: Uses .NET's Socket and TcpListener classes to handle incoming connections

*   **Simple Routing System**: A custom router that maps URL paths to handler functions

*   **Static File Serving**: Ability to serve different file types with appropriate Content-Type headers

*   **Form Data Processing**: Parses and displays form data submitted via POST requests

*   **Minimal Dependencies**: Built using only .NET base libraries


Architecture
------------

The project consists of two main components:

1.  **HttpServer Class**: Manages the server lifecycle, including:

    *   Setting up the TCP listener

    *   Accepting socket connections

    *   Reading HTTP requests

    *   Parsing request data

    *   Sending HTTP responses

2.  **Router Class**: Implements a simple routing system that:

    *   Registers route handlers

    *   Maps incoming requests to the appropriate handler

    *   Provides a 404 response for unregistered routes


Code Example
------------

From the Router class:


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


How to Use
----------

1.  Clone the repository

2.  Open the solution in Visual Studio or JetBrains Rider

3.  Update the BaseDirectory constant in HttpServer.cs to point to your desired public folder

4.  Build and run the application

5.  The server will start on port 6969 by default

6.  Access the server via http://localhost:6969/


Testing the Server
------------------

You can test the server using:

1.  Web browser - for GET requests

2.  HTTP clients like Postman or REST Client - for POST requests and more advanced testing

3.  Sample POST request using REST Client format:


Plain textANTLR4BashCC#CSSCoffeeScriptCMakeDartDjangoDockerEJSErlangGitGoGraphQLGroovyHTMLJavaJavaScriptJSONJSXKotlinLaTeXLessLuaMakefileMarkdownMATLABMarkupObjective-CPerlPHPPowerShell.propertiesProtocol BuffersPythonRRubySass (Sass)Sass (Scss)SchemeSQLShellSwiftSVGTSXTypeScriptWebAssemblyYAMLXML`   ### POST request with form data  POST http://localhost:6969/post  Content-Type: application/x-www-form-urlencoded  username=john&password=12345  ###   `

Technical Challenges and Learning Outcomes
------------------------------------------

Throughout this project, I gained experience with:

*   Socket programming in C#

*   HTTP protocol details including request/response structure

*   MIME types and content handling

*   Parsing and processing form data

*   Error handling in network applications

*   Building a custom routing system


Future Improvements
-------------------

While this server demonstrates the core concepts of HTTP communication, several enhancements could be made:

*   Support for concurrent connections

*   Improved request parsing (headers, query parameters)

*   Session management

*   HTTP/2 support

*   WebSocket capabilities

*   Security improvements (HTTPS, input validation)