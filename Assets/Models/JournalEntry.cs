using System.ComponentModel.DataAnnotations.Schema;
namespace Assets.Models
{
    public class JournalEntry
    {
        public int EntryId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Description { get; set; }
        public string AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        [NotMapped]
        public string AccountName { get; set; }
    }
}
