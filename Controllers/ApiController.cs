using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

[ApiController]
[Route("api/items")]
public class ApiController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ApiController> _logger;

    public ApiController(ApplicationDbContext dbContext, ILogger<ApiController> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemDto>> GetById(int id)
    {
        var entity = await _dbContext.Items.FindAsync(id);
        if (entity != null)
        {
            return Ok(entity);
        }

        return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemDto>>> GetAll(string? search)
    {
        var query = _dbContext.Items.AsQueryable();
        if (search != null)
        {
            query = query.Where(x => x.Value != null && x.Value.Contains(search));
        }

        var entities = await query.ToListAsync();
        var dtos = entities.Select(x => new ItemDto
        {
            Code = x.Code,
            Value = x.Value
        })
        .ToList();

        return dtos;
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> Create(List<ItemDto> dtos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var newItems = dtos
            .OrderBy(x => x.Code)
            .Select(x => new Item
            {
                Code = x.Code.Value,
                Value = x.Value
            })
            .ToArray();

        var currentItems = await _dbContext.Items.ToListAsync();
        _dbContext.Items.RemoveRange(currentItems);
        _dbContext.Items.AddRange(newItems);

        try
        {
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), newItems);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}