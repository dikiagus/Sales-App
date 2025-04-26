using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesApp.Models
{
    public class SO_ORDER
    {
        [Key]
        [Column("so_order_id")]
        public long SO_ORDER_ID { get; set; }

        [Column("order_no")] // Perhatikan: ini sesuai nama kolom di database (bukan 'order_no')
        public string OrderNo { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("com_customer_id")]
        public int COM_CUSTOMER_ID { get; set; }

        [Column("address")]
        public string Address { get; set; }

        public COM_CUSTOMER Customer { get; set; }
        public ICollection<SO_ITEM> SO_ITEMS { get; set; }
    }
}
