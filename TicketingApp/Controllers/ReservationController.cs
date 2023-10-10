using Microsoft.AspNetCore.Mvc;
using TicketingApp.Models;
using TicketingApp.Service;
using System.Collections.Generic;

namespace TicketingApp.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateReservation(Reservation reservation)
        {
            bool created = _reservationService.CreateReservation(reservation);
            if (created)
            {
                return Ok("Reservation created successfully.");
            }
            return BadRequest("Reservation creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateReservation(string id, Reservation updatedReservation)
        {
            bool updated = _reservationService.UpdateReservation(id, updatedReservation);
            if (updated)
            {
                return Ok("Reservation updated successfully.");
            }
            return BadRequest("Reservation update failed due to validation constraints or reservation not found.");
        }

        [HttpDelete("cancel/{id}")]
        public ActionResult CancelReservation(string id)
        {
            bool canceled = _reservationService.CancelReservation(id);
            if (canceled)
            {
                return Ok("Reservation canceled successfully.");
            }
            return BadRequest("Reservation cancellation failed due to validation constraints or reservation not found.");
        }

        [HttpGet("traveler/{travelerId}")]
        public ActionResult<List<Reservation>> GetExistingReservationsForTraveler(int travelerId)
        {
            List<Reservation> reservations = _reservationService.GetExistingReservationsForTraveler(travelerId);
            if (reservations.Count != 0)
            {
                return Ok(reservations);
            }
            return BadRequest("Existing Reservation not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<Reservation>> GetAllReservations()
        {
            List<Reservation> reservations = _reservationService.GetAllReservations();
            foreach (var reservation in reservations)
            {
                reservation.ReservationDate = reservation.ReservationDate.AddHours(5.5);
                reservation.BookingDate = reservation.BookingDate.AddHours(5.5);
            }
            return Ok(reservations);
        }

        [HttpGet("traveler/history/{travelerId}")]
        public ActionResult<List<Reservation>> GetReservationHistoryForTraveler(int travelerId)
        {
            List<Reservation> reservations = _reservationService.GetReservationHistoryForTraveler(travelerId);
            if (reservations.Count != 0)
            {
                return Ok(reservations);
            }
            return BadRequest("Reservation History not found.");
        }
    }
}
