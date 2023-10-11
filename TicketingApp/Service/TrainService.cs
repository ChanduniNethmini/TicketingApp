using MongoDB.Driver;
using TicketingApp.Dtos;
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
                    TrainClass = trainSchedule.TrainClass,
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

        public List<TrainSearchResult> SearchTrains(string fromStationName, string toStationName, DateTime date, int minAvailableSeatCount)
        {
            var filterBuilder = Builders<TrainSchedule>.Filter;
            var filter = filterBuilder.Gte("Date", date) &
                         filterBuilder.Gte("RemainingSeats", minAvailableSeatCount);

            // Fetch all train schedules that meet the date and seat count criteria
            var matchingTrains = _trainScheduleCollection
                .Find(filter)
                .ToList();

            // Filter and calculate the ticket price in memory
            var searchResults = matchingTrains
                .Where(trainSchedule =>
                    trainSchedule.StoppingStations.Any(ss => ss.StationName == fromStationName) &&
                    trainSchedule.StoppingStations.Any(ss => ss.StationName == toStationName) &&
                    GetStationCount(trainSchedule, fromStationName) < GetStationCount(trainSchedule, toStationName))
                .Select(trainSchedule => new TrainSearchResult
                {
                    Name = trainSchedule.Name,
                    Date = trainSchedule.Date,
                    StartLocation = fromStationName,
                    Destination = toStationName,
                    TrainClass = trainSchedule.TrainClass,
                    AvailableSeats = trainSchedule.RemainingSeats,
                    TicketPrice = CalculateTicketPrice(trainSchedule, fromStationName, toStationName),
                    TotalPrice = CalculateTicketPrice(trainSchedule, fromStationName, toStationName) * minAvailableSeatCount,
                    StartLocationArrivalTime = GetArrivalTime(trainSchedule, fromStationName),
                    StartLocationDepartureTime = GetDepartureTime(trainSchedule, fromStationName)
                })
                .ToList();

            return searchResults;
        }

        private int GetStationCount(TrainSchedule trainSchedule, string stationName)
        {
            var stoppingStation = trainSchedule.StoppingStations.Find(ss => ss.StationName == stationName);
            return stoppingStation != null ? stoppingStation.StationCount : 0;
        }
        public DateTime GetArrivalTime(TrainSchedule trainSchedule, string stationName)
        {
            var stoppingStation = trainSchedule.StoppingStations.FirstOrDefault(ss => ss.StationName == stationName);
            return stoppingStation != null ? stoppingStation.ArrivalTime : DateTime.MinValue;
        }

        public DateTime GetDepartureTime(TrainSchedule trainSchedule, string stationName)
        {
            var stoppingStation = trainSchedule.StoppingStations.FirstOrDefault(ss => ss.StationName == stationName);
            return stoppingStation != null ? stoppingStation.DepartureTime : DateTime.MinValue;
        }

        private decimal CalculateTicketPrice(TrainSchedule trainSchedule, string startLocation, string destination)
        {
            var Tdestination = trainSchedule.StoppingStations.Find(ss => ss.StationName == destination);
            var TstartLocation = trainSchedule.StoppingStations.Find(ss => ss.StationName == startLocation);

            if (trainSchedule.TrainClass == "A")
            {
                if (Tdestination != null && TstartLocation != null)
                {
                    return (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 100;
                }
            }
            else if (trainSchedule.TrainClass == "B")
            {
                if (Tdestination != null && TstartLocation != null)
                {
                    return (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 75;
                }
            }
            else if (trainSchedule.TrainClass == "C")
            {
                if (Tdestination != null && TstartLocation != null)
                {
                    return (decimal)(Tdestination.StationCount - TstartLocation.StationCount) * 50;
                }
            }


            return 0;
        }

    }
}
