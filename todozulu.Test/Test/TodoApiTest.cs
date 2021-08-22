using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using todozulo.Common.Models;
using todozulu.Functions.Functions;
using todozulu.Test.Helpers;
using Xunit;

namespace todozulu.Test.Test
{
   public  class TodoApiTest
    {
        private readonly ILogger logger = TestFactory.createlogger();
        private object todoId;

        [Fact]
        public async void CreateTodo_Sshould_Return_200() {

            //Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.TodoRequest;
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            //Act
            IActionResult response = await TodoApi.CreateTodo(request, mockTodos, logger);
            //Asser 
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }


        [Fact]
        public async void UpdateTodo_Sshould_Return_200()
        {

            //Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.TodoRequest;
             Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            //Act
            IActionResult response = await TodoApi.UpdateTodo(request, mockTodos, todoId.ToString(), logger);
            //Asser 
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }


    }
}
