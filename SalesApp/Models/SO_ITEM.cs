using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesApp.Models
{
    public class SO_ITEM
    {
        [Key]
        [Column("so_item_id")]
        public long SO_ITEM_ID { get; set; }

        [Column("so_order_id")]
        public long SO_ORDER_ID { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [ForeignKey("SO_ORDER_ID")]
        public SO_ORDER Order { get; set; }
    }
}
