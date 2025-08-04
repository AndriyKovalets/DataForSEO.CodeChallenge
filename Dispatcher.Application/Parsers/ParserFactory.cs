using Dispatcher.Application.Abstractions.Parsers;
using Dispatcher.Domain.Enums;

namespace Dispatcher.Application.Parsers;

public static class ParserFactory
{
    public static IParser<TEntity> CreateParser<TEntity>(ParserTypeEnum type)
    {
        return type switch
        {
            ParserTypeEnum.Gzip => new GzipParser<TEntity>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Can not create parser")
        };
    }
    
    public static ParserTypeEnum GetTypeOfParser(string url)
    {
        if (url.EndsWith(".data.json.gz"))
        {
            return ParserTypeEnum.Gzip;
        }

        return ParserTypeEnum.Default;
    }
}