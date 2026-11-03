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

        public record TaskCreateRequest(string Title, string Descrition);
        public record TaskUpdateRequest(string Title, string Descrition, bool isCompleted);


        [HttpGet]
        public IActionResult GetTasks()
        {
            var currentLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Админ");

            var query = _context.Tasks.AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(t => t.OwnerLogin == currentLogin);
            }

            var tasks = query.ToList();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            var currLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Админ");

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
                Description = request.Descrition,
                OwnerLogin = currLogin,
                IsCompleted = false
            };
            _context.Tasks.Add(task);
            _context.SaveChanges();

            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskUpdateRequest request)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
        
            if (task == null) 
                return NotFound();

            var currLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Админ");

            if (task.OwnerLogin != currLogin && !isAdmin)
                return Forbid();

            task.Title = request.Title;
            task.Description = request.Descrition;
            task.IsCompleted = request.isCompleted;

            _context.SaveChanges();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            var currentLogin = User.Identity?.Name;
            var isAdmin = User.IsInRole("Админ");

            if (task.OwnerLogin != currentLogin && !isAdmin)
                return Forbid();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
