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
    
    [HttpGet]
    [Route("task/{id}")]
    public async Task<IActionResult> GetTask([FromRoute] int id)
    {
        var result = await _dispatcherService.GetTask(id);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("sub-task/{id}")]
    public async Task<IActionResult> GetSubTask([FromRoute] int id)
    {
        var result = await _dispatcherService.GetSubTask(id);
        
        return Ok(result);
    }
    
    [HttpPost]
    [Route("sub-task/restart")]
    public async Task<IActionResult> RestartSubTaskStatus([FromBody] RestartSubTaskDto subTaskDto)
    {
        await _dispatcherService.RestartSubTask(subTaskDto);
        
        return Ok();
    }
    
    [HttpGet]
    [Route("task/{id}/status")]
    public async Task<IActionResult> GetTaskStatus([FromRoute] int id)
    {
        var result = await _dispatcherService.GetTaskStatus(id);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("sub-task/{id}/status")]
    public async Task<IActionResult> GetSubTaskStatus([FromRoute] int id)
    {
        var result = await _dispatcherService.GetSubTaskStatus(id);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("task/{id}/statistics")]
    public async Task<IActionResult> GetTaskStat([FromRoute] int id)
    {
        var result = await _dispatcherService.GetTaskStat(id);
        
        return Ok(result);
    }
}