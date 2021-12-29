using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;   // request lines after spliting bt "\r\n"
        RequestMethod method;    
        public string relativeURI;   
        Dictionary<string, string> headerLines;   

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;     // the whole request to devide into lines
        string[] contentLines;    

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the *request line*, *header lines* and *content*, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();
            //TODO: parse the receivedRequest using the \r\n delimeter   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            // Parse Request line
            if (!ParseRequestLine())
                return false;

            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;

            // Validate blank line
            if (!ValidateBlankLine())
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            this.requestLines = this.requestString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (requestLines.Length < 3)
                return false;


            string[] tokens = this.requestLines[0].Split(' ');
            if(tokens.Length != 3)
            {
                if (tokens.Length == 2)
                    httpVersion = HTTPVersion.HTTP09;
                else
                    return false;
            }
            //
            if (tokens[0] == "GET")
                method = RequestMethod.GET;
            else if (tokens[0] == "POST")
                method = RequestMethod.POST;
            else if (tokens[0] == "HEAD")
                method = RequestMethod.HEAD;
            else
                return false;

            //
            if (ValidateIsURI(tokens[1]))
                this.relativeURI = tokens[1].Remove(0,1);  // remove the uri slash and replace it with douple slash \\
            
            //
            if (tokens[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (tokens[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else if (tokens[2] == "HTTP/0.9")
                httpVersion = HTTPVersion.HTTP09;
            else
                return false;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            for (int i = 1; i < requestLines.Length -1; i++)
            {
                int separator = this.requestLines[i].IndexOf(':');
                if (separator == -1)
                {
                    return false;
                }
                if (this.requestLines[i][separator + 1] != ' ')
                    return false;

                String name = this.requestLines[i].Substring(0, separator);
                int pos = separator + 2; // +1 for space & +1 for first char of value
                
                string value = this.requestLines[i].Substring(pos, requestLines[i].Length - pos);
                this.headerLines[name] = value;
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
                if (requestLines[requestLines.Length-1] == "\r\n")
                    return true;

            return false;
        }

    }
}
