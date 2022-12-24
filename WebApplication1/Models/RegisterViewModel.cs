using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class  RegisterViewModel
    {
        //[Display(Name ="Kullanıcı Adı",Prompt ="John Doe")]
        //name kısmı label yerine kullanılabiliyor, promt ise place holder yerine geçiyor
        [Required(ErrorMessage = "Bu alan Boş Bırakılamaz")]
        [StringLength(30, ErrorMessage = "30 Karakter olmalı")]
        public string Username { get; set; }

        //[DataType(DataType.Password)]
        [Required(ErrorMessage = "Bu alan Boş Bırakılamaz")]
        [MinLength(6, ErrorMessage = "minimum 6 Karakter olmalı")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Bu alan Boş Bırakılamaz")]
        [MinLength(6, ErrorMessage = "minimum 6 Karakter olmalı")]
        [Compare(nameof(Password))]
        public string RePassword { get; set; }
        
    }
}
