using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesApp.Models.ViewModels
{
    public class SalesOrderViewModel
    {
        public string SalesOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string Address { get; set; }

        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();

        public List<SalesOrderItemViewModel> Items { get; set; } = new List<SalesOrderItemViewModel>();
    }


    public class SalesOrderItemViewModel
    {
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }

    public class SalesOrderSearchViewModel
    {
        public string? SalesOrderNumber { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<SO_ORDER> Results { get; set; } = new List<SO_ORDER>();
    }

}
