using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Todo.Controllers;
using Todo.Dtos;
using Todo.Models;
using Todo.Repo;

namespace TodoTaskNUnitTests
{
    public class TodoTaskTest
    {

        //----------------------------- GET ALL SECTION ---------------------------------//
        [Test]
        public void Get_All_Null_Exception()
        {   
            Assert.Throws<ArgumentNullException>(() => new TodoTaskController(null));
        }
       

        [Test]
        public void Get_All_Tasks_Return_Empty_list()
        {

            // ARRANGE
            ITask tasks = new InMemTaskRepo();
            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetAll();
            var result = actionResult.Result as OkObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            Assert.IsEmpty(returnTodoTasks);
            Assert.IsNotNull(result);  
            Assert.AreEqual(200, result.StatusCode);  // Server always return 200 whether the list is empty or not
        }




        [Test]
        public void Get_All_Tasks_With_One_Added_Task()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();
            tasks.Create(new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = DateTime.Now
            });

            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetAll();
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            Assert.IsNotEmpty(returnTodoTasks);
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);  // Server always return 200 whether the list is empty or not
            Assert.AreEqual(1, returnTodoTasks.Count);
        }





        [Test]
        public void Get_All_Integrity_Result_With_MultipleTasks_Added()
        {
            // ARRANGE
            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is the second task",
                CreatedDate = DateTime.Now
            };


            ITask tasks = new InMemTaskRepo();
            tasks.Create(firstTask);
            tasks.Create(secondTask);

            TodoTaskController controller = new TodoTaskController(tasks);

            // ACT + ASSERT
            var actionResult = controller.GetAll();

            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            // Validity return tests
            Assert.IsNotEmpty(returnTodoTasks);
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);  // Server always return 200 whether the list is empty or not


            // There are 2 tasks in total
            Assert.AreEqual(2, returnTodoTasks.Count);

            // First Task
            Assert.AreEqual(returnTodoTasks[0].Title, firstTask.Title);
            Assert.AreEqual(returnTodoTasks[0].Id, firstTask.Id);
            

            // Second Task
            Assert.AreEqual(returnTodoTasks[1].Title, secondTask.Title);
            Assert.AreEqual(returnTodoTasks[1].Id, secondTask.Id);

        }

        //--------------------------- END OF GET ALL SECTION --------------------------------------//



        //------------------------------- GET BY ID SECTION --------------------------------------//
        [Test]
        public void Get_Task_By_Id_With_Non_Existing_Task()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();
            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetById(Guid.NewGuid());
            var result = actionResult.Result as ObjectResult;
          
        
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }



        [Test]
        public void Get_Task_By_Id_With_Existing_Tasks_Found()
        {
            // ARRANGE
            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is the second task",
                CreatedDate = DateTime.Now
            };

            ITask tasks = new InMemTaskRepo();
            tasks.Create(firstTask);
            tasks.Create(secondTask);

            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT FIRST
            var actionResult = controller.GetById(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);



            // ACT + ASSERT SECOND
            // Reassigning all the data with the second task
            actionResult = controller.GetById(secondTask.Id); 
            result = actionResult.Result as ObjectResult;
            returnTodoTasks = result.Value as List<TodoTask>;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }


        //------------------------------- CREATE SECTION --------------------------------------//

        [Test]
        public void Create_Task_Simple()
        {


            // ARRANGE
            CreateUpdateDTO task = new CreateUpdateDTO()
            {
                Title = "First",
                Description = "First Task"
            };

            TodoTaskController controller = new TodoTaskController(new InMemTaskRepo());



            // ACT + ASSERT
            var actionResult = controller.CreateTask(task);
            var result = actionResult.Result as ObjectResult;
            var returnResult = result.Value as TodoTask;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);


            // Check if the item has been added to the storage(in this case in memory)
            var actionResult_GetAll = controller.GetAll();
            var result_GetAll = actionResult_GetAll.Result as ObjectResult;
            var returnTodoTasks = result_GetAll.Value as List<TodoTask>;


            Assert.IsNotEmpty(returnTodoTasks);
            Assert.IsNotNull(result_GetAll);
            Assert.AreEqual(200, result_GetAll.StatusCode); 
            Assert.AreEqual(1, returnTodoTasks.Count);


            // Check By Id
            var actionResult_GetById = controller.GetById(returnResult.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);

            // Check if the data matches with the DTO
            Assert.AreEqual(returnTaskById.Title, task.Title);
            Assert.AreEqual(returnTaskById.Description, task.Description);
        }




        [Test]
        public void Create_With_Already_Exist_Task_With_Same_Title()
        {
            // ARRANGE
            CreateUpdateDTO task = new CreateUpdateDTO()
            {
                Title = "First",
                Description = "First Task"
            };

            CreateUpdateDTO sTask= new CreateUpdateDTO()
            {
                Title = "First",
                Description = "First Task Modified"
            };

            TodoTaskController controller = new TodoTaskController(new InMemTaskRepo());



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.CreateTask(task);
            var result = actionResult.Result as ObjectResult;
            var returnResult = result.Value as TodoTask;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);


            // ACT + ASSERT FIRST_TASK
            actionResult = controller.CreateTask(sTask);
            result = actionResult.Result as ObjectResult;
          
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);




            // Check By Id first task
            var actionResult_GetById = controller.GetById(returnResult.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);
            

            // Check if the data is not affected by the creation of the same id with the DTO
            Assert.AreNotEqual(returnTaskById.Description, sTask.Description);

        }


        //---------------------------- END OF CREATE SECTION -----------------------------------//


    


        //------------------------------- UPDATE SECTION --------------------------------------//

        [Test]
        public void Update_Non_Existing_Task()
        {
            // ARRANGE
            CreateUpdateDTO task = new CreateUpdateDTO()
            {
                Title = "Not Exist",
                Description = "Id not exist"
            };

            TodoTaskController controller = new TodoTaskController(new InMemTaskRepo());


            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.Update(Guid.NewGuid(), task);
            var result = actionResult.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

        }



        [Test]
        public void Update_Existing_Task()
        {
            // ARRANGE
            CreateUpdateDTO updateComponent = new CreateUpdateDTO()
            {
                Title = "Exist",
                Description = "Id exists"
            };

            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is the second task",
                CreatedDate = DateTime.Now
            };

            ITask tasks = new InMemTaskRepo();
            tasks.Create(firstTask);
            tasks.Create(secondTask);

            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.Update(firstTask.Id, updateComponent);
            var result = actionResult.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(firstTask.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);


            // Check if the data is not affected by the creation of the same id with the DTO
            Assert.AreEqual(returnTaskById.Title, updateComponent.Title);
            Assert.AreEqual(returnTaskById.Description, updateComponent.Description);


        }


        //------------------------------- END OF UPDATE SECTION ------------------------------------//



        //------------------------------- COMPLETION SECTION --------------------------------------//

        [TestCase(-5)]
        [TestCase(101)]
        public void Set_Completion_With_Invalid_Value(short _completion)
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };
      

            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);

           

            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.SetCompliteness(task.Id, new CompletionDTO { Completion = _completion });
            var result = actionResult.Result as ObjectResult;

            //Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);


            // Check if the data is not affected by the creation of the same id with the DTO
            Assert.AreNotEqual(returnTaskById.Completion, _completion);
        }





        [Test]
        public void Set_Completion_With_Non_Existing_Task_Correct_Value()
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };
            short _completion = 85;


            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.SetCompliteness(Guid.NewGuid(), new CompletionDTO { Completion = _completion });
            var result = actionResult.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);


            // Check if the data is not affected by the creation of the same id with the DTO
            Assert.AreNotEqual(returnTaskById.Completion, _completion);

        }


        [Test]
        public void Set_Completion_With_Valid_Input()
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };

            short _completion = 85;

            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.SetCompliteness(task.Id, new CompletionDTO { Completion = _completion });
            var result = actionResult.Result as ObjectResult;
        

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);


            // Check if the data is affected
            Assert.AreEqual(returnTaskById.Completion, _completion);

        }




        //------------------------------ END OF COMPLETION SECTION ---------------------------------//


        //---------------------------------- DONE SECTION ----------------------------------------//

        [Test]
        public void Set_Done_With_Non_Existing_Task()
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };

            bool _isDone = true;


            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.SetDone(Guid.NewGuid(), new DoneDTO { IsDone = _isDone });
            var result = actionResult.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);


            // Check if the data is not affected by the creation of the same id with the DTO
            // False is the default value
            Assert.AreNotEqual(returnTaskById.Done, _isDone);
        }


        [Test]
        public void Set_Done_With_Valid_Input()
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };
            bool _isDone = true;


            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.SetDone(task.Id, new DoneDTO { IsDone = _isDone });
            var result = actionResult.Result as ObjectResult;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
            var returnTaskById = result_GetById.Value as TodoTask;


            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);


            // Check if the data is affected by the creation of the same id with the DTO
            Assert.AreEqual(returnTaskById.Done, _isDone);

        }



        //------------------------------- END OFDONE SECTION ----------------------------------------//



        //---------------------------------- DELETE SECTION ----------------------------------------//

        [Test]
        public void Delete_Non_Existing_Task()
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };



            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.Delete(Guid.NewGuid());
            var result = actionResult.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
       


            // Task still exists
            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(200, result_GetById.StatusCode);
        }




        [Test]
        public void Delete_An_Existing_Task()
        {
            TodoTask task = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is the first task",
                CreatedDate = DateTime.Now
            };



            ITask tasks = new InMemTaskRepo();
            tasks.Create(task);


            TodoTaskController controller = new TodoTaskController(tasks);



            // ACT + ASSERT FIRST_TASK
            var actionResult = controller.Delete(task.Id);
            var result = actionResult.Result as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);


            // Check By Id first task
            var actionResult_GetById = controller.GetById(task.Id);
            var result_GetById = actionResult_GetById.Result as ObjectResult;
          


            // Task still exists
            Assert.IsNotNull(result_GetById);
            Assert.AreEqual(404, result_GetById.StatusCode);
        }



        //------------------------------- END OF DELETE SECTION ------------------------------------//



        //------------------------------- INCOMING TASK SECTION ------------------------------------//

        [Test]
        public void Get_All_Tasks_SameDay_Invalid_Id()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 8)
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);
          

            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskGetTasksOfTheDay(Guid.NewGuid());
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }


        [Test]
        public void Get_All_Tasks_SameDay_Empty()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022,5,9),
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 6, 9),
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2023, 5, 9),
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);
            tasks.Create(thirdTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskGetTasksOfTheDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            //Expected 0, even though they are all have the same day but they do not have the exact same month/year
            Assert.AreEqual(0, returnTodoTasks.Count);

        }





        [Test]
        public void Get_All_Tasks_SameDay_Valid_Id_Empty_List()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 7)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is third test",
                CreatedDate = new DateTime(2022, 4, 8)
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskGetTasksOfTheDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            // Expected 0, Nothing matches
            Assert.AreEqual(0, returnTodoTasks.Count);
        }



        [Test]
        public void Get_All_Tasks_Same_Day_Valid()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 7)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 7)
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is third test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            tasks.Create(firstTask);
            tasks.Create(secondTask);
            tasks.Create(thirdTask);



            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskGetTasksOfTheDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Expected 1, Only first and second task have the same day
            Assert.AreEqual(1, returnTodoTasks.Count);
            Assert.AreEqual(secondTask.Id, returnTodoTasks[0].Id);
        }


        // -----------------------------NEXT DAY

        [Test]
        public void Get_Tasks_Of_Next_Day_Invalid_Id()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 8)
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfNextDay(Guid.NewGuid());
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }



        [Test]
        public void Get_All_Tasks_Next_Empty()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 5, 9),
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 6, 10),
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2023, 6, 9),
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);
            tasks.Create(thirdTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfNextDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            //Expected 0, even though they are all have 1 day apart from each other but they do not have the exact same month/year
            Assert.AreEqual(0, returnTodoTasks.Count);

        }





        [Test]
        public void Get_All_Tasks_NextDay_Valid_Id_Empty_List()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 10)
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is third test",
                CreatedDate = new DateTime(2022, 4, 10)
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfNextDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            // Expected 0, Nothing matches
            Assert.AreEqual(0, returnTodoTasks.Count);
        }



        [Test]
        public void Get_All_Tasks_Next_Day_Valid()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 9)
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is third test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            tasks.Create(firstTask);
            tasks.Create(secondTask);
            tasks.Create(thirdTask);



            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfNextDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Expected 1, Only first and second task match
            Assert.NotNull(returnTodoTasks);
            Assert.AreEqual(1, returnTodoTasks.Count);
            Assert.AreEqual(secondTask.Id, returnTodoTasks[0].Id);
        }






        // ----------------------------- THIS WEEK

        [Test]
        public void Get_Tasks_Of_This_Week_Invalid_Id()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 9)
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfThisWeek(Guid.NewGuid());
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTask>;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }



        [Test]
        public void Get_All_Tasks_This_Week_Empty()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 5, 9),
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 6, 10),
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2023, 6, 9),
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);
            tasks.Create(thirdTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfThisWeek(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            //Expected 0, even though they are all in the same week but they do not have the exact same month/year
            Assert.AreEqual(0, returnTodoTasks.Count);

        }





        [Test]
        public void Get_All_Tasks_This_Week_Valid_Id_Empty_List()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 11)
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is third test",
                CreatedDate = new DateTime(2022, 4, 12)
            };


            tasks.Create(firstTask);
            tasks.Create(secondTask);


            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfThisWeek(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            // Expected 0, first task is not in the same week with second and third tasks
            Assert.AreEqual(0, returnTodoTasks.Count);
        }



        [Test]
        public void Get_All_Tasks_ThisWeek_Valid()
        {
            // ARRANGE
            ITask tasks = new InMemTaskRepo();


            TodoTask firstTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "First Test",
                Description = "This is a test",
                CreatedDate = new DateTime(2022, 4, 8)
            };

            TodoTask secondTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Second Test",
                Description = "This is second test",
                CreatedDate = new DateTime(2022, 4, 9)
            };

            TodoTask thirdTask = new TodoTask()
            {
                Id = Guid.NewGuid(),
                Title = "Third Test",
                Description = "This is third test",
                CreatedDate = new DateTime(2022, 4, 10)
            };

            tasks.Create(firstTask);
            tasks.Create(secondTask);
            tasks.Create(thirdTask);



            TodoTaskController controller = new TodoTaskController(tasks);


            // ACT + ASSERT
            var actionResult = controller.GetTaskOfNextDay(firstTask.Id);
            var result = actionResult.Result as ObjectResult;
            var returnTodoTasks = result.Value as List<TodoTaskDTO>;


            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Expected 1, Only first and second task match
            // Third task is week since we start the week from Monday
            Assert.AreEqual(1, returnTodoTasks.Count);
            Assert.AreEqual(secondTask.Id, returnTodoTasks[0].Id);
        }



        //-----------------------------END INCOMING TASK SECTION ------------------------------------//

    }
}