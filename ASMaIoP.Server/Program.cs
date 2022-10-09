using System;
using System.Net;
using ASMaIoP.General.Client;

namespace ASMaIoP.Server
{
    internal class Program
    {

        class ASMaIoP_Connection : ASMaIoP.General.Server.Connection
        {

            public override bool Process() //Метод обработки протокола
            {
                ProtocolId nProtoId = (ProtocolId)ReadInt(); //Получаем номер протокола от клиента 
                switch(nProtoId) //Проверяем подходит ли нам этот протокол
                {
                    case ProtocolId.Auth:
                        {
                            string CardId = ReadString();
                            if (CardId == "IDE38074AD\r")
                            {
                                Write(1);
                            }
                            else Write(0);
                            Console.WriteLine($"input card_id:{CardId}");
                        }
                        return false;
                    default:
                        return false;
                }

                return true;
            }
        }

        static General.Config Configuration = new General.Config();//созд экземпляр класса config
        static General.Server.Server<ASMaIoP_Connection> m_Server;//объявляем переменную m_Server типа Server

        static void Main(string[] args)
        {
            if(Configuration.ParseFromFile("server.cfg") != General.ErrorCode.SUCCESS) //Загружаем файл server.cfg
            {
                Console.WriteLine("failed to find configuration file!");
                return;
            }
            if(!Configuration.ContaintsVariable("port")) //Ищем в cfg переменную port 
            {
                Console.WriteLine("failed to find variable \'port\'!");
                return;
            }    
            string sPort = Configuration["port"];//Присвайваем переменной знанчение из cfg 

            try
            {
                m_Server = new General.Server.Server<ASMaIoP_Connection>(short.Parse(sPort)); //Создаём экземпляр 
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
            m_Server.Start();//Запускаем сервер
            while(true)
            {
                string Cmd = Console.ReadLine(); //Получаем команду от пользвателя
                if (Cmd.Length > 0)//Проверяем чтобы строка не была пустой
                {
                    string[] sWords = Cmd.Split(' '); //Делим строку по пробелам

                    if (sWords.Length < 0) continue; //Проверям количество слов

                    if (sWords[0] == "CmdStop") //Если 1 слово = CmdStop, выключаем сервер
                    {
                        m_Server.Stop();
                    }
                }
            }
        }
    }
}
