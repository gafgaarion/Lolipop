using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections;
using System.Web;
using FFXICommands.Commands;
using CommunicationHandler;
using System.Threading;
using FFXIWebObserver;
using FFXIWorldKnowledge;

namespace FFXIWebObserver
{
    class ResponseProcessor
    {
        private IBus bus;
        private ITruthRepository truthRepository;

        public ResponseProcessor(IBus bus,
                                ITruthRepository truthRepository)
        {
            this.bus = bus;
            this.truthRepository = truthRepository;
        }

        public void Process(HttpListenerContext context)
        {
            //Console.WriteLine("{0} {1}", context.Request.HttpMethod, context.Request.RawUrl);

            var response = context.Response;

            if (context.Request.RawUrl.Contains("/api/startStrategy/?cs"))
            {
                var characterName = int.Parse(context.Request.QueryString.Get("c"));
                var strategy = int.Parse(context.Request.QueryString.Get("s"));




                response.SendChunked = true;
                using (var tw = new StreamWriter(response.OutputStream))
                {
                    tw.Write("OK");
                }
                return;
            }


            if (context.Request.RawUrl.Contains("/api/getCharacters"))
            {
                response.SendChunked = true;
                using (var tw = new StreamWriter(response.OutputStream))
                {
                    tw.Write(ajaxServeJsonCharacterList());
                }

                return;
            }

            if (context.Request.RawUrl.Contains("/api/getMonsters"))
            {
                response.SendChunked = true;
                using (var tw = new StreamWriter(response.OutputStream))
                {
                    tw.Write(ajaxServeJsonMonstersList());
                }

                return;
            }

            if (context.Request.RawUrl.Contains("/api/registerCharacter?c="))
            {

                var characterName = context.Request.QueryString.Get("c");
                var uid = int.Parse(context.Request.QueryString.Get("uid"));
                var cmd = new InitializeCharacterCommand { characterName = characterName };
                bus.Send(cmd);

          //      CommunicationState state = null;

                //wait until connection port is assigned
                //while ((state = communicationStatesRepository.GetCharacterCommunicationState(uid)) == null)
                //{
                //    Thread.Sleep(100);
                //}
                
            //    Console.WriteLine("Registering " + characterName + "(" + uid +") on port "+ state.port );

                //response.SendChunked = true;
                //using (var tw = new StreamWriter(response.OutputStream))
                //{
                //    tw.Write("PORT " + state.port + " ID " + uid);
                //}
                //return;
            }

            //static routes
            switch (context.Request.RawUrl)
            {

                case "/":
                    response.SendChunked = true;
                    using (var tw = new StreamWriter(response.OutputStream))
                    {
                        tw.WriteLine(serveHomePage());
                    }
                    break;

                case "/favicon.ico":
                    using (FileStream fs = File.OpenRead(@"resources\vlad.jpg"))
                    {
                        response.ContentLength64 = fs.Length;
                        response.SendChunked = false;
                        response.ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;

                        byte[] buffer = new byte[64 * 1024];
                        int read;
                        using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                        {
                            while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, read);
                                bw.Flush();
                            }

                            bw.Close();
                        }

                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.StatusDescription = "OK";
                        response.OutputStream.Close();
                    }
                    break;

                default:

                    var substr = context.Request.RawUrl.Substring(1);

                    try
                    {
                        using (FileStream fs = File.OpenRead(@"resources\" + substr))
                        {
                            response.ContentLength64 = fs.Length;
                            response.SendChunked = false;

                            if (substr.Contains(".jpg"))
                                response.ContentType = "image/jpg";

                            if (substr.Contains(".png"))
                                response.ContentType = "image/png";

                            if (substr.Contains(".js"))
                                response.ContentType = "text/javascript";

                            if (substr.Contains(".css"))
                                response.ContentType = "text/css";


                            byte[] buffer = new byte[64 * 1024];
                            int read;
                            using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                            {
                                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    bw.Write(buffer, 0, read);
                                    bw.Flush();
                                }

                                bw.Close();
                            }

                            response.StatusCode = (int)HttpStatusCode.OK;
                            response.StatusDescription = "OK";
                            response.OutputStream.Close();
                        }
                    }
                    catch (System.IO.IOException ex)
                    {
                        response.SendChunked = true;
                        using (var tw = new StreamWriter(response.OutputStream))
                        {
                            tw.WriteLine(serve404());
                        }
                    }

                    break;
            }

        }

        private Dictionary<string, string> GetFormValues(HttpListenerRequest request)
        {

            var formVars = new Dictionary<string, string>();

            if (request.HasEntityBody)
            {
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new System.IO.StreamReader(body, encoding);

                if (request.ContentType.ToLower() == "application/x-www-form-urlencoded")
                {
                    string s = reader.ReadToEnd();
                    string[] pairs = s.Split('&');

                    for (int x = 0; x < pairs.Length; x++)
                    {
                        string[] item = pairs[x].Split('=');
                        formVars.Add(item[0], HttpUtility.UrlDecode(item[1]));
                    }
                }

                body.Close();
                reader.Close();
            }

            return formVars;
        }



        private string getBodyHeader()
        {

            return @"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8"">
    <title>Vladibot</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta name=""description"" content="""">
    <meta name=""author"" content="""">

    <!-- Le styles -->
    <link href=""/bootstrap.min.css"" rel=""stylesheet"">
    <link href=""/bootstrap-responsive.min.css"" rel=""stylesheet"">
    <link href=""/vladibot.css"" rel=""stylesheet"">
  </head>

  <body><div class=""hero-unit""><img class=""vladLogo"" src='/vlad.jpg'><h1>Vladibot 2 - A Vlad Reborn</h1></div>";
            
        }

        private string getBodyFooter()
        {
            return @"
    <script src=""/jquery-1.9.1.min.js""></script>
    <script src=""/bootstrap.min.js""></script>
    <script src=""/vladibot.js""></script>
  </body>
</html>";
        }

        private string serveHomePage()
        {
            var botstatus = @"<div class=""span6""><h2>Character status</h2><div id=""characterList""></div></div><div class=""span6""><h2>Monsters status</h2><div id=""monstersList""></div></div>";

//            var registerform = @"
//                <div class=""span6"">
//                <form name='input' action='registerCharacter' method='post'>
//                Character Name: <input type='text' name='characterName'>
//                <br/><button type=""submit"" class=""btn btn-success""><i class=""icon-plus""></i>Register character</button>
//                </form></div>";

            return getBodyHeader() + botstatus + getBodyFooter();
        }

        private string serve404()
        {
            return getBodyHeader() + "<p>404</p>" + getBodyFooter();
        }



        private string ajaxServeJsonMonstersList()
        {
            return @"{ ""monsters"" : []}";

            //var returnVal = @"{ ""monsters"" : [";

            //var characterObjects = truthRepository.getMonsterShards().Select(
            //        c => @"{""id"":""" + c.monsterId + @""", ""name"":""" + c.name + @""", ""hpp"":""" + c.hpp + @""", ""x"":""" + c.position.X + @""", ""y"":""" + c.position.Y + @""", ""z"":""" + c.position.Z + @"""}");

            //var bIsFirst = true;

            //foreach (var subEl in characterObjects)
            //{
            //    if (!bIsFirst)
            //        returnVal += ",";
            //    else
            //        bIsFirst = false;

            //    returnVal += subEl;
            //}

            //return returnVal + "]}";
        }

        private string ajaxServeJsonCharacterList()
        {
            return @"{ ""characters"" : []}";

//            var returnVal = @"{ ""characters"" : [";

            //var characterObjects = truthRepository.getCharacterShards().Select(
            //        c => @"{""id"":""" + c.characterName + @""", ""name"":""" + c.name + @""", ""hpp"":""" + c.hpp + @""", ""hp"":""" + c.hp + @""", ""mp"":""" + c.mp + @""", ""tp"":""" + c.tp + @""", ""x"":""" + c.position.X + @""", ""y"":""" + c.position.Y + @""", ""z"":""" + c.position.Z + @"""}");

            //var bIsFirst = true;

            //foreach (var subEl in characterObjects)
            //{
            //    if (!bIsFirst)
            //        returnVal += ",";
            //    else
            //        bIsFirst = false;

            //    returnVal += subEl;
            //}

            //return returnVal + "]}";
        }
        

    }
}
