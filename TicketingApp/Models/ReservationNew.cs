using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TicketingApp.Models
{
    public class ReservationNew
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }

        [BsonElement("travelerID")]
        public int TravelerID { get; set; }

        [BsonElement("reservationDate")]
        public string ReservationDate { get; set; }

        [BsonElement("bookingDate")]
        public string BookingDate { get; set; }

        [BsonElement("trainID")]
        public string TrainID { get; set; }

        [BsonElement("startLocation")]
        public string StartLocation { get; set; }

        [BsonElement("destination")]
        public string Destination { get; set; }

        [BsonElement("trainclass")]
        public string TrainClass { get; set; }

        [BsonElement("seatCount")]
        public int SeatCount { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }
    }
}
