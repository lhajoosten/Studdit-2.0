using MediatR;
using Microsoft.AspNetCore.Mvc;
using Studdit.Application.Common.Extensions;
using Studdit.Application.Common.Models;
using Studdit.Application.Users.Commands.CreateUser;
using Studdit.Application.Users.Commands.DeleteUser;
using Studdit.Application.Users.Commands.UpdateUser;
using Studdit.Application.Users.Models;
using Studdit.Application.Users.Queries.GetUser;
using Studdit.Application.Users.Queries.GetUserProfile;
using Studdit.Application.Users.Queries.GetUsers;

namespace Studdit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all users with optional filtering and pagination
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<UserSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<UserSummaryDto>>> GetUsers([FromQuery] GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var query = new GetUserQuery { Id = id };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get user profile with additional statistics
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User profile with statistics</returns>
        [HttpGet("{id:int}/profile")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(int id)
        {
            var query = new GetUserProfileQuery { Id = id };
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="command">User creation details</param>
        /// <returns>Created user ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<int>> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = result.Value },
                    result.Value);
            }

            return result.ToActionResult();
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="command">Updated user details</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in URL does not match ID in request body.");
            }

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Delete user account (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Get user statistics
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User statistics</returns>
        [HttpGet("{id:int}/stats")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserStats(int id)
        {
            // This could be implemented as a separate query
            var userResult = await _mediator.Send(new GetUserProfileQuery { Id = id });

            if (!userResult.IsSuccess)
            {
                return Result.Failure(userResult.Error).ToActionResult();
            }

            var user = userResult.Value;
            var stats = new
            {
                user.Id,
                user.Username,
                user.Reputation,
                user.QuestionCount,
                user.AnswerCount,
                JoinDate = user.CreatedDate,
                LastSeen = user.LastLoginDate
            };

            return Ok(stats);
        }
    }
}