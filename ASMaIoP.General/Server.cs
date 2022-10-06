using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.ComponentModel;
using ASMaIoP.General.Client;

namespace ASMaIoP.General
{
    namespace Server
    {
        // данный класс будет описывать соединение клиента
        public class Connection
        {
            //Класс описывающий соединение клиента в System.Net.Sockets
            TcpClient m_TCPClient;
            //поток данных для доступа к сети.
            NetworkStream m_Stream;

            Thread m_ProccessThread;

            int m_nId;

            public Connection()
            {

            }

            public void ProcessHandler()
            {
                while(true)
                {
                    if(!Process())
                    {
                        Disconnect();
                        
                        return;
                    }
                }
            }

            public virtual bool Process() 
            {
                return false;
            }

            public void SetClient(TcpClient client, int nId)
            {
                m_nId = nId;
                m_TCPClient = client;
                // получаем поток для отправки и принятия данных
                m_Stream = client.GetStream();
                // Запускаем поток обработчика клиента
                m_ProccessThread = new Thread(ProcessHandler);
                m_ProccessThread.Start();
            }

            public Connection(TcpClient client)
            {
                m_TCPClient = client;
                // получаем поток для отправки и принятия данных
                m_Stream = client.GetStream();
            }

            // Данный метод позволяет отправлять int переменную
            public void Write(int nData)
            {
                // конвертируем int в байты
                byte[] bytes = BitConverter.GetBytes(nData);
                // отправляем полученные байты клиенту
                m_Stream.Write(bytes, 0, sizeof(int));
            }

            // Данный метод позволяет считать int с клиента
            public int ReadInt()
            {
                // Резервируем байты для принятия данных
                byte[] data = new byte[sizeof(int)];
                // принимает байты с клиента
                m_Stream.Read(data, 0, sizeof(int));
                // конвертируем полуечнные данные в int
                return BitConverter.ToInt32(data, 0);
            }

            // Данный метод позволяет отправлять строку клиенту
            public void Write(string sData)
            {
                // конвертируем строку в байты
                byte[] data = Encoding.Unicode.GetBytes(sData);
                // получаем количество байт
                int nSize = sData.Length;
                // отправляем размер строки клиенту
                Write(nSize);
                // отправляем байты строки клиенту
                m_Stream.Write(data, 0, data.Length);
            }

            // Данный метод позволяет принять 
            public string ReadString()
            {
                // Получаем размер строки у клиента
                int nSize = ReadInt();
                // резервируем память под строку
                byte[] data = new byte[nSize];
                // Получаем саму строку у клиента
                m_Stream.Read(data, 0, nSize);
                // Получаем конвертируем байты в строку
                return Encoding.UTF8.GetString(data);
            }

            // Данный метод позволяет отключить клиента
            public void Disconnect()
            {
                // разрываем соединение
                m_TCPClient.Close();
                // закрываем поток данных
                m_Stream.Close();
            }
        }

        // Данный класс описывает сервер 
        public class Server<T_connection> where T_connection : Connection, new()
        {
            // прослушиватель - прослушивает подключений от TCP клиентов сети
            TcpListener listener;
            // Поток для прослушивания подключений
            Thread serverListenThread;
            // Сами подключения
            protected List<T_connection> connections;

            public Server(short nPort)
            {
                listener = new TcpListener(nPort);
            }

            // Данный метод описывает поток прослушивателя соеденений
            void ListenThread()
            {
                // Начинаем прошлушивание
                listener.Start();

                while (true)
                {
                    // AcceptTcpClient - данный метод позволяет принять подключение клиента
                    TcpClient client = listener.AcceptTcpClient();
                    // Создаем класс опписывающий подключение клиента и добавляем его в лист

                    T_connection con = new T_connection();
                    con.SetClient(client, connections.Count);
                    connections.Add(con);
                }
            }

            public ErrorCode Start()
            {
                try
                {
                    // Создаем поток
                    serverListenThread = new Thread(ListenThread);
                    // Отделяем поток от главного
                    serverListenThread.Start();
                }
                catch
                {
                    return ErrorCode.InvaliedError;
                }

                return ErrorCode.SUCCESS;
            }

            // Данный метод останавливает сервер
            public void Stop()
            {
                // останавливает прослушиваетль порта
                listener.Stop();
                // отключаем всех клиентов
                foreach (Connection conn in connections)
                {
                    conn.Disconnect();
                }
                // присоеденяем поток прослушивателя
                serverListenThread.Join();
            }

        }
    }
}
