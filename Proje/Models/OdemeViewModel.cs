using System.ComponentModel.DataAnnotations;

namespace YemekSepeti.WebUI.Models
{
    public class OdemeViewModel
    {
        public SepetViewModel Sepet { get; set; } = new SepetViewModel();

        [Required(ErrorMessage = "Teslimat adresi zorunludur.")]
        [Display(Name = "Teslimat Adresi")]
        public string TeslimatAdresi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart üzerindeki isim zorunludur.")]
        [Display(Name = "Kart Sahibi")]
        public string KartSahibi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart numarası zorunludur.")]
        [StringLength(19, MinimumLength = 16, ErrorMessage = "Geçersiz kart numarası.")]
        [Display(Name = "Kart Numarası")]
        public string KartNumarasi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        public string SonKullanmaTarihi { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVC kodu zorunludur.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVC 3 haneli olmalıdır.")]
        [Display(Name = "CVC")]
        public string CVC { get; set; } = string.Empty;
    }
}
