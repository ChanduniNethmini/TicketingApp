using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using MongoDbGenericRepository.Attributes;

namespace TicketingApp.Models
{
    [CollectionName("users")]
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }

        [BsonElement("nic")]
        public string NIC { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("dob")]
        public DateTime DOB { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }
    }
}
