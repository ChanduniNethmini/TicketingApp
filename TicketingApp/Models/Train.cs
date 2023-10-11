using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TicketingApp.Models
{
    public class TrainSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("startTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("startLocation")]
        public string StartLocation { get; set; }

        [BsonElement("destination")]
        public string Destination { get; set; }

        [BsonElement("trainclass")]
        public string TrainClass { get; set; }

        [BsonElement("seatCount")]
        public int SeatCount { get; set; }

        [BsonElement("remainingSeats")]
        public int RemainingSeats { get; set; }

        [BsonElement("isActive")]
        public int IsActive { get; set; }

        [BsonElement("stoppingStations")]
        public List<TrainStoppingStation> StoppingStations { get; set; }
    }

    public class TrainStoppingStation
    {
        [BsonElement("stationName")]
        public string StationName { get; set; }

        [BsonElement("stationCount")]
        public int StationCount { get; set; }

        [BsonElement("arrivalTime")]
        public DateTime ArrivalTime { get; set; }

        [BsonElement("departureTime")]
        public DateTime DepartureTime { get; set; }
    }
}
