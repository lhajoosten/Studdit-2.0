using Studdit.Domain.Entities;

namespace Studdit.Test.Utils.Builders
{
    public class QuestionBuilder
    {
        private string _title = "Test Question Title";
        private string _content = "This is a test question content that meets the minimum length requirement.";
        private User _author = new UserBuilder().Build();
        private bool _isClosed = false;
        private int _viewCount = 0;

        public QuestionBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public QuestionBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public QuestionBuilder WithAuthor(User author)
        {
            _author = author;
            return this;
        }

        public QuestionBuilder WithClosed(bool isClosed = true)
        {
            _isClosed = isClosed;
            return this;
        }

        public QuestionBuilder WithViewCount(int viewCount)
        {
            _viewCount = viewCount;
            return this;
        }

        public Question Build()
        {
            var question = Question.Create(_title, _content, _author);

            if (_isClosed)
            {
                question.Close("Test closure reason");
            }

            for (int i = 0; i < _viewCount; i++)
            {
                question.IncrementViewCount();
            }

            return question;
        }
    }
}
