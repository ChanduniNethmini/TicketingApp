using MongoDB.Driver;
using TicketingApp.Models;

namespace TicketingApp.Service
{
    public class ResNewService
    {
        //create the database connection
        private readonly IMongoCollection<ResNew> _reservationNewCollection;
        private readonly IMongoCollection<TrainSchedule> _trainScheduleCollection;


        public ResNewService(IMongoDatabase database)
        {
            _reservationNewCollection = database.GetCollection<ResNew>("reservationsNew");
            _trainScheduleCollection = database.GetCollection<TrainSchedule>("trainSchedule");
        }

        //Create a new reservation
        public bool CreateResNew(ResNew reservation)
        {
            ResNew newResNew = new ResNew
            {
                NIC = reservation.NIC,
                ReservationDate = reservation.ReservationDate,
                BookingDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                TrainID = reservation.TrainID,
                StartLocation = reservation.StartLocation,
                Destination = reservation.Destination,
                TrainClass = reservation.TrainClass,
                DepartureTime = reservation.DepartureTime,
                Status = "0",
                SeatCount = reservation.SeatCount,
                Price = "0",
            };

            DateTime reservationDate = DateTime.Parse(newResNew.ReservationDate);
            DateTime bookingDate = DateTime.Parse(newResNew.BookingDate);

            TimeSpan dateDifference = reservationDate - bookingDate;
            int daysDifference = dateDifference.Days;

            if (daysDifference <= 30)
            {
                // Find the corresponding TrainService document
                var trainServiceFilter = Builders<TrainSchedule>.Filter.Eq(ts => ts.ID, newResNew.TrainID)
                    & Builders<TrainSchedule>.Filter.Eq(ts => ts.Date, newResNew.ReservationDate);

                var trainService = _trainScheduleCollection.Find(trainServiceFilter).FirstOrDefault();

                if (trainService != null)
                {
                    var Tdestination = trainService.StoppingStations
                    .Find(ss => ss.StationName == newResNew.Destination);

                    var TstartLocation = trainService.StoppingStations
                        .Find(ss => ss.StationName == newResNew.StartLocation);

                    decimal price = 0;
                    if (newResNew.TrainClass == "A")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 100;
                    }
                    else if (newResNew.TrainClass == "B")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 75;
                    }
                    else if (newResNew.TrainClass == "C")
                    {
                        price = (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 50;
                    }

                    // Calculate new RemainingSeats
                    int newRemainingSeats = trainService.RemainingSeats - int.Parse(newResNew.SeatCount);

                    if (newRemainingSeats >= 0)
                    {
                        var existingResNewNews = _reservationNewCollection
                        .Find(r => r.NIC == newResNew.NIC && r.Status == "1")
                        .ToList();

                        if (existingResNewNews.Count >= 4)
                        {
                            return false;
                        }

                        // Update RemainingSeats in TrainService
                        var update = Builders<TrainSchedule>.Update.Set(ts => ts.RemainingSeats, newRemainingSeats);
                        _trainScheduleCollection.UpdateOne(trainServiceFilter, update);
                        newResNew.Price = (price * int.Parse(newResNew.SeatCount)).ToString();
                        _reservationNewCollection.InsertOne(newResNew);
                        return true;
                    }
                }


            }

            return false;
        }



        //Update an existing reservation
        public bool UpdateResNew(string id, ResNew updatedResNew)
        {
            var existingResNew = _reservationNewCollection
                .Find(r => r.ID == id && r.Status == "0")
                .FirstOrDefault();

            if (existingResNew != null)
            {
                DateTime reservationDate = DateTime.Parse(updatedResNew.ReservationDate);

                TimeSpan dateDifference = reservationDate - DateTime.Now.Date;
                int daysDifference = dateDifference.Days;

                if (daysDifference >= 5)
                {
                    updatedResNew.Price = ((int.Parse(existingResNew.Price) / int.Parse(existingResNew.SeatCount)) * int.Parse(updatedResNew.SeatCount)).ToString();
                    updatedResNew.ID = id;
                    _reservationNewCollection.ReplaceOne(r => r.ID == id, updatedResNew);
                    return true;
                }
            }

            return false;
        }

        //Cancel a reservation
        // status = 3 means canceled 
        public bool CancelResNew(string reservationId)
        {
            var reservationToCancel = _reservationNewCollection
                .Find(r => r.ID == reservationId && r.Status != "3" && r.Status != "2")
                .FirstOrDefault();

            var trainServiceFilter = Builders<TrainSchedule>.Filter.Eq(ts => ts.ID, reservationToCancel.TrainID);

            var trainService = _trainScheduleCollection.Find(trainServiceFilter).FirstOrDefault();

            if (reservationToCancel != null)
            {
                DateTime reservationDate = DateTime.Parse(reservationToCancel.ReservationDate);

                TimeSpan dateDifference = reservationDate - DateTime.Now.Date;
                int daysDifference = dateDifference.Days;

                if (daysDifference >= 5)
                {
                    reservationToCancel.Status = "3";
                    int newRemainingSeats = trainService.RemainingSeats + int.Parse(reservationToCancel.SeatCount);
                    var update = Builders<TrainSchedule>.Update.Set(ts => ts.RemainingSeats, newRemainingSeats);
                    _trainScheduleCollection.UpdateOne(trainServiceFilter, update);
                    _reservationNewCollection.ReplaceOne(r => r.ID == reservationId, reservationToCancel);
                    return true;
                }
            }

            return false;
        }

        // Get all reservations
        public List<ResNew> GetAllResNews()
        {
            return _reservationNewCollection
                .Find(r => r.Status != "3")
                .ToList();
        }


        //Get a list of reservations for a specific traveler
        // status = 1 means bookings confirmed
        public List<ResNew> GetExistingResNewsForTraveler(string nic)
        {

            return _reservationNewCollection
                .Find(r => r.NIC == nic && r.Status == "1" && r.Status == "0")
                .ToList();
        }

        // status = 2 means Completed bookings
        public List<ResNew> GetResNewHistoryForTraveler(string nic)
        {

            return _reservationNewCollection
                .Find(r => r.NIC == nic && r.Status != "2")
                .ToList();
        }

        public List<ResNew> GetResNewbyID(string id)
        {

            return _reservationNewCollection
                .Find(r => r.ID == id && r.Status == "0")
                .ToList();
        }

        public bool ConfirmResNew(string id)
        {
            var existingResNewNew = _reservationNewCollection
                 .Find(r => r.ID == id)
                 .FirstOrDefault();

            if (existingResNewNew != null && existingResNewNew.Status != "1")
            {
                var update = Builders<ResNew>.Update.Set(t => t.Status, "1");
                existingResNewNew.ID = id;
                _reservationNewCollection.UpdateOne(r => r.ID == id, update);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
