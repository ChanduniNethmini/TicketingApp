using MongoDB.Driver;
using TicketingApp.Models;

namespace TicketingApp.Service
{
    public class ReservationServiceNew
    {
        private readonly IMongoCollection<ReservationNew> _reservationNewCollection;

        public ReservationServiceNew(IMongoDatabase database)
        {
            _reservationNewCollection = database.GetCollection<ReservationNew>("reservationNew");
        }

        public bool CreateReservationNew(ReservationNew reservation)
        {
            ReservationNew newReservation = new ReservationNew
            {
                TravelerID = reservation.TravelerID,
                ReservationDate = reservation.ReservationDate,
                BookingDate = reservation.BookingDate,
                TrainID = reservation.TrainID,
                StartLocation = reservation.StartLocation,
                Destination = reservation.Destination,
                TrainClass = reservation.TrainClass,
                Status = 1,
                SeatCount = reservation.SeatCount,
            };

            _reservationNewCollection.InsertOne(newReservation);
            return true;
        }

        public bool UpdateReservationNew(string id, ReservationNew updatedReservation)
        {
            var existingReservationNew = _reservationNewCollection
                .Find(r => r.ID == id && r.Status == 1)
                .FirstOrDefault();

            if (existingReservationNew != null)
            {

                updatedReservation.ID = id;
                _reservationNewCollection.ReplaceOne(r => r.ID == id, updatedReservation);
                return true;

            }

            return false;
        }
    }
}
