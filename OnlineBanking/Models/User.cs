using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.Models
{

    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        //[DisplayName("Username")]
        //[Required(ErrorMessage = "{0} Username Cannot Be Blank")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        //[Required(ErrorMessage = "{0} Password Cannot Be Blank")]
        public string Password { get; set; }

        [DisplayName("Full Name")]
       // [Required(ErrorMessage = "{0} Full Name Cannot Be Blank")]
        public string FullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone Number")]
        //[Required(ErrorMessage = "{0} Full Name Cannot Be Blank")]
        public string NumberPhone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} Address Cannot Be Blank")]
        public string Address { get; set; }

        public DateTime DateCreate { get; set; }

        [DisplayName("Identity Card")]
       // [Required(ErrorMessage = "{0} Identity Card Cannot Be Blank")]
        public string IdentityCard { get; set; }

        public bool IsAdmin { get; set; }
        public bool Status { get; set; }

        //Thêm các List cho dễ truy vấn
        public List<AddressBook> AddressBooks { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
