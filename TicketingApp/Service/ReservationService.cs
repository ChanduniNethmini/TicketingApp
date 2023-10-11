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
                TravelerID = reservation.TravelerID,
                ReservationDate = reservation.ReservationDate,
                BookingDate = DateTime.Now,
                TrainID = reservation.TrainID,
                StartLocation = reservation.StartLocation,
                Destination = reservation.Destination,
                TrainClass = reservation.TrainClass,
                DepartureTime = reservation.DepartureTime,
                Status = 1, //Confirmed
                SeatCount = reservation.SeatCount,
                Price = 0
            };

            if ((newReservation.ReservationDate - newReservation.BookingDate).Days <= 30)
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
                    if (trainService.TrainClass == "A")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 100;
                    }
                    else if (trainService.TrainClass == "B")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 75;
                    }
                    else if (trainService.TrainClass == "C")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 50;
                    }

                    // Calculate new RemainingSeats
                    int newRemainingSeats = trainService.RemainingSeats - newReservation.SeatCount;

                    if (newRemainingSeats >= 0)
                    {
                        var existingReservations = _reservationCollection
                        .Find(r => r.TravelerID == newReservation.TravelerID && r.Status != 3)
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
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (existingReservation != null)
            {
                if ((updatedReservation.ReservationDate - DateTime.Now).Days >= 5)
                {
                    updatedReservation.ID = id;
                    _reservationCollection.ReplaceOne(r => r.ID == id, updatedReservation);
                    return true;
                }
            }

            return false;
        }

        //Cancel a reservation
        // status = 3 means canceled orders
        public bool CancelReservation(string reservationId)
        {
            var reservationToCancel = _reservationCollection
                .Find(r => r.ID == reservationId && r.Status != 3)
                .FirstOrDefault();

            if (reservationToCancel != null)
            {
                if ((reservationToCancel.ReservationDate - DateTime.Now).Days >= 5)
                {
                    reservationToCancel.Status = 3;
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
        public List<Reservation> GetExistingReservationsForTraveler(int travelerId)
        {

            return _reservationCollection
                .Find(r => r.TravelerID == travelerId && r.Status == 1)
                .ToList();
        }

        // status = 2 means existing bookings
        public List<Reservation> GetReservationHistoryForTraveler(int travelerId)
        {

            return _reservationCollection
                .Find(r => r.TravelerID == travelerId && r.Status != 1)
                .ToList();
        }
    }
}
