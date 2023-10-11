using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TicketingApp.Models
{
    public class Station
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }

        [BsonElement("stationName")]
        public string StationName { get; set; }

        [BsonElement("isActive")]
        public int IsActive { get; set; }
    }
}
