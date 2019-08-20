using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace bankAccounts.Models
{
    public class Transaction
    {
        [Required]
        public int TransactionId {get; set;}

        [Required]
        public decimal Amount {get; set;}

        [Required]
        public int UserId {get; set;}

        public User User {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;

        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}