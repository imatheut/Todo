/// <summary>
/// Interface to implement CRUD operations
/// </summary>

using Microsoft.AspNetCore.Mvc;
using Todo.Models;

namespace Todo.Repo
{
    public interface ITask
    {
        public bool Create(TodoTask task);                      // Create a task, return true if the element has successfully created
        public List<TodoTask> GetAll();                         // Get all task  
        public bool Update(Guid id, TodoTask task);             // Update a specific task by id, return true if the element has successfully added
        public bool Delete(Guid id);                            // Update a specific task by id, return true if the element has successfully deleted   
        public void DeleteAll();                                // Delete all task available
        public TodoTask GetById(Guid id);                       // Get a specific task by id
        public bool SetCompletion(Guid id, short progress);     // Set completion
        public bool SetDone(Guid id, bool isDone);              // Set to done or not
        public List<TodoTask> GetIncomingTasks(TodoTask self, short offSet = 0); // Get incoming task
        public List<TodoTask> GetThisWeekTaks(TodoTask self);   // Get all the tasks from the current week

    }
}