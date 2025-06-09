using FluentValidation;
using Studdit.Application.Common.Abstractions;
using Studdit.Application.Common.Interfaces;
using Studdit.Application.Common.Models;
using Studdit.Domain.Entities;

namespace Studdit.Application.Questions.Commands.CreateQuestion
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueryRepository<User> _userRepository;
        private readonly IQueryRepository<Tag> _tagRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateQuestionCommandHandler(
            IUnitOfWork unitOfWork,
            IQueryRepository<User> userRepository,
            IQueryRepository<Tag> tagRepository,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<int>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure<int>("User must be authenticated.");
            }

            // Get the current user
            var author = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
            if (author == null || !author.IsActive)
            {
                return Result.Failure<int>("User not found or inactive.");
            }

            try
            {
                // Create the question
                var question = Question.Create(request.Title, request.Content, author);

                // Get or create tags
                var normalizedTagNames = request.TagNames.Select(t => t.ToLowerInvariant()).ToList();
                var existingTags = await _tagRepository.FindAsync(t => normalizedTagNames.Contains(t.Name), cancellationToken);

                var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();
                var missingTagNames = normalizedTagNames.Where(name => !existingTagNames.Contains(name)).ToList();

                // Check if user can create new tags (if any are missing)
                if (missingTagNames.Any() && !author.CanCreateTag())
                {
                    return Result.Failure<int>($"You need {1500 - author.Reputation} more reputation points to create new tags. Please use existing tags.");
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

                // Save the question
                var questionRepository = _unitOfWork.CommandRepository<Question>();
                var createdQuestion = await questionRepository.AddAsync(question, _currentUserService.UserId.Value, cancellationToken);
                await _unitOfWork.SaveChangesAsync(_currentUserService.UserId.Value, cancellationToken);

                return Result.Success(createdQuestion.Id);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>($"Failed to create question: {ex.Message}");
            }
        }
    }
}
