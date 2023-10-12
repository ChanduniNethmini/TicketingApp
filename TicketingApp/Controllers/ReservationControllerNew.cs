using Microsoft.AspNetCore.Mvc;
using TicketingApp.Models;
using TicketingApp.Service;

namespace TicketingApp.Controllers
{
    [Route("api/reservationsNew")]
    [ApiController]
    public class ReservationControllerNew : ControllerBase
    {
        private readonly ReservationServiceNew _reservationNewService;

        public ReservationControllerNew(ReservationServiceNew reservationNewService)
        {
            _reservationNewService = reservationNewService;
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateReservationNew(ReservationNew reservation)
        {
            bool created = _reservationNewService.CreateReservationNew(reservation);
            if (created)
            {
                return Ok("Reservation created successfully.");
            }
            return BadRequest("Reservation creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateReservationNew(string id, ReservationNew updatedReservation)
        {
            bool updated = _reservationNewService.UpdateReservationNew(id, updatedReservation);
            if (updated)
            {
                return Ok("Reservation updated successfully.");
            }
            return BadRequest("Reservation update failed due to validation constraints or reservation not found.");
        }
    }
}
