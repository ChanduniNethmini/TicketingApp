using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TicketingApp.Models
{
    [CollectionName("traveller")]
    public class Traveller
    {
        [BsonId]
        [BsonElement("nic")]
        [BsonRepresentation(BsonType.String)]
        [BsonRequired]
        public string NIC { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("dob")]
        public string DOB { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BsonElement("status")]
        public int Status { get; set; }
    }
}
