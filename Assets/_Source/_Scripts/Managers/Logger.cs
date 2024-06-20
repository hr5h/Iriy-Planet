#define ENABLE_LOGS

using System.Diagnostics;
using static UnityEngine.Debug;

/// <summary>
/// Класс для ведения логов. Нужен чтобы не нагружать процессор, если логи отключены
/// </summary>
public static class Logger
{

    [Conditional("ENABLE_LOG")]
    public static void Debug(string message)
    {
        Log(message);
    }
    [Conditional("ENABLE_LOG")]
    public static void Debug<T>(T message)
    {
        Log(message.ToString());
    }
}
