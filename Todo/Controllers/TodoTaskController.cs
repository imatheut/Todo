using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Dtos;
using Todo.Models;
using Todo.Repo;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoTaskController : ControllerBase
    {

        private ITask Tasks;


        public TodoTaskController(ITask tasks)
        {
            Tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        }

       
        /// Getting all the tasks available
        [HttpGet]
        public ActionResult<IEnumerable<TodoTaskDTO>> GetAll()
        {
            return Ok(Tasks.GetAll());
        }



        /// <summary>
        /// Getting a specific task searched by id
        /// </summary>
        [HttpGet("search/{id}")]
        public ActionResult<TodoTaskDTO> GetById(Guid id)
        {
            var _task = Tasks.GetById(id);
            if(_task is null)
            {
                return NotFound("Not Found");
            }

            return Ok(_task);
        }



        /// <summary>
        ///  Creating a new task
        /// </summary>
        /// <param name="task"> Task with only {title} and {description} fields</param>
        /// <returns> Bad request if title already exists or the task that just being created in case of success</returns>
        [HttpPost("create")]
        public ActionResult<TodoTaskDTO> CreateTask(CreateUpdateDTO task)
        {
            TodoTask _task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = task.Title,
                Description = task.Description,
                CreatedDate = DateTime.Now
            };

            bool ok = Tasks.Create(_task);

            /// Check if title already exists
            if(!ok)
            {
                return BadRequest("Title already exists");
            }

            return Ok(new TodoTask() { Id = _task.Id, Title = _task.Title, Description = _task.Description, CreatedDate = _task.CreatedDate });
        }

        /// <summary>
        /// Update a specific task by a given ID.
        /// </summary>
        /// <param name="id">Task's ID that we want to update</param>
        /// <param name="task">New Task's fields that we want to change</param>
        /// <returns>Not Found in case of non existing task, otherwise return the whole new updated task</returns>
        [HttpPatch("{id}/update")]
        public ActionResult<TodoTaskDTO> Update(Guid id, CreateUpdateDTO task)
        {
            TodoTask _task = new TodoTask();

            _task.Title = task.Title;
            _task.Description = task.Description;

            bool result = Tasks.Update(id, _task);

            /// If not exist
            if(!result)
            {
                return NotFound("Task does not exist");
            }

            return Ok(new TodoTaskDTO { Id = _task.Id, Title = _task.Title, CreatedDate = _task.CreatedDate, Description = _task.Description });
        }



        [HttpPatch("{id}/set_completion")]
        public ActionResult<bool> SetCompliteness(Guid id, CompletionDTO completion)
        {

            bool result = Tasks.SetCompletion(id, completion.Completion);
            if (!result)
            {
                return BadRequest("Bad request");
            }

            return Ok(true);

        }

        [HttpPatch("{id}/setdone")]
        public ActionResult<bool> SetDone(Guid id, DoneDTO isDone)
        {

            bool result = Tasks.SetDone(id, isDone.IsDone);
            if (!result)
            {
                return BadRequest("Task does not exist");
            }

            return Ok(result);

        }


        [HttpDelete("{id}/delete")]
        public ActionResult<bool> Delete(Guid id)
        {
            bool result = Tasks.Delete(id);
            if(!result)
            {
                return BadRequest("Task does not exist");
            }

            return Ok(result);

        }

        /// <summary>
        /// Get the incoming tasks
        /// </summary>
        /// <param name="self">Given task to compare with</param>
        /// <param name="offSet">how many days we want to advance, 0 = today, 1 = 1 day in advance(tomorrow), 2 = two days in advance and so on</param>
        /// <returns></returns>
        private List<TodoTaskDTO> GetIncomingTaks(TodoTask self, short offSet = 0)
        {
            List<TodoTask> tasks = Tasks.GetIncomingTasks(self, offSet);

            List<TodoTaskDTO> results = new List<TodoTaskDTO>();

            foreach (TodoTask _task in tasks)
            {
                results.Add(new TodoTaskDTO
                {
                    Id = _task.Id,
                    CreatedDate = _task.CreatedDate,
                    Completion = _task.Completion,
                    Title = _task.Title,
                    Description = _task.Description
                });
            }
            // We return even with empty collection
            return results;
        }



        [HttpGet("{id}/task-same-day")]
        public ActionResult<List<TodoTaskDTO>> GetTaskGetTasksOfTheDay(Guid id)
        {

            TodoTask task = Tasks.GetById(id);
            if(task is null)
            {
                return NotFound("Task Not Found");
            }
            List<TodoTaskDTO> results = GetIncomingTaks(task);

            
            // We return even with empty collection
            return Ok(results);
        }



        [HttpGet("{id}/task-next-day")]
        public ActionResult<List<TodoTaskDTO>> GetTaskOfNextDay(Guid id)
        {
            TodoTask task = Tasks.GetById(id);
            if (task is null)
            {
                return NotFound("Task Not Found");
            }
            // Set offset to 1(for next day)
            List<TodoTaskDTO> results = GetIncomingTaks(task, 1);


            // We return even with empty collection
            return Ok(results);
        }



        [HttpGet("{id}/task-this-week")]
        public ActionResult<List<TodoTaskDTO>> GetTaskOfThisWeek(Guid id)
        {
            TodoTask task = Tasks.GetById(id);
            if (task is null)
            {
                return NotFound("Task Not Found");
            }
            List<TodoTask> tasks = Tasks.GetThisWeekTaks(task); 

            List<TodoTaskDTO> results = new List<TodoTaskDTO>();

            foreach (TodoTask _task in tasks)
            {
                results.Add(new TodoTaskDTO
                {
                    Id = _task.Id,
                    CreatedDate = _task.CreatedDate,
                    Completion = _task.Completion,
                    Title = _task.Title,
                    Description = _task.Description
                });
            }
            
            // We return even with empty collection
            return Ok(results);
        }




    }
}
