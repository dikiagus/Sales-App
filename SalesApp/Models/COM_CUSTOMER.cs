using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesApp.Models
{
    public class COM_CUSTOMER
    {
        [Key]
        [Column("com_customer_id")]
        public int COM_CUSTOMER_ID { get; set; }

        [Column("customer_name")]
        public string CustomerName { get; set; }

        public ICollection<SO_ORDER> SO_ORDERS { get; set; }
    }
}
