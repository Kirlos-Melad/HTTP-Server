using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(200);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.

                Socket clientSocket = this.serverSocket.Accept();

                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                ThreadPool.QueueUserWorkItem(HandleConnection, clientSocket);
                newThread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket _clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            _clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024];
                    int receivedLength = _clientSocket.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                        break;

                    // TODO: Create a Request object using received request string
                    string received = Encoding.ASCII.GetString(data, 0, receivedLength);
                    Request request_ = new Request(received);

                    // TODO: Call HandleRequest Method that returns the response
                    Response _response = HandleRequest(request_);
                    string response = _response.ResponseString;

                    // TODO: Send Response back to client
                    data = Encoding.ASCII.GetBytes(response);
                    _clientSocket.Send(data);

                    

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            _clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;
            Response response;
            string physicalPath;
            bool validReq = true;
            try
            {
                //TODO: check for bad request 
                validReq = request.ParseRequest();
                if (!validReq)
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    physicalPath = Configuration.RootPath + Configuration.BadRequestDefaultPageName;
                    response = new Response(StatusCode.BadRequest, "text/html", content, physicalPath);
                }
                else 
                {
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    physicalPath = Configuration.RootPath + request.relativeURI;

                    //TODO: check for redirect
                    string str = GetRedirectionPagePathIFExist(request.relativeURI);
                    if (str != string.Empty)
                    {
                        physicalPath = Configuration.RootPath + str;
                        content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                        response = new Response(StatusCode.Redirect, "text/html", content, physicalPath);
                    }
                    //TODO: check file exists
                    else if (!File.Exists(physicalPath))
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        physicalPath = Configuration.RootPath + Configuration.NotFoundDefaultPageName;
                        response = new Response(StatusCode.NotFound, "text/html", content, physicalPath);
                    }
                    else 
                    {
                        //TODO: read the physical file
                        content = LoadDefaultPage(physicalPath);

                        // Create OK response
                        response = new Response(StatusCode.OK, "text/html", content, physicalPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                physicalPath = Configuration.RootPath + Configuration.InternalErrorDefaultPageName;
                response = new Response(StatusCode.InternalServerError, "text/html", content, physicalPath);
                return response;
            }
            return response;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            foreach (KeyValuePair<string, string> kvp in Configuration.RedirectionRules)
            {
                if (relativePath == kvp.Key)
                {
                    return kvp.Value;
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            Exception ex = new Exception(); // what should the exception hold? is it correct ??
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            bool Exist = File.Exists(filePath);
            if (!Exist)
            {
                Logger.LogException(ex); // Is it right this way ??
                return string.Empty;
            }
            else
            {
                // else read file and return its content
                string content = File.ReadAllText(filePath);
                return content;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file
                string[] Rule = File.ReadAllLines(filePath);
                Dictionary<String, String> dict = Rule.ToDictionary(item => item.Split(',')[0], item => item.Split(',')[1]);
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules = dict;
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
