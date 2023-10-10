using System.ComponentModel.DataAnnotations;

namespace TicketingApp.Dtos
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string NIC { get; set; } = string.Empty;
        [Required]
        public DateTime DOB { get; set; } = DateTime.Now;
        [Required]
        public string ContactNo { get; set; } = string.Empty;
        [Required]
        public int Status { get; set; } = 0;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
