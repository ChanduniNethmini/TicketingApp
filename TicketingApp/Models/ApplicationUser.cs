using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MongoAuthenticatorAPI.Models
{
    [CollectionName("users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;

        public string NIC { get; set; } = string.Empty;

        public DateTime DOB { get; set; } = DateTime.Now;

        public string ContactNo { get; set; } = string.Empty;

        public int Status { get; set; } = 0;
    }
}