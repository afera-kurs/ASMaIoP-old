using System;
using System.Collections.Generic;
using System.Text;

namespace ASMaIoP.General.Client
{
    public enum ProtocolId //Код проток
    { 
        Auth = 0, 
        DataTransfer_MyProfile = 1,
        DataTransfer_Inventory = 2,

    }

    public class Session : ASMaIoP.General.Client.Client
    {
        public Session(string Address)
        {
            SetupAddress(Address);//Устанавливаем адрес сесии
        }

        public ErrorCode Open() => Connect(); // Открываем соедение с сервером

        int nSessionId = 0;

        public int SessionId { get => nSessionId; }//Свойства для чтение

        public bool Auth(string CardId)
        {
            if (Open() != General.ErrorCode.SUCCESS) //Провереям смогли мы подключиться
            {
                return false;
            }

            Write((int)General.Client.ProtocolId.Auth); //Отправляем серверу протокол авторизации
            Write(CardId); //Отправляем в сервер ID приложенной карты

            int Code = ReadInt(); //Считваем код отправленный сервером для определение ID сесии
            if (Code == 1)
            {
                nSessionId = ReadInt();
            }

            Close();

            return Code == 1;
        }


    }
}
