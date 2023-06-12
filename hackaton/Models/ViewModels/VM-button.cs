namespace hackaton.Models.ViewModels
{
    public class VM_button
    {
        //O ideal seria ter text OU value; infelizmente, não vai rolar implementar o requerimento hoje.
        public string? Text { get; set; }
        public string? Value { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public string Class { get; set; }
        public string? IconURL { get; set; }
        public string? IconClass { get; set; }
    }
}
