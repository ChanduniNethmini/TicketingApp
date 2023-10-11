namespace TicketingApp.Dtos
{
    public class TrainSearchResult
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string TrainClass { get; set; }
        public string StartLocation { get; set; }
        public string Destination { get; set; }
        public int AvailableSeats { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime StartLocationArrivalTime { get; set; }
        public DateTime StartLocationDepartureTime { get; set; }
    }

}
