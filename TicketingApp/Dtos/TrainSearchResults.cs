namespace TicketingApp.Dtos
{
    public class TrainSearchResult
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string TrainClass { get; set; }
        public string StartLocation { get; set; }
        public string Destination { get; set; }
        public int AvailableSeats { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string StartLocationArrivalTime { get; set; }
        public string StartLocationDepartureTime { get; set; }
    }

}
