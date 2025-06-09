using MediatR;
using Microsoft.AspNetCore.Mvc;
using Studdit.Application.Answers.Commands.AcceptAnswer;
using Studdit.Application.Answers.Commands.CreateAnswer;
using Studdit.Application.Answers.Commands.DeleteAnswer;
using Studdit.Application.Answers.Commands.UpdateAnswer;
using Studdit.Application.Answers.Models;
using Studdit.Application.Answers.Queries.GetAnswer;
using Studdit.Application.Answers.Queries.GetAnswers;
using Studdit.Application.Common.Extensions;
using Studdit.Application.Common.Models;

namespace Studdit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AnswersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnswersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all answers with optional filtering and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of answers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<AnswerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<AnswerSummaryDto>>> GetAnswers([FromQuery] GetAnswersQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get a specific answer by ID
        /// </summary>
        /// <param name="id">Answer ID</param>
        /// <returns>Answer details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AnswerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AnswerDto>> GetAnswer(int id)
        {
            var query = new GetAnswerQuery { Id = id };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Create a new answer
        /// </summary>
        /// <param name="command">Answer details</param>
        /// <returns>Created answer ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> CreateAnswer([FromBody] CreateAnswerCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetAnswer),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Update an existing answer
        /// </summary>
        /// <param name="id">Answer ID</param>
        /// <param name="command">Updated answer details</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAnswer(int id, [FromBody] UpdateAnswerCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in URL does not match ID in request body.");
            }

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Delete an answer
        /// </summary>
        /// <param name="id">Answer ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAnswer(int id)
        {
            var command = new DeleteAnswerCommand { Id = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Accept an answer (mark as accepted)
        /// </summary>
        /// <param name="id">Answer ID</param>
        /// <returns>No content if successful</returns>
        [HttpPost("{id:int}/accept")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AcceptAnswer(int id)
        {
            var command = new AcceptAnswerCommand { AnswerId = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get answers for a specific question
        /// </summary>
        /// <param name="questionId">Question ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of answers for the question</returns>
        [HttpGet("by-question/{questionId:int}")]
        [ProducesResponseType(typeof(PaginatedList<AnswerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<AnswerSummaryDto>>> GetAnswersByQuestion(
            int questionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetAnswersQuery
            {
                QuestionId = questionId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDescending = false // Show oldest answers first
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get answers by specific author
        /// </summary>
        /// <param name="authorId">Author user ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of user's answers</returns>
        [HttpGet("by-author/{authorId:int}")]
        [ProducesResponseType(typeof(PaginatedList<AnswerSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<AnswerSummaryDto>>> GetAnswersByAuthor(
            int authorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetAnswersQuery
            {
                AuthorId = authorId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedDate",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get accepted answers
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of accepted answers</returns>
        [HttpGet("accepted")]
        [ProducesResponseType(typeof(PaginatedList<AnswerSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<AnswerSummaryDto>>> GetAcceptedAnswers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetAnswersQuery
            {
                IsAccepted = true,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "VoteScore",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }
    }
}