
namespace Code.Extensions;
public static class HttpResponseMessageExtensions
{
    public static void WriteRequestToConsole(this HttpResponseMessage response, CancellationTokenSource cts, Stopwatch watch)
    {
        if (response is null)
        {
            return;
        }
        while (!cts.Token.IsCancellationRequested)
        {
            var request = response.RequestMessage;
            Console.Write($"{request?.Method} ");
            Console.Write($"{request?.RequestUri} ");
            Console.Write($"{request?.Content} ");
            Console.WriteLine($"HTTP/{request?.Version}");
            Console.WriteLine($"status: {response.StatusCode}, via HTTP version: {response.Version}");
            Console.WriteLine($"Response message: {response.Content.ReadAsStringAsync().Result} ");
            Console.WriteLine($" Stopwatch elapsed {watch.Elapsed} milliseconds {watch.ElapsedMilliseconds} and ticks {watch.ElapsedTicks}");
            Console.WriteLine(); Console.WriteLine();
        }
    }
}