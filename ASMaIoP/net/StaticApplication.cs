using System;
using System.Collections.Generic;
using System.Text;
using ASMaIoP.General.Client;
using ASMaIoP.General;
using System.Windows;

namespace ASMaIoP.net
{
    static class StaticApplication
    {
        static public Session Session;
        static Config ApplicationCfg;
        static string COMPortName;

        /*
         * server_address=caseum.chuc.ru:32;
         * serial_port=COM3;
         */
        public static bool ApplicationStart() //Метод проверки соеденение пользвателя с сервером
        {
            ApplicationCfg = new Config(); //Создаем обеект класса Config
            if (ApplicationCfg.ParseFromFile("client.cfg") != ErrorCode.SUCCESS) //Проверка на наличие client.cfg
            {
                MessageBox.Show("Ошибка не удолось открыть файл 'client.cfg'\nпожалуйства обратитесь к ващему системному администратору! Код ошибки 0");
                return false;
            }
            //Проверка на наличие в client.cfg строки server_address"
            if (ApplicationCfg.ContaintsVariable("server_address"))
            {
                Session = new Session(ApplicationCfg["server_address"]);
            }
            else
            {
                MessageBox.Show("Ошибка не удолось открыть файл 'client.cfg'\nпожалуйства обратитесь к ващему системному администратору! Код ошибки 1");
                return false;
            }
            //Проверка на наличие в client.cfg строки serial_port"
            if (ApplicationCfg.ContaintsVariable("serial_port"))
            {
                COMPortName = ApplicationCfg["serial_port"];
            }
            else
            {
                MessageBox.Show("Ошибка не удолось открыть файл 'client.cfg'\nпожалуйства обратитесь к ващему системному администратору! Код ошибки 2");
                return false;
            }

            if (!General.Client.ArduinoApplicationAPI.OpenArduino(COMPortName))
            {
                MessageBox.Show($"Не удолось открыть serial port:{COMPortName}\nпожалуйства обратитесь к ващему системному администратору! Код ошибки CP");
                return false;
            }

            return true;//доделать проверка на возможность открытие если программа будет экстренна закрыта!@?
        }
        //Метод для закрытия соедение с сервером
        public static void ApplicationExit()
        {
            General.Client.ArduinoApplicationAPI.ClosePort();
        }
    }
}
