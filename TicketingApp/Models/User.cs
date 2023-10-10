using MongoDbGenericRepository.Attributes;

namespace TicketingApp.Models
{
    [CollectionName("users")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NIC { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool Status { get; set; }

    }
}
