/// Database is not stored in DBMS
/// Resetting the application causes the reinitialization of memory, hence data lost
using Todo.Models;

namespace Todo.Repo
{
    public class InMemTaskRepo : ITask
    {

        private List<TodoTask> Tasks;

        public InMemTaskRepo()
        {
            Tasks = new List<TodoTask>();
        }

        public bool Create(TodoTask task)
        {
            if (task is null)
            {
                return false;
            }

            /// We do not want to modify an existing task
            /// Check if the given title already exists
            if (Tasks.Find(_task => String.Compare(_task.Title, task.Title, StringComparison.OrdinalIgnoreCase) == 0) != null)
            {
                return false;
            }


            Tasks.Add(task);
            return true; // task is successfully added
        }

        public bool Delete(Guid id)
        {
            TodoTask task = GetById(id);

            if (task is null)
            {
                return false;
            }
            
            Tasks.Remove(task);
            return true;
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public List<TodoTask> GetAll()
        {
            return Tasks;
        }

        public TodoTask GetById(Guid id)
        {
            #pragma warning disable CS8603 // Possible null reference return
            return Tasks.Find(x => x.Id == id); // If not found return null instead
            #pragma warning restore CS8603 // Possible null reference return
        }

        public bool Update(Guid id, TodoTask self)
        {
            /// Check if the given title exists
            TodoTask _task = GetById(id);
            if(_task is null)
            {
                return false; // task not found
            }

            _task.Title = self.Title;
            _task.Description = self.Description;
            _task.CreatedDate = DateTime.Now;
            return true; // task is successfully updated
        }

       
        public bool SetCompletion(Guid id, short progress)
        {
            // Progress value must be between 0 and 100
            if(progress < 0 || progress > 100)
            {
                return false;
            }

            TodoTask task = GetById(id);
            /// Check if the task exists
            if (task is null)
            {
                return false;
            }
            /// Set the completion
            task.Completion = progress;

            return true;
        }


        public bool SetDone(Guid id, bool isDone)
        {
            TodoTask task = GetById(id);
            /// Check if the task exists
            if (task is null)
            {
                return false;
            }
            /// Set to done or not
            task.Done = isDone;
            /// Make completion to 100
            // We do not want to call SetCompletion method to avoid an overhead
            task.Completion = 100; 

            return true;
        }



        /// <summary>
        /// Get the incoming tasks
        /// </summary>
        /// <param name="self">Given task to compare with</param>
        /// <param name="offSet">how many days we want to advance, 0 = today, 1 = 1 day in advance(tomorrow), 2 = two days in advance and so on</param>
        /// <returns></returns>
        public List<TodoTask> GetIncomingTasks(TodoTask self, short offSet = 0)
        {
            //No need to error checking task == null since we eventually return null if list is empty
            List<TodoTask> tasks = new List<TodoTask>();
            
            foreach (TodoTask _task in Tasks)
            {
                // Skip self comparison
                if (_task.Id == self.Id)
                {
                    continue;
                }
                /// Absolute value to avoid negative number
                if (Math.Abs((_task.CreatedDate.Date - self.CreatedDate.Date).Days) == offSet)
                {
                    tasks.Add(_task);
                }
            }

            return tasks;
        }




        /// <summary>
        /// Check whether two given weeks are falling into the same week
        /// </summary>
        /// <param name="date1">First Date to compare</param>
        /// <param name="date2">Second Date to compare</param>
        /// <param name="weekStartsOn">If the two date are in the same week</param>
        private bool AreFallingInSameWeek(DateTime date1, DateTime date2, DayOfWeek weekStartsOn = DayOfWeek.Monday)
        {
            /// Need to convert to .Date to avoid millisecond comparison 
            return date1.AddDays(-GetOffsetedDayofWeek(date1.DayOfWeek, (int)weekStartsOn)).Date == date2.AddDays(-GetOffsetedDayofWeek(date2.DayOfWeek, (int)weekStartsOn)).Date;
        }

        private int GetOffsetedDayofWeek(DayOfWeek dayOfWeek, int offsetBy)
        {
            return (((int)dayOfWeek - offsetBy + 7) % 7);
        }



        public List<TodoTask> GetThisWeekTaks(TodoTask self)
        {
            //No need to error checking task == null since we eventually return null if list is empty
            List<TodoTask> tasks = new List<TodoTask>();

            foreach (TodoTask _task in Tasks)
            {
                // Skip self comparison
                if (_task.Id == self.Id)
                {
                    continue;
                }
           
                if (AreFallingInSameWeek(_task.CreatedDate, self.CreatedDate))
                {
                    tasks.Add(_task);
                }
            }

            return tasks;
        }


    }

}