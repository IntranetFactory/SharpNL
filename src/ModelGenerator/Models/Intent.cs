namespace ModelGenerator.Models
{
    class Intent
    {
        public string Id { get; set; }
        public string IntentName { get; set; }
        public string Text { get; set; }

        public string FileName { get; set; }
        public int Line { get; set; }
    }
}
