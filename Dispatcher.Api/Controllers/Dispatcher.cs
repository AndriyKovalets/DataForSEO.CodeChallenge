using Dispatcher.Application.Abstractions.Services;
using Dispatcher.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dispatcher.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class Dispatcher: ControllerBase
{
    private readonly IDispatcherService _dispatcherService;

    public Dispatcher(IDispatcherService dispatcherService)
    {
        _dispatcherService = dispatcherService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTask([FromBody] CreateTaskDto task)
    {
        var createdTask = await _dispatcherService.CreateTask(task);
        
        return Created($"/{createdTask.Id}", createdTask);
    }
}