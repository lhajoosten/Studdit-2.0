using MediatR;
using Microsoft.AspNetCore.Mvc;
using Studdit.Application.Common.Extensions;
using Studdit.Application.Common.Models;
using Studdit.Application.Questions.Commands.CreateQuestion;
using Studdit.Application.Questions.Commands.DeleteQuestion;
using Studdit.Application.Questions.Commands.UpdateQuestion;
using Studdit.Application.Questions.Models;
using Studdit.Application.Questions.Queries.GetQuestion;
using Studdit.Application.Questions.Queries.GetQuestions;

namespace Studdit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all questions with optional filtering and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of questions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<QuestionSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<QuestionSummaryDto>>> GetQuestions([FromQuery] GetQuestionsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get a specific question by ID
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <returns>Question details with answers</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            var query = new GetQuestionQuery { Id = id };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Create a new question
        /// </summary>
        /// <param name="command">Question details</param>
        /// <returns>Created question ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> CreateQuestion([FromBody] CreateQuestionCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetQuestion),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Update an existing question
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <param name="command">Updated question details</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in URL does not match ID in request body.");
            }

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Delete a question
        /// </summary>
        /// <param name="id">Question ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            var command = new DeleteQuestionCommand { Id = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get questions by specific author
        /// </summary>
        /// <param name="authorId">Author user ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of user's questions</returns>
        [HttpGet("by-author/{authorId:int}")]
        [ProducesResponseType(typeof(PaginatedList<QuestionSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<QuestionSummaryDto>>> GetQuestionsByAuthor(
            int authorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetQuestionsQuery
            {
                AuthorId = authorId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedAt",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get questions by tags
        /// </summary>
        /// <param name="tags">Comma-separated list of tag names</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of questions with specified tags</returns>
        [HttpGet("by-tags")]
        [ProducesResponseType(typeof(PaginatedList<QuestionSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<QuestionSummaryDto>>> GetQuestionsByTags(
            [FromQuery] string tags,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(tags))
            {
                return BadRequest("Tags parameter is required.");
            }

            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(t => t.Trim())
                             .ToList();

            var query = new GetQuestionsQuery
            {
                TagNames = tagList,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedAt",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Search questions
        /// </summary>
        /// <param name="searchTerm">Search term to look for in title and content</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of matching questions</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PaginatedList<QuestionSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<QuestionSummaryDto>>> SearchQuestions(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term is required.");
            }

            var query = new GetQuestionsQuery
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedAt",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get unanswered questions
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of unanswered questions</returns>
        [HttpGet("unanswered")]
        [ProducesResponseType(typeof(PaginatedList<QuestionSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<QuestionSummaryDto>>> GetUnansweredQuestions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetQuestionsQuery
            {
                IsAnswered = false,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "CreatedAt",
                SortDescending = true
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }
    }
}