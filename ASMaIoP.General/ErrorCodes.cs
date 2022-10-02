namespace ASMaIoP.General
{
    public enum ErrorCode : short
    {
        // Успешно
        SUCCESS,
        // Неизвестная ошибка
        InvaliedError,
        // Не получилось открыть файл
        FailedToOpenFile,
        // Не получилось запарсить
        FailedToParse,
        // Объект не инициализирован
        ObjectNotInitialized
    }
}
