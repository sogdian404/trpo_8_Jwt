using _8.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _8.Models;
namespace _8.Controllers
{
    [ApiController]
    [Route("api/v1/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        public record TaskCreateRequest(string Title, string Description);
        public record TaskUpdateRequest(string Title, string Description, bool IsCompleted);


        [HttpGet]
        public IActionResult GetTasks()
        {
            var currentLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            var query = _context.Tasks.AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(t => t.OwnerLogin == currentLogin);
            }

            var tasks = query.ToList();
            return Ok(tasks);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllTasks()
        {
            return Ok(_context.Tasks.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            var currLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            if (task.OwnerLogin != currLogin && !isAdmin)
                return Forbid();

            return Ok(task);
        }
        [HttpPost]
        public IActionResult CreateTask(TaskCreateRequest request)
        {
            var currLogin = User.Identity?.Name;

            var task = new Models.Task
            {
                Title = request.Title,
                Description = request.Description,
                OwnerLogin = currLogin,
                IsCompleted = false
            };
            _context.Tasks.Add(task);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskUpdateRequest request)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
        
            if (task == null) 
                return NotFound();

            var currLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            if (task.OwnerLogin != currLogin && !isAdmin)
                return Forbid();

            task.Title = request.Title;
            task.Description = request.Description;
            task.IsCompleted = request.IsCompleted;

            _context.SaveChanges();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            var currentLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            if (task.OwnerLogin != currentLogin && !isAdmin)
                return Forbid();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
