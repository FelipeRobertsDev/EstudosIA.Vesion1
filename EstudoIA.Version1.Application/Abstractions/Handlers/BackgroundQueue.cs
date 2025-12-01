using System.Collections.Concurrent;

namespace EstudoIA.Version1.Application.Abstractions.Handlers;

public static class BackgroundQueue
{
    private static readonly ConcurrentDictionary<Type, Task> _runningTasks = new();
    public static void Start(Type handlerType, int maxParallelism)
    {
        if (!_runningTasks.TryAdd(handlerType, Task.CompletedTask))
            return;

        Task.Run(() =>
        {
            SemaphoreSlim semaphore = new(maxParallelism);

            while (true)
            {

                semaphore.Wait();

                Task.Run(async () =>
                {
                    try
                    {

                        Console.WriteLine($"Executando handler: {handlerType.Name}");
                        await Task.Delay(100);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }
        });
    }
}
