using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskmanager_api_dotnet.Data;
using taskmanager_api_dotnet.DTOs;
using taskmanager_api_dotnet.Models;

namespace taskmanager_api_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // Implement CRUD operations for TaskItem here
        // Endpoints:

        //GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            // Transforma a lista de Models em lista de DTOs
            return Ok(tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                UserId = t.UserId
            }));
        }

        //GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }

        //POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> PostTask(TaskRequestDto taskDto)
        {
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                IsCompleted = taskDto.IsCompleted,
                UserId = taskDto.UserId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, taskDto);
        }

        //PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskItem task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }
            _context.Entry(task).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tasks.Any(equals => equals.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //204 - No Content
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            //204 - No Content
            return NoContent();
        }
    }
}
