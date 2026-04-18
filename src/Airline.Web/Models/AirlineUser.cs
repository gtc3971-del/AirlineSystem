using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Airline.Web.Models
{
    public enum UserRole
    {
        Passenger = 1,
        Agent = 2,
        Dispatcher = 3,
        Admin = 4
    }

    public class AirlineUser : IdentityUser
    {
        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [MaxLength(100, ErrorMessage = "ФИО не может превышать 100 символов")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Номер паспорта не может превышать 20 символов")]
        [Display(Name = "Номер паспорта")]
        public string? PassportNumber { get; set; }

        [Phone(ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Телефон")]
        public override string? PhoneNumber { get; set; }

        [Display(Name = "Роль")]
        public UserRole UserRole { get; set; } = UserRole.Passenger;

        [Display(Name = "Дата регистрации")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Последний вход")]
        public DateTime? LastLoginAt { get; set; }
    }
}