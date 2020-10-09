using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WPFBusinessSense_Task.Models
{
    internal class CustomItems
    {
        [MetadataType(typeof(ItemsMetaData))]
        public partial class Items
        {
        }

        public class ItemsMetaData
        {
            [Key, Column(Order = 0)]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int ItemId { get; set; }

            [Required]
            public string ItemBarCode { get; set; }

            [Required]
            public string ItemName { get; set; }

            [Required]
            public decimal ItemSalesPrice { get; set; }
        }
    }
}