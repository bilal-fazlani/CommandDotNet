namespace CommandDotNet.Parsing
{
    public class OptionTokenType
    {
        private readonly string _value;
        private readonly int _assignmentIndex;

        public bool IsLong { get; }
        public bool IsShort => !IsLong;
        public bool IsClubbed { get; }
        public bool HasValue { get; }

        public OptionTokenType(
            string value,
            bool isLong = false, 
            bool isClubbed = false, 
            bool hasValue = false, 
            int assignmentIndex = -1)
        {
            _value = value;
            IsLong = isLong;
            IsClubbed = isClubbed;
            HasValue = hasValue;
            _assignmentIndex = assignmentIndex;
        }

        public string GetPrefix() => IsLong ? "--" : "-";

        public string GetName() => HasValue
            ? _value.Substring(0, _assignmentIndex)
            : _value;

        public string? GetAssignedValue() => HasValue
            ? _value.Substring(_assignmentIndex + 1)
            : null;


    }
}