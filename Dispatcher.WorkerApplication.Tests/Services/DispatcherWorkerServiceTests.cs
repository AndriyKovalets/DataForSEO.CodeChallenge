using System.Net;
using Dispatcher.Application.Abstractions.Persistence;
using Dispatcher.Domain.Entities;
using Dispatcher.Domain.Enums;
using Dispatcher.Domain.Models;
using Dispatcher.SharedApplication.Abstractions.Queue;
using Dispatcher.WorkerApplication.Abstractions.Services.Parsers;
using Dispatcher.WorkerApplication.Abstractions.Services.Processors;
using Dispatcher.WorkerApplication.Services;
using Moq;
using Moq.Protected;

namespace Dispatcher.WorkerApplication.Tests.Services;

public class DispatcherWorkerServiceTests
{
    
    [Fact]
    public async Task ProcessSubTask_InvalidUrl_SetsFailStatus()
    {
        // Arrange
        var subTask = new SubTaskEntity { Url = "not-a-valid-url" };

        var service = CreateService();

        // Act
        await service.ProcessSubTask(subTask);

        // Assert
        Assert.Equal(SubTaskStatusEnum.FailFile, subTask.Status);
    }
    
    [Fact]
    public async Task ProcessSubTask_HttpFails_SetsFailStatus()
    {
        // Arrange
        var subTask = new SubTaskEntity { Url = "http://valid-url.com" };

        var httpClient = CreateMockHttpClient(HttpStatusCode.NotFound);
        var service = CreateService(httpClient: httpClient);

        // Act
        await service.ProcessSubTask(subTask);

        // Assert
        Assert.Equal(SubTaskStatusEnum.FailFile, subTask.Status);
    }
    
    [Fact]
    public async Task ProcessSubTask_ValidFlow_SetsCompletedStatusAndCallsProcessors()
    {
        // Arrange
        var subTask = new SubTaskEntity { Url = "http://valid-url.com" };

        var keywordData = new List<KeywordModel> { new() };

        var parserMock = new Mock<IParser<KeywordModel>>();
        parserMock.Setup(p => p.Parse(It.IsAny<Stream>()))
            .Returns(GetAsyncEnumerable(keywordData));

        var parserFactoryMock = new Mock<IParserFactory>();
        parserFactoryMock.Setup(f => f.GetTypeOfParser(It.IsAny<string>())).Returns(ParserTypeEnum.Gzip);
        parserFactoryMock.Setup(f => f.CreateParser<KeywordModel>(ParserTypeEnum.Gzip)).Returns(parserMock.Object);

        var processorMock = new Mock<IMetricProcessor>();
        processorMock.Setup(p => p.Process(It.IsAny<KeywordModel>()));
        processorMock.Setup(p => p.SetMetric(It.IsAny<SubTaskEntity>()));

        var metricModuleMock = new Mock<IMetricProcessorsModule>();
        metricModuleMock.Setup(m => m.GetMetricProcessors()).Returns(new List<IMetricProcessor> { processorMock.Object });

        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{\"keyword\":\"test\"}");

        var service = CreateService(
            parserFactory: parserFactoryMock.Object,
            metricModule: metricModuleMock.Object,
            httpClient: httpClient
        );

        // Act
        await service.ProcessSubTask(subTask);

        // Assert
        Assert.Equal(SubTaskStatusEnum.Completed, subTask.Status);
        processorMock.Verify(p => p.Process(It.IsAny<KeywordModel>()), Times.Once);
        processorMock.Verify(p => p.SetMetric(subTask), Times.Once);
    }
    
    private static async IAsyncEnumerable<T> GetAsyncEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
            await Task.Yield();
        }
    }

    private DispatcherWorkerService CreateService(
        HttpClient? httpClient = null,
        IMetricProcessorsModule? metricModule = null,
        IParserFactory? parserFactory = null)
    {
        var contextMock = new Mock<IApplicationDbContext>();
        var queueMock = new Mock<IQueueService>();
        var clientFactoryMock = new Mock<IHttpClientFactory>();

        httpClient ??= CreateMockHttpClient(HttpStatusCode.OK, "{}");
        clientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
        metricModule ??= Mock.Of<IMetricProcessorsModule>();
        parserFactory ??= Mock.Of<IParserFactory>();

        return new DispatcherWorkerService(
            contextMock.Object,
            clientFactoryMock.Object,
            metricModule,
            queueMock.Object,
            parserFactory);
    }

    private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content = "")
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });

        return new HttpClient(handlerMock.Object);
    }
}