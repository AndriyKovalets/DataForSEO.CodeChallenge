using System.IO.Compression;
using System.Text.Json;
using Dispatcher.Application.Abstractions.Parsers;

namespace Dispatcher.Application.Parsers;

public class GzipParser<TModel>: IParser<TModel>
{
    public async IAsyncEnumerable<TModel?> Parse(Stream stream)
    {
        await using var decompressedStream = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressedStream);

        while (reader.EndOfStream == false)
        {
            var stringJson = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(stringJson))
            {
                yield return default;
                continue;
            }

            TModel? result = default;

            try
            {
                result = JsonSerializer.Deserialize<TModel>(stringJson);
            }
            catch (JsonException ex)
            {
                //todo: add logger
                //Console.WriteLine($"Deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //todo: add logger
                //Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            yield return result;
        }
    }
}