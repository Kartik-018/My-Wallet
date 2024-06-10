using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }


        [Range(1,int.MaxValue,ErrorMessage ="Please select the category.")]
        public int CategoryId { get; set; }//It will create foreign key as cascase mode so when we delete from here it will also be deleleted from category table
        public Category? Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount should be More than zero")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime Date { get; set; }= DateTime.Now;

        [NotMapped]
        public string? CategoryTitleIcon 
        {
            get { return Category == null ? "" : Category.Icon + " " + Category.Title; }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Expense") ? "- " : "+ ") + Amount.ToString("C0");
            }
        }
    }
}
