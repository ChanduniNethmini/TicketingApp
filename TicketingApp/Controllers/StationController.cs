using Microsoft.AspNetCore.Mvc;
using TicketingApp.Models;
using TicketingApp.Service;

namespace TicketingApp.Controllers
{
    [Route("api/stations")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly StationService _stationService;

        public StationController(StationService stationService)
        {
            _stationService = stationService;
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateReservation(Station reservation)
        {
            bool created = _stationService.CreateStation(reservation);
            if (created)
            {
                return Ok("Station created successfully.");
            }
            return BadRequest("Station creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateReservation(string id, Station updatedReservation)
        {
            bool updated = _stationService.UpdateStation(id, updatedReservation);
            if (updated)
            {
                return Ok("Station updated successfully.");
            }
            return BadRequest("Station update failed due to validation constraints or Station not found.");
        }

        [HttpDelete("delete/{id}")]
        public ActionResult CancelReservation(string id)
        {
            bool deleted = _stationService.DeleteStation(id);
            if (deleted)
            {
                return Ok("Station deleted successfully.");
            }
            return BadRequest("Station deleting failed due to validation constraints or Station not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<Station>> GetAllStations()
        {
            List<Station> stations = _stationService.GetAllStations();

            return Ok(stations);
        }
    }
}
