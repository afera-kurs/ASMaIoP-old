using System;
using System.Net;
using ASMaIoP.General.Client;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using ASMaIoP.General;

namespace ASMaIoP.Server
{
    internal class Program
    {
        static List<UserData> lUsers = new List<UserData>();
        //mission complite


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

                            if (database.IsCardIdExits(CardId))
                            {
                                int somethingNumber = 0;
                                bool IsFind = false;
                                foreach(UserData _data in lUsers)
                                {
                                    if(_data.sCardId == CardId)
                                    {
                                        IsFind = true;
                                        break;
                                    }
                                    somethingNumber++;
                                }

                                if (!IsFind)
                                {
                                    lUsers.Add(new UserData { sCardId = CardId });
                                    somethingNumber = lUsers.Count - 1;
                                }
                                
                                Write(1);
                                Write(somethingNumber);
                            }
                            else
                            {
                                Write(0);
                            }
                        }
                        return false;
                    case ProtocolId.DataTransfer_MyProfile:
                        int nSessionId = ReadInt();
                        UserData data = lUsers[nSessionId];
                        string EmployeeId = database.GetEmployeeId(data.sCardId);

                        database._connection.Open();

                        MySqlCommand cmd = new MySqlCommand($"SELECT people_name,people_surname,people_patronymic,role_title,role_lvl " +
                            $"FROM employee JOIN people ON employee_ID=people_ID" +
                            $"JOIN role on employee_role_ID=role_ID WHERE employee_id={EmployeeId}", database._connection);

                        MySqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();

                        string Name = reader[0].ToString();
                        string Surname = reader[1].ToString();
                        string Patronimyc = reader[2].ToString();
                        string role = reader[3].ToString();
                        string lvl = reader[4].ToString();

                        reader.Close();

                        string ProfileInfo = $"Name={Name};Surname={Surname};Patronimyc={Patronimyc};lvl={lvl};role={role};ID={EmployeeId};";
                        database._connection.Close();
                        Write(ProfileInfo);
                        Console.WriteLine(ProfileInfo);

                        return false;
                    default:
                        return false;
                }

                return true;
            }
        }

        static General.Config Configuration = new General.Config();//созд экземпляр класса config
        static General.Server.Server<ASMaIoP_Connection> m_Server;//объявляем переменную m_Server типа Server
        static DatabaseInterface database;

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
            if (!Configuration.ContaintsVariable("MySQL_Address")) //Ищем в cfg переменную MySQL_Address 
            {
                Console.WriteLine("failed to find variable \'MySQL_Address\'!");
                return;
            }
            if (!Configuration.ContaintsVariable("MySQL_Port")) //Ищем в cfg переменную MySQL_Port 
            {
                Console.WriteLine("failed to find variable \'MySQL_Port\'!");
                return;
            }
            if (!Configuration.ContaintsVariable("MySQL_Login")) //Ищем в cfg переменную MySQL_Login 
            {
                Console.WriteLine("failed to find variable \'MySQL_Login\'!");
                return;
            }
            if (!Configuration.ContaintsVariable("MySQL_Password")) //Ищем в cfg переменную MySQL_Password 
            {
                Console.WriteLine("failed to find variable \'MySQL_Password\'!");
                return;
            }
            if (!Configuration.ContaintsVariable("MySQL_DataBase")) //Ищем в cfg переменную MySQL_DataBase 
            {
                Console.WriteLine("failed to find variable \'MySQL_DataBase\'!");
                return;
            }

            string sPort = Configuration["port"];//Присвайваем переменной знанчение из cfg 

            //"server=caseum.ru;port=33333;user=test_user;" +
            // "database=db_test;password=test_pass;";
            //
            string sConnectionString = $"" +
            $"server={Configuration["MySQL_Address"]};" +
            $"port={Configuration["MySQL_Port"]};" +
            $"user={Configuration["MySQL_Login"]};" +
            $"database={Configuration["MySQL_DataBase"]};" +
            $"password={Configuration["MySQL_Password"]};";

            Console.WriteLine($"[Debug]: {sConnectionString}");

            try
            {
                database = new DatabaseInterface(sConnectionString);
                m_Server = new General.Server.Server<ASMaIoP_Connection>(short.Parse(sPort)); //Создаём экземпляр 
            }
            catch(FormatException e)
            {
                Console.WriteLine(e.Message); 
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid error: {e.Message}!");
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
