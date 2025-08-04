namespace Dispatcher.Application.Abstractions.Parsers;

public interface IParser<out TModel>
{
    public IAsyncEnumerable<TModel?> Parse(Stream stream);
}