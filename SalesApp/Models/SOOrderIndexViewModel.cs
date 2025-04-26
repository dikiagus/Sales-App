using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace SalesApp.Models
{
    public class SOOrderIndexViewModel
    {
        public string? Keyword { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        public IPagedList<SO_ORDER>? Orders { get; set; }


        // Tambahkan ini:
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
