using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application.Interfaces.Services;
using Rentify.Backend.Infraestructure.Identity.Interfaces;
using Rentify.Backend.Core.Application.Dtos.Roles;
using System.Net.Mime;

namespace Rentify.Backend.Presentation.WebApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
public class RolController : BaseApiController
{
    private readonly IRolService _rolService;

    public RolController(IRolService rolService)
    {
        _rolService = rolService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _rolService.GetRolesAsync();

        if (response != null && response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return response.Error.Code switch
        {
            StatusCodes.Status404NotFound => NotFound(response.Error.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error.Message)
        };
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        try
        {
            var response = await _rolService.GetRoleByIdAsync(id);

            if (response != null)
            {
                return Ok(response);
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateAsync([FromBody] SaveRoleRequest request)
    {
        try
        {
            var response = await _rolService.AddRoleAsync(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateAsync(string id, [FromBody] SaveRoleRequest request)
    {
        try
        {
            var response = await _rolService.UpdateRoleAsync(id, request);
            if (response != null)
            {
                return Ok(response);
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        try
        {
            await _rolService.DeleteRoleAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}