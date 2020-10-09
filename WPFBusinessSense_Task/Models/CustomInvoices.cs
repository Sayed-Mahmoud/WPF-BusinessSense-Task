using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WPFBusinessSense_Task.Models
{
    [MetadataType(typeof(InvoicesMetaData))]
    public partial class Invoices
    {
    }

    public class InvoicesMetaData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InvoiceId { get; set; }

        [Required]
        public System.DateTime InvoiceDate { get; set; }

        [NotMapped()]
        public decimal TotalInvoicePrice
        {
            get
            {
                return Math.Round(this.Invoices_Items.Sum(x => x.InvItem_SalesPrice * x.Quantity), 2);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Invoices_Items> Invoices_Items { get; set; }

        // public bool IsNewInvoice;
    }
}