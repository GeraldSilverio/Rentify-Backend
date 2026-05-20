using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application.Contracts.Services;
using Rentify.Backend.Core.Application.Dtos.Common;
using Rentify.Backend.Core.Application.Dtos.RentCars;
using System.Net.Mime;

namespace Rentify.Backend.Presentation.WebApi.Controllers.V1;

[ApiVersion("1.0")]
public class RentCarsController : BaseApiController
{
    private readonly IRentCarService _rentCarService;

    public RentCarsController(IRentCarService rentCarService)
    {
        _rentCarService = rentCarService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedAsync([FromQuery] PaginationRequest request)
    {
        var result = await _rentCarService.GetPagedAsync(request);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _rentCarService.GetByIdAsync(id);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error!.Code switch
        {
            StatusCodes.Status404NotFound => NotFound(result.Error),
            StatusCodes.Status400BadRequest => BadRequest(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRentCarRequest request)
    {
        var result = await _rentCarService.CreateAsync(request);

        if (result.IsSuccess)
            return Created("created",result.Value);

        return BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRentCarRequest request)
    {
        var result = await _rentCarService.UpdateAsync(id, request);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error!.Code switch
        {
            StatusCodes.Status404NotFound => NotFound(result.Error),
            StatusCodes.Status400BadRequest => BadRequest(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }

    [HttpPatch("{id:guid}/disable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisableAsync(Guid id)
    {
        var result = await _rentCarService.DisableAsync(id);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error!.Code switch
        {
            StatusCodes.Status404NotFound => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }

    [HttpPatch("{id:guid}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnableAsync(Guid id)
    {
        var result = await _rentCarService.EnableAsync(id);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error!.Code switch
        {
            StatusCodes.Status404NotFound => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
}
