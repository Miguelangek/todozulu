using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using todozulu.Functions.Entities;

namespace todozulu.Functions.Functions
{
    public static class ScheduledFunction
    {
        private static IEnumerable<TodoEntity> completedTodos;

        [FunctionName("ScheduledFunction")]
        public static async Task Run(
           [TimerTrigger("0 * /2 * * * *")]TimerInfo myTimer,
           [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
           ILogger log)
          
        {   
            log.LogInformation($"Deleted completed function executed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("IsCompleted", QueryComparisons.Equal, true);
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>().Where(filter);
            TableQuerySegment<TodoEntity> completedTodo = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            int  deleted = 0;
            foreach (TodoEntity completedTodos in completedTodos)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(completedTodos));
                deleted++;
            }
            log.LogInformation($"Deleted: {deleted} items  at: {DateTime.Now}");
        }
    }
}
