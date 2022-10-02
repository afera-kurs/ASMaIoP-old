using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ASMaIoP.General
{
    public class Config
    {
        //Создаем хеш таблицу, подробности:https://learn.microsoft.com/en-us/dotnet/api/system.collections.hashtable?view=net-7.0
        Hashtable m_ConfigurationTable = new Hashtable();

        //Метод возвращает значение переменной из конфига
        public string? GetValue(string sVarName)
        {
            // в [] указывается имя переменной и возвращаем занчение явно преобразуется в nullable string
            return (string?)m_ConfigurationTable[sVarName];
        }

        // Метод возвращает находиться ли в таблице переменная 
        public bool ContaintsVariable(string sVarName)
        {
            return m_ConfigurationTable.ContainsKey(sVarName);
        }

        public ErrorCode ParseFromString(string sData)
        {
            // удоляем ненужные символы
            string sFormatedLine = sData.Replace(" ", String.Empty);
            sFormatedLine = sFormatedLine.Replace("\n", String.Empty);
            sFormatedLine = sFormatedLine.Replace("\t", String.Empty);

            // указывает на индекс текущего символа в строке
            int i = 0;

            List<char> lTmpWord = new List<char>();
            string sVarName = "";
            string sVarValue = "";
            bool bIsNameFind = false;

            for (; i < sFormatedLine.Length; i++)
            {
                //получаем текуший символ из отформативанной строки
                char cCurrentSym = sFormatedLine[i];

                // если текуший символ будет '=' следовательно мы нашли имя переменной так как имя находиться до занака '='
                if (cCurrentSym == '=')
                {
                    // Конвертируем лист в строку
                    sVarName = new string(lTmpWord.ToArray());
                    // Очищаем лист
                    lTmpWord.Clear();
                    // узкаваем что имя найдено
                    bIsNameFind = true;
                    // Пропускаем текущую итерацю
                    continue;
                }

                // если текуший символ будет ';' следовательно мы нашли значение переменной так как значение находиться до занака ';'
                if (cCurrentSym == ';')
                {
                    // Проверяем чтобы имя было найдено
                    if (!bIsNameFind) return ErrorCode.FailedToParse;
                    // Конвертируем лист в строку
                    sVarValue = new string(lTmpWord.ToArray());
                    // Очищаем лист
                    lTmpWord.Clear();
                    // Обнуляем переменную храняшую состояния (найдено ли имя)
                    bIsNameFind = false;

                    // Добавляем нашу переменную в хеш таблицу
                    m_ConfigurationTable.Add(sVarName, sVarValue);
                    // Пропускаем текущую итерацю
                    continue;
                }

                // Добавляем текуший символ в лист
                lTmpWord.Add(cCurrentSym);
            }

            return ErrorCode.SUCCESS;
        }


        //Метод парсит конфигурационный файл в хеш таблицу
        public ErrorCode ParseFromFile(string sFileName)
        {
            string[] sLines = null;

            try
            {
                //File.ReadAllLines позволяет считать файл в массив строк
                sLines = File.ReadAllLines(sFileName);
            }
            catch
            {
                return ErrorCode.FailedToOpenFile;
            }
            //данный лист будет содежать считанные символы из файла
            List<char> lTmpWord = new List<char>();

            //обьявляем цикл который проходит по каждой строке
            foreach (string sLine in sLines)
            {
                // j указывает на индекс текущего символа в строке
                int j = 0;

                // удоляем ненужные символы
                string sFormatedLine = sLine.Replace(" ", String.Empty);
                sFormatedLine = sFormatedLine.Replace("\n", String.Empty);
                sFormatedLine = sFormatedLine.Replace("\t", String.Empty);

                // bIsVariableParsedCorrect
                // позволяет определить получилось ли у парсера найти либо имя переменной либо значение
                bool bIsVariableParsedCorrect = false;
                // данный цикл проходиться по стоке и определяет имя перенной которое находиться до знака равно
                // port=33;
                // ---^
                for (; j < sFormatedLine.Length; j++)
                {
                    //получаем текуший символ из отформативанной строки
                    char cSym = sFormatedLine[j];
                    // если текуший символ будет '=' следовательно мы нашли имя переменной так как имя находиться до занака '='
                    if (cSym == '=')
                    {
                        // если мы нашли пробел следовательно мы можем определить имя переменной 
                        // следовательно мы указываем в переменной bIsVariableParsedCorrect что все успешно запарсилось
                        bIsVariableParsedCorrect = true;
                        // и также обрываем цыкл 
                        break;
                    }

                    //добавляем текущий символ в лист символов
                    lTmpWord.Add(cSym);
                }
                // так как на этапе со знаком равно мы оборвали цикл
                // следовательно должны пропустить знак равно что он не помешал циклу который будет считывать занчение
                j++;

                // проверяем получилось ли определить имя переменной если нет то тогда мы выходим из метода и возвращаем false 
                if (!bIsVariableParsedCorrect)
                {
                    return ErrorCode.FailedToParse;
                }

                // раз уж мы нашли пробел то следовательно мы должны обнулить состояние
                bIsVariableParsedCorrect = false;

                //ToArray() возвращаем массив символов который мы передаем в конструктор переменной string
                // которая будет хранить имя переменной
                string sVarName = new string(lTmpWord.ToArray());

                // мы очищаем лист с именем листа чтобы потом считать туда значение перменной
                lTmpWord.Clear();

                // обьявляем цикл который будет считывать занчение переменной со строки
                // port=33;
                //      _^
                for (; j < sFormatedLine.Length; j++)
                {
                    //получаем текуший символ из отформативанной строки
                    char cSym = sFormatedLine[j];
                    // если текуший символ будет ';' следовательно мы нашли значение переменной так как значение находиться до занака ';'
                    if (cSym == ';')
                    {
                        // если мы нашли ';' следовательно мы можем определить значение переменной 
                        // следовательно мы указываем в переменной bIsVariableParsedCorrect что все успешно запарсилось
                        bIsVariableParsedCorrect = true;
                        break;
                    }

                    //добавляем текущий символ в лист символов
                    lTmpWord.Add(cSym);
                }

                // проверяем получилось ли определить значение переменной если нет то тогда мы выходим из метода и возвращаем false 
                if (!bIsVariableParsedCorrect)
                {
                    return ErrorCode.FailedToParse;
                }

                //ToArray() возвращаем массив символов который мы передаем в конструктор переменной string
                // которая будет хранить значение переменной
                string sVarValue = new string(lTmpWord.ToArray());

                // мы очищаем лист с именем листа для следующей строки
                lTmpWord.Clear();

                // добавляем значение в таблицу
                m_ConfigurationTable.Add(sVarName, sVarValue);

            }

            // и если все прошло успешно 
            return ErrorCode.SUCCESS;
        }

        //обьявляем индексатор чтобы переопрделить оператор []
        public string? this[string val]
        {
            get
            {
                return (string?)m_ConfigurationTable[val];
            }
        }

        public Config(string sFileName)
        {
            // парсим файл
            ParseFromFile(sFileName);
        }

        public Config()
        {

        }
    }

}
