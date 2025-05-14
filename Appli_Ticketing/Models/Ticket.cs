namespace Appli_Ticketing.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime DateCreation { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Response { get; set; }
        public DateTime? DateResponse { get; set; }
        public bool IsSelected { get; set; }

        public string ProblemName { get; set; }
        public int ProblemCriticite { get; set; }
    }

}
