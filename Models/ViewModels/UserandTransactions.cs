using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bankAccounts.Models
{
    public class BankAccountView
    {
        public List<Transaction> AllTransactions {get; set;}
        public User CurrentUser {get; set;}

        public Transaction UserTransaction {get; set;}
    }
}