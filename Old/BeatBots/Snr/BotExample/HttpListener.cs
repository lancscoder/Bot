using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BotExample
{
    class HttpListenerClass
    {
        private readonly string[] _indexFiles =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        private IDictionary<string, string> _mimeTypeMappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                #region extension to MIME type list
                {".asf", "video/x-ms-asf"},
                {".asx", "video/x-ms-asf"},
                {".avi", "video/x-msvideo"},
                {".bin", "application/octet-stream"},
                {".cco", "application/x-cocoa"},
                {".crt", "application/x-x509-ca-cert"},
                {".css", "text/css"},
                {".deb", "application/octet-stream"},
                {".der", "application/x-x509-ca-cert"},
                {".dll", "application/octet-stream"},
                {".dmg", "application/octet-stream"},
                {".ear", "application/java-archive"},
                {".eot", "application/octet-stream"},
                {".exe", "application/octet-stream"},
                {".flv", "video/x-flv"},
                {".gif", "image/gif"},
                {".hqx", "application/mac-binhex40"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".img", "application/octet-stream"},
                {".iso", "application/octet-stream"},
                {".jar", "application/java-archive"},
                {".jardiff", "application/x-java-archive-diff"},
                {".jng", "image/x-jng"},
                {".jnlp", "application/x-java-jnlp-file"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "application/x-javascript"},
                {".mml", "text/mathml"},
                {".mng", "video/x-mng"},
                {".mov", "video/quicktime"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".msi", "application/octet-stream"},
                {".msm", "application/octet-stream"},
                {".msp", "application/octet-stream"},
                {".pdb", "application/x-pilot"},
                {".pdf", "application/pdf"},
                {".pem", "application/x-x509-ca-cert"},
                {".pl", "application/x-perl"},
                {".pm", "application/x-perl"},
                {".png", "image/png"},
                {".prc", "application/x-pilot"},
                {".ra", "audio/x-realaudio"},
                {".rar", "application/x-rar-compressed"},
                {".rpm", "application/x-redhat-package-manager"},
                {".rss", "text/xml"},
                {".run", "application/x-makeself"},
                {".sea", "application/x-sea"},
                {".shtml", "text/html"},
                {".sit", "application/x-stuffit"},
                {".swf", "application/x-shockwave-flash"},
                {".tcl", "application/x-tcl"},
                {".tk", "application/x-tcl"},
                {".txt", "text/plain"},
                {".war", "application/java-archive"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".wmv", "video/x-ms-wmv"},
                {".xml", "text/xml"},
                {".xpi", "application/x-xpinstall"},
                {".zip", "application/zip"},

                #endregion
            };

        private Thread _serverThread;
        private HttpListener _listener;
        private int _port;
        
        public int Port
        {
            get { return _port; }
            private set { }
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="port">Port of the server.</param>
        public HttpListenerClass(int port)
        {
            this.Initialize(port);           
        }


        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception ex)
                {

                }
            }
        }

 
        private void Process(HttpListenerContext context)
        {
            string body = string.Empty;
            StreamReader sr = new StreamReader(context.Request.InputStream);
            using (sr)
            {
                body = sr.ReadToEnd();
            }

            switch (context.Request.Url.AbsolutePath.Replace("/", ""))
            {
                case "start":
                {
                    ProcessStartResponse(body);
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "text/plain";
                    StreamWriter sw = new StreamWriter(context.Response.OutputStream);
                    using (sw)
                    {
                        sw.WriteLine(context.Request.RawUrl);
                    }

                    break;
                }
                case "move":
                {
                    if (context.Request.HttpMethod.ToLower() == "get")
                    {

                        StreamWriter sww = new StreamWriter(context.Response.OutputStream);
                        string responsestr = BotAIClass.GetMove();
                        Console.WriteLine(string.Format("My move {0}", responsestr));


                        context.Response.ContentLength64 = responsestr.Length;
                        sww.Write(responsestr);
                        

                        sww.Flush();
                        sww.Close();
                        context.Response.OutputStream.Close();
                        context.Response.Close();
                        break;
                    }
                    else
                    {
                        BotAIClass.SetLastOpponentsMove(body);
                        Console.WriteLine(string.Format("Their move {0}", body));
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = "text/plain";
                        StreamWriter sw = new StreamWriter(context.Response.OutputStream);
                        using (sw)
                        {
                            sw.WriteLine(context.Request.RawUrl);
                        }

                    }
                    break;
                }
                    break;
            }
        }

        private void ProcessStartResponse(string responseBody)
        {
            int dynamiteCount = 0;
            int pointToWin = 0;
            int maxRounds = 0;
            string opponentName = string.Empty;

            string[] parameters = responseBody.Split(new char[] {'&'});
            foreach (var parameter in parameters)
            {
                if (parameter.Contains('='))
                {
                    string paramName = parameter.Split(new char[] {'='})[0];
                    string paramValue = parameter.Split(new char[] {'='})[1];

                    switch (paramName.ToLower())
                    {

                        case "dynamitecount":
                        {
                            dynamiteCount = int.Parse(paramValue);
                            break;
                        }
                        case "pointstowin":
                        {
                            pointToWin = int.Parse(paramValue);
                            break;
                        }
                        case "maxrounds":
                        {
                            maxRounds = int.Parse(paramValue);
                            break;
                        }
                        case "opponentname":
                        {
                            opponentName = paramValue;
                        }                      
                         
                        break;
                    }
               }
            }

            Console.WriteLine(string.Format("START Opponentname={0} Pointstowin={1} Maxrounds={2} Dynamitecount={3}", opponentName, pointToWin, maxRounds, dynamiteCount));

            BotAIClass.SetStartValues(opponentName, pointToWin, maxRounds, dynamiteCount);
        }


        private void Initialize(int port)
        {
            this._port = port;
            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
            Console.WriteLine(string.Format("Bot initialised on port {0}.", port));
        }


    }
}