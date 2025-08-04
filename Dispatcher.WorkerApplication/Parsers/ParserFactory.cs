using Dispatcher.Domain.Enums;
using Dispatcher.WorkerApplication.Abstractions.Services.Parsers;

namespace Dispatcher.WorkerApplication.Parsers;

public class ParserFactory: IParserFactory
{
    public IParser<TEntity> CreateParser<TEntity>(ParserTypeEnum type)
    {
        return type switch
        {
            ParserTypeEnum.Gzip => new GzipParser<TEntity>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Can not create parser")
        };
    }
    
    public ParserTypeEnum GetTypeOfParser(string url)
    {
        if (url.EndsWith(".data.json.gz"))
        {
            return ParserTypeEnum.Gzip;
        }

        return ParserTypeEnum.Default;
    }
}