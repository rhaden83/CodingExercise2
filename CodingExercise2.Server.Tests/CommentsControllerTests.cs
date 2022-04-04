using CodingExercise2.Server.Controllers;
using CodingExercise2.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Data.Common;
using System.Linq;

namespace CodingExercise2.Server.Tests
{
    public class CommentsControllerTests
    {
        private readonly DbConnection _dbConnection;
        private readonly DatabaseContext _databaseContext;
        private readonly CommentsController _controller;

        public CommentsControllerTests()
        {
            _dbConnection = new SqliteConnection("Filename=:memory:");
            _dbConnection.Open();

            var contextOptions = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite(_dbConnection).Options;
            _databaseContext = new DatabaseContext(contextOptions);

            _databaseContext.Comments.Add(new Comment { Username = "Eddard", Content = "Stark", CreationTimestamp = DateTime.Now.AddDays(-1) });
            _databaseContext.Comments.Add(new Comment { Username = "Jon", Content = "Snow", CreationTimestamp = DateTime.Now });
            _databaseContext.SaveChanges();

            var mockHubClients = new Mock<IHubClients>();
            mockHubClients.Setup(c => c.All).Returns(new Mock<IClientProxy>().Object);

            var mockHub = new Mock<IHubContext<CommentsHub>>();
            mockHub.Setup(h => h.Clients).Returns(mockHubClients.Object);

            _controller = new CommentsController(_databaseContext, mockHub.Object);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _databaseContext.Dispose();
            _dbConnection.Dispose();
        }

        [Test, Order(1)]
        public void GetCommentsTest()
        {
            var results = _controller.GetComments().ToList();
            Assert.AreEqual("Jon", results[0].Username);
            Assert.AreEqual("Snow", results[0].Content);
            Assert.AreEqual("Eddard", results[1].Username);
            Assert.AreEqual("Stark", results[1].Content);
        }

        [Test]
        public void AddCommentTest_NullUsername()
        {
            var result = _controller.AddComment(new AddCommentRequest { Comment = "Seaworth" }) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("A username is required.", result.Value);
        }

        [Test]
        public void AddCommentTest_EmptyUsername()
        {
            var result = _controller.AddComment(new AddCommentRequest { Username = "", Comment = "Seaworth" }) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("A username is required.", result.Value);
        }

        [Test]
        public void AddCommentTest_TooLongUsername()
        {
            var result = _controller.AddComment(new AddCommentRequest { Username = "Stannnnnnnnnnnis", Comment = "Baratheon" }) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("A username must not exceed 15 characters.", result.Value);
        }

        [Test]
        public void AddCommentTest_NullContent()
        {
            var result = _controller.AddComment(new AddCommentRequest { Username = "Davos" }) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("A comment is required.", result.Value);
        }

        [Test]
        public void AddCommentTest_EmptyContent()
        {
            var result = _controller.AddComment(new AddCommentRequest { Username = "Davos", Comment = "" }) as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("A comment is required.", result.Value);
        }

        [Test]
        public void AddCommentTest_TooLongContent()
        {
            var result = _controller.AddComment(new AddCommentRequest
            {
                Username = "Hodor",
                Comment = "Hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor hodor"
            }) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("A comment must not exceed 140 characters.", result.Value);
        }

        [Test]
        public void AddCommentTest_Successful()
        {
            var result = _controller.AddComment(new AddCommentRequest { Username = "Davos", Comment = "Seaworth" }) as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
            Assert.AreEqual(3, _databaseContext.Comments.Count());
        }
    }
}