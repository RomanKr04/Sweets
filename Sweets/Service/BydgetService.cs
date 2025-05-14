namespace Sweets.Service
{
    public class BydgetService
    {
        private readonly SweetContext sweetContext;
        public decimal GetBydget()
        {
            var bydget = sweetContext.Budgets.FirstOrDefault();
            return bydget?.TotalAmount ?? 0;
        }
    }
}
