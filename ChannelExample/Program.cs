//for (int i = 0; i < 10; i++)
//{
//    Console.WriteLine(i);
//    Task.Run(() => SendNotification());
//}
//void SendNotification()
//{
//    Task.Delay(1000).GetAwaiter().GetResult();
//    Console.WriteLine("complete");
//}

using System.Collections.Concurrent;
var channel = new Channel<string>();
Task.WaitAll(Consumer(channel),Producer(channel), Producer(channel), Producer(channel));


async Task Producer(IWrite<string> writer)
{
    for (int i = 0; i < 10; i++)
    {
        writer.Push(i.ToString());
        await Task.Delay(100);
    }
    writer.Complete();
}



async Task Consumer(IRead<string> reader)
{
    while(reader.IsCompleted() is false)
    {
        var msg = await reader.Read();
        Console.WriteLine("msg : "+msg);
    }
}














public interface IRead<T>
{
    Task<T> Read();
    bool IsCompleted();
}
public interface IWrite<T>
{
    void Push(T msg);
    void Complete();
}

public class Channel<T> : IRead<T>, IWrite<T>
{
    private bool Finshed;
    private readonly ConcurrentQueue<T> Queue;
    private SemaphoreSlim _flag;
    public Channel()
    {
        Queue=new ConcurrentQueue<T>();
        _flag = new SemaphoreSlim(0);
    }

    public void Push(T msg)
    {
        Queue.Enqueue(msg);
        _flag.Release();
    }

    public async Task<T> Read()
    {
        await _flag.WaitAsync();
        return Queue.TryDequeue(out T result) is false ? default : result;
    }


    public void Complete()
    {
        Finshed = true;
    }

    public bool IsCompleted()
    {
        return Finshed && Queue.IsEmpty;
    }
}
