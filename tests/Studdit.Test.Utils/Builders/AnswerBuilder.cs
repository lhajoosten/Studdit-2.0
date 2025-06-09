using Studdit.Domain.Entities;

namespace Studdit.Test.Utils.Builders
{
    public class AnswerBuilder
    {
        private string _content = "This is a test answer content.";
        private User _author = new UserBuilder().Build();
        private Question _question = new QuestionBuilder().Build();
        private bool _isAccepted = false;

        public AnswerBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public AnswerBuilder WithAuthor(User author)
        {
            _author = author;
            return this;
        }

        public AnswerBuilder WithQuestion(Question question)
        {
            _question = question;
            return this;
        }

        public AnswerBuilder WithAccepted(bool isAccepted = true)
        {
            _isAccepted = isAccepted;
            return this;
        }

        public Answer Build()
        {
            var answer = Answer.Create(_content, _author, _question);

            if (_isAccepted)
            {
                // This would require the question author to mark it as accepted
                // For testing purposes, we'll use reflection or create a method
                answer.MarkAsAccepted();
            }

            return answer;
        }
    }
}
