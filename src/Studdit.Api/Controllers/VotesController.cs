using MediatR;
using Microsoft.AspNetCore.Mvc;
using Studdit.Application.Common.Extensions;
using Studdit.Application.Common.Models;
using Studdit.Application.Votes.Commands.CreateVote;
using Studdit.Application.Votes.Commands.DeleteVote;
using Studdit.Application.Votes.Commands.UpdateVote;
using Studdit.Application.Votes.Models;
using Studdit.Application.Votes.Queries.GetVote;
using Studdit.Application.Votes.Queries.GetVotes;

namespace Studdit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VotesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all votes with optional filtering and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of votes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<VoteSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<VoteSummaryDto>>> GetVotes([FromQuery] GetVotesQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get a specific vote by ID
        /// </summary>
        /// <param name="id">Vote ID</param>
        /// <returns>Vote details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(VoteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VoteDto>> GetVote(int id)
        {
            var query = new GetVoteQuery { Id = id };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Create a new vote (upvote or downvote)
        /// </summary>
        /// <param name="command">Vote details</param>
        /// <returns>Created vote ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> CreateVote([FromBody] CreateVoteCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetVote),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Update an existing vote (change vote type)
        /// </summary>
        /// <param name="id">Vote ID</param>
        /// <param name="command">Updated vote details</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateVote(int id, [FromBody] UpdateVoteCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in URL does not match ID in request body.");
            }

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Delete a vote
        /// </summary>
        /// <param name="id">Vote ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVote(int id)
        {
            var command = new DeleteVoteCommand { Id = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Upvote a question
        /// </summary>
        /// <param name="questionId">Question ID</param>
        /// <returns>Created vote ID</returns>
        [HttpPost("questions/{questionId:int}/upvote")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> UpvoteQuestion(int questionId)
        {
            var command = new CreateVoteCommand
            {
                VoteType = "Upvote",
                QuestionId = questionId
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetVote),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Downvote a question
        /// </summary>
        /// <param name="questionId">Question ID</param>
        /// <returns>Created vote ID</returns>
        [HttpPost("questions/{questionId:int}/downvote")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> DownvoteQuestion(int questionId)
        {
            var command = new CreateVoteCommand
            {
                VoteType = "Downvote",
                QuestionId = questionId
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetVote),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Upvote an answer
        /// </summary>
        /// <param name="answerId">Answer ID</param>
        /// <returns>Created vote ID</returns>
        [HttpPost("answers/{answerId:int}/upvote")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> UpvoteAnswer(int answerId)
        {
            var command = new CreateVoteCommand
            {
                VoteType = "Upvote",
                AnswerId = answerId
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetVote),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Downvote an answer
        /// </summary>
        /// <param name="answerId">Answer ID</param>
        /// <returns>Created vote ID</returns>
        [HttpPost("answers/{answerId:int}/downvote")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> DownvoteAnswer(int answerId)
        {
            var command = new CreateVoteCommand
            {
                VoteType = "Downvote",
                AnswerId = answerId
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetVote),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Get votes for a specific question
        /// </summary>
        /// <param name="questionId">Question ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of votes for the question</returns>
        [HttpGet("by-question/{questionId:int}")]
        [ProducesResponseType(typeof(PaginatedList<VoteSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<VoteSummaryDto>>> GetVotesByQuestion(
            int questionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetVotesQuery
            {
                QuestionId = questionId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get votes for a specific answer
        /// </summary>
        /// <param name="answerId">Answer ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of votes for the answer</returns>
        [HttpGet("by-answer/{answerId:int}")]
        [ProducesResponseType(typeof(PaginatedList<VoteSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<VoteSummaryDto>>> GetVotesByAnswer(
            int answerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetVotesQuery
            {
                AnswerId = answerId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get votes by a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of user's votes</returns>
        [HttpGet("by-user/{userId:int}")]
        [ProducesResponseType(typeof(PaginatedList<VoteSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<VoteSummaryDto>>> GetVotesByUser(
            int userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetVotesQuery
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }
    }
}