using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class LoginViewModel
    {
        //[Display(Name ="Kullanıcı Adı",Prompt ="John Doe")]
        //name kısmı label yerine kullanılabiliyor, promt ise place holder yerine geçiyor
        [Required(ErrorMessage ="Bu alan Boş Bırakılamaz")]
        [StringLength(30)]
        public string Username { get; set; }

        //[DataType(DataType.Password)]
        [Required(ErrorMessage="Bu alan Boş Bırakılamaz")]
        [MinLength(6,ErrorMessage ="Minimum 6 karakter giriniz")]
        [StringLength(30)]
        public string Password { get; set; }

    }
}
