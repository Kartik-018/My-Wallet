using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }
    public class DashboardController : Controller
    {
        private ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
           _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //To show last 7 days trasaction
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction>SelectedTransaction=await _context.Transactions
                .Include(x=>x.Category)
                .Where(y=>y.Date >=StartDate  &&  y.Date <= EndDate)
                .ToListAsync();

            //total Income
            int TotalIncome = SelectedTransaction
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //total Expense
            int TotalExpense = SelectedTransaction
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            int Balance=TotalIncome-TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");

            //doughnut chart expense by category
            ViewBag.DoughnutChartData = SelectedTransaction
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon=k.First().Category.Icon+" "+k.First().Category.Title,
                    amount=k.Sum(j=>j.Amount),
                    formattedAmount=k.Sum(j=>j.Amount).ToString("C0"),
                })
                .OrderByDescending(l=>l.amount)
                .ToList();

            //Spline chart for income vs expense

            //Income collection
            List<SplineChartData> IncomeSummary = SelectedTransaction
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day=k.First().Date.ToString("dd-MMM"),
                    income=k.Sum(l=>l.Amount)

                }).ToList();


            //Expense collection
            List<SplineChartData> ExpenseSummary = SelectedTransaction
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)

                }).ToList();


            //combine expense and income collection to pass in spline chart as data

            string[] Last7Days=Enumerable.Range(0,7)
                .Select(i=>StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day=day,
                                          income=income==null?0:income.income,
                                          expense=expense==null?0:expense.expense,
                                      };


            ViewBag.RecentTransactions=await _context.Transactions
                .Include(i=>i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();
            return View();
        }
    }
}
