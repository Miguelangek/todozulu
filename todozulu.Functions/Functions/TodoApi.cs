using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using todozulo.Common.Models;
using todozulo.Common.Responses;
using todozulu.Functions.Entities;

namespace todozulu.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable TodoTable,
            ILogger log)
        {
            log.LogInformation("Recivied a new todo.");

           

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo Todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(Todo?.TaskDescription)){
                return new BadRequestObjectResult(new Response
                {
                    ItSuccess = false,
                    Message = "The request must have a TaskDescription"
                });
            }

            TodoEntity todoEntity = new TodoEntity
            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false, 
                PartitionKey ="TODO",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription =   Todo.TaskDescription

            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await TodoTable.ExecuteAsync(addOperation);

            string message = "new todo stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response { 
            ItSuccess = true,
            Message =message,
            Result = todoEntity
            
            });



            
        }


        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
          [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
          [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable TodoTable,
          string id,
          ILogger log)  
        {
            log.LogInformation($"update for todo: {id}, received");
           
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo Todo = JsonConvert.DeserializeObject<Todo>(requestBody);


            //validate todo id
            TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);
            TableResult findResult = await TodoTable.ExecuteAsync(findOperation);

            if (findResult.Result == null) {
                return new BadRequestObjectResult(new Response
                {
                    ItSuccess = false,
                    Message = "Todo not found"
                });
            }

            //update todo
            TodoEntity todoEntity = (TodoEntity)findResult.Result;
            todoEntity.IsCompleted = Todo.IsCompleted;
            if (string.IsNullOrEmpty(Todo.TaskDescription))
            {
                todoEntity.TaskDescription = Todo.TaskDescription;
            }

          

           

            TableOperation addOperation = TableOperation.Replace(todoEntity);
            await TodoTable.ExecuteAsync(addOperation);

            string message = $"Todo {id}, update in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                ItSuccess = true,
                Message = message,
                Result = todoEntity

            });



           
        }



        [FunctionName(nameof(GetAllTodos))]
        public static async Task<IActionResult> GetAllTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable TodoTable,
            ILogger log)
        {
            log.LogInformation("Get all todos received");


            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> todos = await TodoTable.ExecuteQuerySegmentedAsync(query, null);
          

            string message = "retrieved all todos";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                ItSuccess = true,
                Message = message,
                Result = todos

            });



           
        }


        [FunctionName(nameof(GetTodoById))]
        public static IActionResult  GetTodoById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
           [Table("todo", "TODO", "{id}",Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
           string id,
           ILogger log)
        {
            log.LogInformation($"Get todo By Id: {id} , received");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    ItSuccess = false,
                    Message = "Todo not found"
                });
            }



            string message = $"todo {todoEntity.RowKey}, retrieved";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                ItSuccess = true,
                Message = message,
                Result = todoEntity

            });




        }



        [FunctionName(nameof(DeleteTodo))]
        public static async Task<IActionResult> DeleteTodo(
          [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
          [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
           [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
          string id,
          ILogger log)
        {
            log.LogInformation($"Delete todo : {id} , received");

            

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    ItSuccess = false,
                    Message = "Todo not found"
                });
            }


            await todoTable.ExecuteAsync(TableOperation.Delete(todoEntity));
            string message = $"todo {todoEntity.RowKey}, deleted";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                ItSuccess = true,
                Message = message,
                Result = todoEntity

            });




        }




    }

}
