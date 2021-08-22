using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using todozulo.Common.Models;
using todozulu.Functions.Entities;

namespace todozulu.Test.Helpers
{
    public class TestFactory
    {
       

        public static TodoEntity GetTodoEntity() {

            return new TodoEntity
            {
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Task : kill the humans"
               
            };
        
        }

   

        public static DefaultHttpRequest CreateHttpRequest(Guid TodoId, Todo todoRequest) {

            string request = JsonConvert.SerializeObject(todoRequest);
           return new  DefaultHttpRequest (new DefaultHttpContext())
           {

               Body = GenerateStreamFromString(request),
               Path = $"/{TodoId}"
           };
        }







        public static DefaultHttpRequest CreateHttpRequest(Guid todoId)
        {

           
            return new DefaultHttpRequest(new DefaultHttpContext())
            {

                
                Path = $"/{todoId}"
            };
        }


        public static DefaultHttpRequest CreateHttpRequest(Todo todoRequest)
        {

            string request = JsonConvert.SerializeObject(todoRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {

                Body = GenerateStreamFromString(request),
                
            };
        }


        public static DefaultHttpRequest CreateHttpRequest()
        {

            return new DefaultHttpRequest(new DefaultHttpContext());
          
        }


        public static Todo TodoRequest => new Todo
        {
            CreatedTime = DateTime.UtcNow,
            IsCompleted = false,
            TaskDescription = "Try to conquer  the world"
        };




        public  static Stream GenerateStreamFromString(string stringToconvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter write = new StreamWriter(stream);
            write.Write(stringToconvert);
            write.Flush();
            stream.Position = 0;
            return stream;
        }


        public static ILogger createlogger(LoggerTypes type = LoggerTypes.Null) {
            ILogger logguer;

            if (type == LoggerTypes.list)
            {

                logguer = new ListLogger();
            }
            else {

                logguer = NullLoggerFactory.Instance.CreateLogger("Null logger");
            
            }

            return logguer;
        
        }

        internal class GetTodoRequest : Todo
        {
        }
    }
}
