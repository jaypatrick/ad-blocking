
namespace Code.Extensions;
public static class HttpResponseMessageExtensions
{
    public static async Task WriteRequestToConsoleAsync(this HttpResponseMessage response, CancellationTokenSource cts, Stopwatch watch)
    {
        if (response is null)
        {
            return;
        }

        var request = response.RequestMessage;
        Console.Write($"{request?.Method} ");
        Console.Write($"{request?.RequestUri} ");
        Console.Write($"{request?.Content} ");
        Console.WriteLine($"HTTP/{request?.Version}");
        Console.WriteLine($"status: {response.StatusCode}, via HTTP version: {response.Version}");

        string responseContent = await response.Content.ReadAsStringAsync(cts.Token);
        Console.WriteLine($"Response message: {responseContent}");
        Console.WriteLine($" Stopwatch elapsed {watch.Elapsed} milliseconds {watch.ElapsedMilliseconds} and ticks {watch.ElapsedTicks}");
        Console.WriteLine();
        Console.WriteLine();
    }
}