using System;
using System.Collections.Generic;
using System.Text;

namespace ASMaIoP.General.Client
{
    
    public enum ProtocolId //Коды пркотол для общения с сервером
    { 
        Auth = 0, 
        DataTransfer_MyProfile = 1,
        DataTransfer_Inventory = 2,
        DataTransfer_CreateProfile = 3,
        DataTransfer_UpdateImage_CreateProfile = 319,
        DataSearchEmpl_CreateProfile =4,
        DataWriteEmpl_CreateProfile = 5,
        DataUpdateEmpl_CreateProfile = 6,
        DataDeleteEmpl_CreateProfile = 7,
        DataTransfer_Tasks = 8,
        DataDelete_Tasks = 9,
        DataTake_Tasks = 10,
        DataUpdateState_Tasks = 11,
        DataWrite_AppItems = 12,
        DataTransfer_AppItems = 13,
        DataDelete_AppItems = 14,
        DataUpdate_AppItems = 15,
        DataLoadFromEmployeeID_AppItems = 16,      
        DataLoadImage = 17,
        DataGetImage = 18,
        GetTaskInfo = 19,
    }

    public class Session : ASMaIoP.General.Client.Client
    {
        public Session(string Address)
        {
            SetupAddress(Address);//Устанавливаем адрес сесии
        }

        public ErrorCode Open() => Connect(); // Открываем соедение с сервером

        int nSessionId = 0;
        int nLvlId = 0;

        public int SessionId { get => nSessionId; }//Свойства для чтение
        public int AccessLevel { get => nLvlId; }//Свойства для чтение

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
                // считываем с сервера 
                nSessionId = ReadInt();
                nLvlId = ReadInt();
            }

            Close();

            return Code == 1;
        }
    }
}
