using Studdit.Domain.Entities;
using Studdit.Domain.Enums;

namespace Studdit.Test.Utils.Builders
{
    public class VoteBuilder
    {
        private VoteType _type = VoteType.Upvote;
        private User _user = new UserBuilder().Build();
        private Question? _question = null;
        private Answer? _answer = null;

        public VoteBuilder WithType(VoteType type)
        {
            _type = type;
            return this;
        }

        public VoteBuilder WithUser(User user)
        {
            _user = user;
            return this;
        }

        public VoteBuilder ForQuestion(Question question)
        {
            _question = question;
            _answer = null;
            return this;
        }

        public VoteBuilder ForAnswer(Answer answer)
        {
            _answer = answer;
            _question = null;
            return this;
        }

        public Vote Build()
        {
            if (_question != null)
            {
                return Vote.CreateForQuestion(_type, _user, _question);
            }
            else if (_answer != null)
            {
                return Vote.CreateForAnswer(_type, _user, _answer);
            }
            else
            {
                // Default to question vote with a new question
                var defaultQuestion = new QuestionBuilder().Build();
                return Vote.CreateForQuestion(_type, _user, defaultQuestion);
            }
        }
    }
}
