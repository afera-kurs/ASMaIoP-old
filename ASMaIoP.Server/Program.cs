using System;
using System.Net;

namespace ASMaIoP.Server
{
    internal class Program
    {

        class ASMaIoP_Connection : ASMaIoP.General.Server.Connection
        {
            enum ProtoId : short
            { 
                Auth = 0
            }

            public override bool Process()
            {
                ProtoId nProtoId = (ProtoId)ReadInt();
                switch(nProtoId)
                {
                    case ProtoId.Auth:
                        break;
                }

                return true;
            }
        }

        static General.Config Configuration = new General.Config();
        static General.Server.Server<ASMaIoP_Connection> m_Server;

        static void Main(string[] args)
        {
            if(Configuration.ParseFromFile("server.cfg") != General.ErrorCode.SUCCESS)
            {
                Console.WriteLine("failed to find configuration file!");
                return;
            }
            if(!Configuration.ContaintsVariable("port"))
            {
                Console.WriteLine("failed to find variable \'port\'!");
                return;
            }    
            string sPort = Configuration["port"];

            try
            {
                m_Server = new General.Server.Server<ASMaIoP_Connection>(short.Parse(sPort));
            }
            catch(FormatException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch
            {
                Console.WriteLine("Invalid error!");
                return;
            }
            m_Server.Start();
            while(true)
            {
                string Cmd = Console.ReadLine();
                if (Cmd.Length > 0)
                {
                    string[] sWords = Cmd.Split(' ');

                    if (sWords.Length < 0) continue;

                    if (sWords[0] == "CmdStop")
                    {
                        m_Server.Stop();
                    }
                }
            }
        }
    }
}
