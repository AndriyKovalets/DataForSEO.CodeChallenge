namespace Dispatcher.WorkerApplication.Abstractions.Services.Parsers;

public interface IParser<out TModel>
{
    public IAsyncEnumerable<TModel?> Parse(Stream stream);
}