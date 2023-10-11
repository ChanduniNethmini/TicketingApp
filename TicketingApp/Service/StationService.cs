using MongoDB.Driver;
using TicketingApp.Models;

namespace TicketingApp.Service
{
    public class StationService
    {
        private readonly IMongoCollection<Station> _stationCollection;
        public StationService(IMongoDatabase database)
        {
            _stationCollection = database.GetCollection<Station>("station");
        }

        public bool CreateStation(Station station)
        {
            try
            {
                Station newStation = new Station
                {
                    StationName = station.StationName,
                    IsActive = 1
                };

                _stationCollection.InsertOne(newStation);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateStation(string id, Station updatedStation)
        {
            var existingStation = _stationCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (existingStation != null)
            {
                updatedStation.ID = id;
                _stationCollection.ReplaceOne(r => r.ID == id, updatedStation);
                return true;
            }

            return false;
        }

        public bool DeleteStation(string id)
        {
            var StationToDelete = _stationCollection
                .Find(r => r.ID == id)
                .FirstOrDefault();

            if (StationToDelete != null)
            {
                _stationCollection.ReplaceOne(r => r.ID == id, StationToDelete);
                return true;
            }
            return false;
        }

        public List<Station> GetAllStations()
        {
            return _stationCollection
                .Find(r => r.IsActive != 0)
                .ToList();
        }
    }
}
