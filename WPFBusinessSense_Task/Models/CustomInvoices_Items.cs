using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WPFBusinessSense_Task.Models
{
    [MetadataType(typeof(Invoices_ItemsMetaData))]
    public partial class Invoices_Items
    {
    }

    public class Invoices_ItemsMetaData
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Invoices")]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Invoice_Id { get; set; }

        public virtual Invoices Invoices { get; set; }

        [ForeignKey("Items")]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Item_Id { get; set; }

        public virtual Items Items { get; set; }

        [Required]
        public decimal InvItem_SalesPrice { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [NotMapped]
        public decimal Item_TotalPrice
        {
            get
            {
                return Math.Round(InvItem_SalesPrice * Quantity, 2);
            }
        }

        //  public bool IsNewInvoiceItem;
    }
}