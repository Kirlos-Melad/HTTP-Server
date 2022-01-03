using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        private string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }

        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // Create response string

            // Adding status line
            responseString = GetStatusLine(code);

            // Adding header lines
            List<string> headerLines = GetHeaderLines(contentType, content.Length, redirectoinPath);
            foreach (string line in headerLines)
                responseString += line;

            // Adding the content
            responseString += content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion + ' ';
            statusLine += ((int)code).ToString() + ' ' + code.ToString() + "\r\n";

            return statusLine;
        }

        private List<string> GetHeaderLines(string contentType, int contentLength, string redirectoinPath)
        {
            List<string> headerLines = new List<string>();
            string crlf = "\r\n";

            // Add headlines(Content-Type, Content-Length, Date, [location if there is redirection])
            string contentTypeString = "Content-Type: " + contentType + crlf;
            string contentLengthString = "Content-Length: " + contentLength.ToString() + crlf;
            string dateString = "Date: " + DateTime.Now.ToString("R") + crlf; // R is the format given in the example
            string locationString = "Location: ";
            string serverTypeString = "Server: " + Configuration.ServerType + crlf;

            // add location if a redirection is done
            if (redirectoinPath.Length != 0)
                headerLines.Add(locationString + redirectoinPath + crlf);

            headerLines.Add(dateString);
            headerLines.Add(serverTypeString);
            headerLines.Add(contentLengthString);
            headerLines.Add(contentTypeString);

            // add blank line to mark the end of header lines
            headerLines.Add(crlf);

            return headerLines;
        }
    }
}
