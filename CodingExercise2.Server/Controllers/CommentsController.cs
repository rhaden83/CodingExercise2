using CodingExercise2.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodingExercise2.Server.Controllers
{
    /// <summary>
    /// The CommentsController class serves as a web-API controller for posting and viewing comments.
    /// </summary>
    [ApiController, Route("api/comments")]
    public class CommentsController : ControllerBase
    {
        private const int UsernameMaxLength = 15;
        private const int ContentMaxLength = 140;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
        };

        private readonly DatabaseContext _databaseContext;
        private readonly IHubContext<CommentsHub> _hub;

        public CommentsController(DatabaseContext databaseContext, IHubContext<CommentsHub> hub)
        {
            _databaseContext = databaseContext;
            _hub = hub;
        }

        [HttpGet]
        public IEnumerable<Comment> GetComments()
        {
            return _databaseContext.Comments.OrderByDescending(c => c.CreationTimestamp);
        }

        [HttpPost]
        public ActionResult AddComment(AddCommentRequest request)
        {
            if((request.Username?.Length ?? 0) == 0)
            {
                return BadRequest("A username is required.");
            }
            else if(request.Username.Length > UsernameMaxLength)
            {
                return BadRequest(string.Format("A username must not exceed {0} characters.", UsernameMaxLength));
            }
            else if((request.Comment?.Length ?? 0) == 0)
            {
                return BadRequest("A comment is required.");
            }
            else if(request.Comment.Length > ContentMaxLength)
            {
                return BadRequest(string.Format("A comment must not exceed {0} characters.", ContentMaxLength));
            }

            var comment = new Comment { Username = request.Username, Content = request.Comment, CreationTimestamp = DateTime.Now };
            _databaseContext.Comments.Add(comment);
            _databaseContext.SaveChanges();
            _hub.Clients.All.SendAsync("broadcast", JsonConvert.SerializeObject(comment, JsonSerializerSettings));

            return NoContent();
        }
    }
}