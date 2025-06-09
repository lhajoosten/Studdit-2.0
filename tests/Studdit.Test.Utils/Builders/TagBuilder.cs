using Studdit.Domain.Entities;

namespace Studdit.Test.Utils.Builders
{
    public class TagBuilder
    {
        private string _name = "test-tag";
        private string _description = "This is a test tag description.";
        private int _usageCount = 0;

        public TagBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public TagBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public TagBuilder WithUsageCount(int usageCount)
        {
            _usageCount = usageCount;
            return this;
        }

        public Tag Build()
        {
            var tag = Tag.Create(_name, _description);

            for (int i = 0; i < _usageCount; i++)
            {
                tag.IncrementUsage();
            }

            return tag;
        }
    }
}
