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

        // GET: api/tasks/user/{userId}
        [HttpGet("user/{userId}")] 
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByUserId(int userId)
        {
            var tasks = await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();

            if (tasks == null || !tasks.Any())
            {
                return NotFound($"Nenhuma tarefa encontrada para o usuário {userId}.");
            }

            return Ok(tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                UserId = t.UserId
            }));
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> PostTask(TaskRequestDto taskDto)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.Id == taskDto.UserId);

            if (!userExists)
            {
                return BadRequest($"Usuário com Id {taskDto.UserId} não existe.");
            }

            var newTask = new TaskItem
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                IsCompleted = taskDto.IsCompleted,
                UserId = taskDto.UserId,
                CreatedAt = DateTime.Now 
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            var response = new TaskResponseDto
            {
                Id = newTask.Id,
                Title = newTask.Title,
                Description = newTask.Description,
                IsCompleted = newTask.IsCompleted,
                UserId = newTask.UserId
            };

            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, response);
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
