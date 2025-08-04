using Dispatcher.Domain.Enums;

namespace Dispatcher.WorkerApplication.Abstractions.Services.Parsers;

public interface IParserFactory
{
    IParser<TEntity> CreateParser<TEntity>(ParserTypeEnum type);
    ParserTypeEnum GetTypeOfParser(string contentType);
}