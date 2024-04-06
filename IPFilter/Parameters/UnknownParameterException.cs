namespace IPFilter.Parameters
{
    internal class UnknownParameterException : Exception
    {
        public string Parameter { get; set; }
        public UnknownParameterException(string message, string parameter) : base(message)
        {
            Parameter = parameter;
        }
    }
}