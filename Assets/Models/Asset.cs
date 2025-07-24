using System;
using System.ComponentModel.DataAnnotations;
namespace Assets.Models
{
    public class Asset
    {
        public int AssetId { get; set; }
        public string AssetNumber { get; set; }
        public string AssetName { get; set; }
        [Required(ErrorMessage = "تاريخ الشراء مطلوب")]
        [DataType(DataType.Date)]
        public DateTime AssetDate { get; set; }
        [Required(ErrorMessage = "قيمة الشراء مطلوبة")]
        [Range(0.01, double.MaxValue, ErrorMessage = "قيمة الشراء يجب أن تكون أكبر من صفر")]
        public decimal PurchaseValue { get; set; }
        [Required(ErrorMessage = "حساب الأصل مطلوب")]
        public int AssetAccountId { get; set; }
        [Required(ErrorMessage = "حساب الشراء مطلوب")]
        public int PurchaseAccountId { get; set; }
    }
}
