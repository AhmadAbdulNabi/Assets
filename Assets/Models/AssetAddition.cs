using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Assets.Models
{
    public class AssetAddition
    {
        public int AdditionId { get; set; }
        [Required(ErrorMessage = "يجب اختيار الأصل")]
        public int AssetId { get; set; }
        [NotMapped]
        public string? AssetName { get; set; }
        [Required(ErrorMessage = "يجب إدخال تاريخ الإضافة")]
        public DateTime AdditionDate { get; set; }
        [Required(ErrorMessage = "يجب إدخال قيمة الإضافة")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن تكون القيمة أكبر من صفر")]
        public decimal AdditionValue { get; set; }
        public string Description { get; set; }
    }
}
