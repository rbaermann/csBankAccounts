using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bankAccounts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private BankAccountsContext dbContext;

        public HomeController(BankAccountsContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("InSession", newUser.UserId);
                return RedirectToAction("Account", new{id = newUser.UserId});
            }

            return RedirectToAction("Index");
        }

        [HttpGet("login")]
        public ViewResult Login()
        {
            return View("Login");
        }

        [HttpPost("logging")]
        public IActionResult CheckingUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Email does not excist");
                    return View("Login");
                }
                
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Wrong password");
                    return View("Login");
                }

                HttpContext.Session.SetInt32("InSession", userInDb.UserId);
                return RedirectToAction("Account", new{id = userInDb.UserId});
            }

            return RedirectToAction("Login");
        }

        [HttpGet("account/{id}")]
        public IActionResult Account(int id)
        {
            if(HttpContext.Session.GetInt32("InSession") != id)
            {
                return RedirectToAction("Index");
            }

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == id);
            List<Transaction> transactions = dbContext.Transactions.Include(t => t.User).OrderByDescending(t => t.CreatedAt).ToList();
            BankAccountView viewModel = new BankAccountView();
            viewModel.AllTransactions = transactions;
            viewModel.CurrentUser = user;
            return View("Account", viewModel);
        }

        [HttpPost("balance/{UserId}")]
        public IActionResult ChangingBalance(BankAccountView newTransaction, int UserId)
        {
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
            List<Transaction> transactions = dbContext.Transactions.Include(t => t.User).OrderByDescending(t => t.CreatedAt).ToList();
            BankAccountView viewModel = new BankAccountView();
            viewModel.CurrentUser = user;
            viewModel.AllTransactions = transactions;

            if(ModelState.IsValid)
            {
                if(user.Balance + newTransaction.UserTransaction.Amount < 0)
                {
                    ModelState.AddModelError("UserTransaction.Amount", "Insufficient Funds");
                    return View("Account", viewModel);
                }
                user.Balance += newTransaction.UserTransaction.Amount;
                user.UpdatedAt = DateTime.Now;
                dbContext.Add(newTransaction.UserTransaction);
                dbContext.SaveChanges();
                return RedirectToAction("Account", new{id = UserId});
            }
            return View("Account", viewModel);
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
