using MediatR;
using Studdit.Application.Common.Models;

namespace Studdit.Application.Common.Abstractions
{
    /// <summary>
    /// Marker interface for queries
    /// </summary>
    /// <typeparam name="TResponse">The type of the response</typeparam>
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
