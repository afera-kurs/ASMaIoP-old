using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ASMaIoP.General.Client
{
    public static class ArduinoApplicationAPI
    {
        // Обявляем переменую храняшую в себе обьект класса SerialPort благодаря которому происходить общение с ардуино
        static SerialPort CurrentArduionoPort = null;
        // Обьявляем делегата который будет вызываться при получении данных с сериал порта
        public static CardReceivedHandler cardReceivedHandler = null;
        // Данный метод будет открывать порт для общения с ардуино
        public static bool OpenArduino(string sPortName)
        {
            // создаем экземпляр класса SerialPort
            CurrentArduionoPort = new SerialPort();
            // Устанавливаем Название порта который будем открывать
            CurrentArduionoPort.PortName = sPortName;
            // Скорость передачи в бодах
            CurrentArduionoPort.BaudRate = 9600;

            CurrentArduionoPort.DataReceived += port_DataReceived; // Присвайваем делегату метод port_DataReceived
            try
            {
                CurrentArduionoPort.Open(); //Открываем serial порт
            }
            catch
            {
                return false; //Возвращаем false если порт не удалось открыть
            }
            return true; //Возварщаем если всё успешно
        }

        public delegate void CardReceivedHandler(string CardId); //объявляем сигнатуру делигата принимающий ID карты пользвателя 

        static void port_DataReceived(object sender, SerialDataReceivedEventArgs e) //Стандартное событие для данных с SerialPort
        {
            try
            {
                string CardId = CurrentArduionoPort.ReadLine();// Считываем строку содержащую id карты из ардуинки
                cardReceivedHandler.Invoke(CardId);// Передаем вызываем и передаем ее делегату
            }
            catch
            {
                return;
            }
        }
        //Метод для вызова закрытия SerialPort
        public static void ClosePort()
        {   
            CurrentArduionoPort.Close();//Само закрытие 
        }
    }
}
