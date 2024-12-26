namespace E_commerce_Back_end.OPT
{
    public class EmailDto
    {
        private string _subject = "Your single-use code";
        public string Body { get; set; } = string.Empty;
        public string Subject
        {
            get => _subject;
            set => _subject = value ?? throw new ArgumentNullException(nameof(Subject), "Email subject is required.");
        }

        public string SetBody(string newBody)
        {
            Body = newBody;
            return Body;
        }

    }
}
