using MongoDB.Driver;
using TicketingApp.Models;

namespace TicketingApp.Service
{
    public class TrainService
    {
        private readonly IMongoCollection<TrainSchedule> _trainScheduleCollection;
        public TrainService(IMongoDatabase database)
        {
            _trainScheduleCollection = database.GetCollection<TrainSchedule>("trainSchedule");
        }

        public bool CreateTrain(TrainSchedule trainSchedule)
        {
            try
            {
                TrainSchedule newtrainSchedule = new TrainSchedule
                {
                    Name = trainSchedule.Name,
                    Date = trainSchedule.Date,
                    StartTime = trainSchedule.StartTime,
                    StartLocation = trainSchedule.StartLocation,
                    Destination = trainSchedule.Destination,
                    Class = trainSchedule.Class,
                    SeatCount = trainSchedule.SeatCount,
                    IsActive = 1,
                    RemainingSeats = trainSchedule.SeatCount,
                    StoppingStations = trainSchedule.StoppingStations
                };

                _trainScheduleCollection.InsertOne(newtrainSchedule);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateTrainSchedule(string id, TrainSchedule updatedSchedule)
        {
            var existingSchedule = _trainScheduleCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (existingSchedule != null)
            {
                updatedSchedule.ID = id;
                _trainScheduleCollection.ReplaceOne(r => r.ID == id, updatedSchedule);
                return true;
            }

            return false;
        }

        public bool CancelTrainSchedule(string id)
        {
            var trainScheduleToCancel = _trainScheduleCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (trainScheduleToCancel != null)
            {
                if (trainScheduleToCancel.RemainingSeats == trainScheduleToCancel.SeatCount)
                {
                    _trainScheduleCollection.ReplaceOne(r => r.ID == id, trainScheduleToCancel);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public List<TrainSchedule> GetAllTrains()
        {
            return _trainScheduleCollection
                .Find(r => r.IsActive != 0)
                .ToList();
        }

        public List<TrainSchedule> GetTrainById(string id)
        {

            return _trainScheduleCollection
                .Find(r => r.ID == id)
                .ToList();
        }
    }
}
