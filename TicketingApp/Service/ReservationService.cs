using MongoDB.Driver;
using TicketingApp.Models;

namespace TicketingApp.Service
{
    public class ReservationService
    {
        //create the database connection
        private readonly IMongoCollection<Reservation> _reservationCollection;
        private readonly IMongoCollection<TrainSchedule> _trainScheduleCollection;


        public ReservationService(IMongoDatabase database)
        {
            _reservationCollection = database.GetCollection<Reservation>("reservations");
            _trainScheduleCollection = database.GetCollection<TrainSchedule>("trainSchedule");
        }

        //Create a new reservation
        public bool CreateReservation(Reservation reservation)
        {
            Reservation newReservation = new Reservation
            {
                NIC = reservation.NIC,
                ReservationDate = reservation.ReservationDate,
                BookingDate = DateTime.Now.Date.ToString(),
                TrainID = reservation.TrainID,
                StartLocation = reservation.StartLocation,
                Destination = reservation.Destination,
                TrainClass = reservation.TrainClass,
                DepartureTime = reservation.DepartureTime,
                Status = 1,
                SeatCount = reservation.SeatCount,
                Price = 0,
            };

            DateTime reservationDate = DateTime.Parse(newReservation.ReservationDate);
            DateTime bookingDate = DateTime.Parse(newReservation.BookingDate);

            TimeSpan dateDifference = reservationDate - bookingDate;
            int daysDifference = dateDifference.Days;

            if (daysDifference <= 30)
            {
                // Find the corresponding TrainService document
                var trainServiceFilter = Builders<TrainSchedule>.Filter.Eq(ts => ts.ID, newReservation.TrainID)
                    & Builders<TrainSchedule>.Filter.Eq(ts => ts.Date, newReservation.ReservationDate);

                var trainService = _trainScheduleCollection.Find(trainServiceFilter).FirstOrDefault();

                if (trainService != null)
                {
                    var Tdestination = trainService.StoppingStations
                    .Find(ss => ss.StationName == newReservation.Destination);

                    var TstartLocation = trainService.StoppingStations
                        .Find(ss => ss.StationName == newReservation.StartLocation);

                    decimal price = 0;
                    if (newReservation.TrainClass == "A")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 100;
                    }
                    else if (newReservation.TrainClass == "B")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 75;
                    }
                    else if (newReservation.TrainClass == "C")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 50;
                    }

                    // Calculate new RemainingSeats
                    int newRemainingSeats = trainService.RemainingSeats - newReservation.SeatCount;

                    if (newRemainingSeats >= 0)
                    {
                        var existingReservations = _reservationCollection
                        .Find(r => r.NIC == newReservation.NIC && r.Status == 1)
                        .ToList();

                        if (existingReservations.Count >= 4)
                        {
                            return false;
                        }

                        // Update RemainingSeats in TrainService
                        var update = Builders<TrainSchedule>.Update.Set(ts => ts.RemainingSeats, newRemainingSeats);
                        _trainScheduleCollection.UpdateOne(trainServiceFilter, update);
                        newReservation.Price = price * newReservation.SeatCount;
                        _reservationCollection.InsertOne(newReservation);
                        return true;
                    }
                }


            }

            return false;
        }



        //Update an existing reservation
        public bool UpdateReservation(string id, Reservation updatedReservation)
        {
            var existingReservation = _reservationCollection
                .Find(r => r.ID == id && r.Status == 1)
                .FirstOrDefault();

            if (existingReservation != null)
            {
                DateTime reservationDate = DateTime.Parse(updatedReservation.ReservationDate);

                TimeSpan dateDifference = reservationDate - DateTime.Now.Date;
                int daysDifference = dateDifference.Days;

                if (daysDifference >= 5)
                {
                    updatedReservation.ID = id;
                    _reservationCollection.ReplaceOne(r => r.ID == id, updatedReservation);
                    return true;
                }
            }

            return false;
        }

        //Cancel a reservation
        // status = 3 means canceled 
        public bool CancelReservation(string reservationId)
        {
            var reservationToCancel = _reservationCollection
                .Find(r => r.ID == reservationId && r.Status != 3 && r.Status != 2)
                .FirstOrDefault();

            var trainServiceFilter = Builders<TrainSchedule>.Filter.Eq(ts => ts.ID, reservationToCancel.ID)
                    & Builders<TrainSchedule>.Filter.Eq(ts => ts.Date, reservationToCancel.ReservationDate);

            var trainService = _trainScheduleCollection.Find(trainServiceFilter).FirstOrDefault();

            if (reservationToCancel != null)
            {
                DateTime reservationDate = DateTime.Parse(reservationToCancel.ReservationDate);

                TimeSpan dateDifference = reservationDate - DateTime.Now.Date;
                int daysDifference = dateDifference.Days;

                if (daysDifference >= 5)
                {
                    reservationToCancel.Status = 3;
                    int newRemainingSeats = trainService.RemainingSeats + reservationToCancel.SeatCount;
                    var update = Builders<TrainSchedule>.Update.Set(ts => ts.RemainingSeats, newRemainingSeats);
                    _trainScheduleCollection.UpdateOne(trainServiceFilter, update);
                    _reservationCollection.ReplaceOne(r => r.ID == reservationId, reservationToCancel);
                    return true;
                }
            }

            return false;
        }

        // Get all reservations
        public List<Reservation> GetAllReservations()
        {
            return _reservationCollection
                .Find(r => r.Status != 3)
                .ToList();
        }


        //Get a list of reservations for a specific traveler
        // status = 1 means existing bookings
        public List<Reservation> GetExistingReservationsForTraveler(string nic)
        {

            return _reservationCollection
                .Find(r => r.NIC == nic && r.Status == 1)
                .ToList();
        }

        // status = 2 means Completed bookings
        public List<Reservation> GetReservationHistoryForTraveler(string nic)
        {

            return _reservationCollection
                .Find(r => r.NIC == nic && r.Status != 1)
                .ToList();
        }
    }
}
