namespace GelirGider.Models
{
    public class IncomeModel
    {
        public int Id { get; set; }
        public string Income { get; set; }
        public int IncomePrice { get; set; }
        public int UserId { get; set; }
    }
    public class ExpenseModel
    {
        public int Id { get; set; }
        public string Expense { get; set; }
        public int Price { get; set; }  
        public int UserId { get; set; }
    }
    public class ChartModel
    {
        public List<IncomeModel> Income { get; set; }
        public List<ExpenseModel> Expense { get; set; }
    }
}
