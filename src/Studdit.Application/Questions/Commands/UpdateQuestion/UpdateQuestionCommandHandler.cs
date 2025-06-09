using FluentValidation;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Commands.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : ICommandHandler<UpdateQuestionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<Question> _questionRepository;
        private readonly IQueryRepository<Tag> _tagRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateQuestionCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<Question> questionRepository,
            IQueryRepository<Tag> tagRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _questionRepository = questionRepository;
            _tagRepository = tagRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User must be authenticated.");
            }

            // Get the question with its tags
            var question = await _questionRepository.GetByIdAsync(request.Id, cancellationToken, q => q.Tags, q => q.Author);
            if (question == null)
            {
                return Result.Failure("Question not found.");
            }

            // Check authorization
            if (question.Author.Id != _currentUserService.UserId.Value)
            {
                return Result.Failure("You can only edit your own questions.");
            }

            if (question.IsClosed)
            {
                return Result.Failure("Cannot edit a closed question.");
            }

            try
            {
                // Update question content
                question.Update(request.Title, request.Content);

                // Handle tag updates
                var normalizedTagNames = request.TagNames.Select(t => t.ToLowerInvariant()).ToList();
                var existingTags = await _tagRepository.FindAsync(t => normalizedTagNames.Contains(t.Name), cancellationToken);

                var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();
                var missingTagNames = normalizedTagNames.Where(name => !existingTagNames.Contains(name)).ToList();

                // Check if user can create new tags (if any are missing)
                if (missingTagNames.Any() && !question.Author.CanCreateTag())
                {
                    return Result.Failure($"You need {1500 - question.Author.Reputation} more reputation points to create new tags. Please use existing tags.");
                }

                // Remove old tags
                var currentTags = question.Tags.ToList();
                foreach (var tag in currentTags)
                {
                    question.RemoveTag(tag);
                }

                // Create missing tags
                var tagRepository = _unitOfWork.CommandRepository<Tag>();
                var newTags = new List<Tag>();

                foreach (var tagName in missingTagNames)
                {
                    var newTag = Tag.Create(tagName, $"Description for {tagName}");
                    var createdTag = await tagRepository.AddAsync(newTag, _currentUserService.UserId.Value, cancellationToken);
                    newTags.Add(createdTag);
                }

                // Add all tags to the question
                var allTags = existingTags.Concat(newTags).ToList();
                foreach (var tag in allTags)
                {
                    question.AddTag(tag);
                }

                // Save changes
                var questionRepository = _unitOfWork.CommandRepository<Question>();
                await questionRepository.UpdateAsync(question, _currentUserService.UserId.Value, cancellationToken);
                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update question: {ex.Message}");
            }
        }
    }
}
