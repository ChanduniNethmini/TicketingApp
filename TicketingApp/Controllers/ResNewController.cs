using Microsoft.AspNetCore.Mvc;
using TicketingApp.Models;
using TicketingApp.Service;
using System.Collections.Generic;

namespace TicketingApp.Controllers
{
    [Route("api/resNew")]
    [ApiController]
    public class ResNewController : ControllerBase
    {
        private readonly ResNewService _reservationNewService;

        public ResNewController(ResNewService reservationService)
        {
            _reservationNewService = reservationService;
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateResNew(ResNew reservation)
        {
            bool created = _reservationNewService.CreateResNew(reservation);
            if (created)
            {
                return Ok("ResNew created successfully.");
            }
            return BadRequest("ResNew creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateResNew(string id, ResNew updatedResNew)
        {
            bool updated = _reservationNewService.UpdateResNew(id, updatedResNew);
            if (updated)
            {
                return Ok("ResNew updated successfully.");
            }
            return BadRequest("ResNew update failed due to validation constraints or reservation not found.");
        }

        [HttpDelete("cancel/{id}")]
        public ActionResult CancelResNew(string id)
        {
            bool canceled = _reservationNewService.CancelResNew(id);
            if (canceled)
            {
                return Ok("ResNew canceled successfully.");
            }
            return BadRequest("ResNew cancellation failed due to validation constraints or reservation not found.");
        }

        [HttpGet("get/{id}")]
        public ActionResult<List<ResNew>> GetResNewbyID(string id)
        {
            List<ResNew> reservations = _reservationNewService.GetResNewbyID(id);
            if (reservations.Count != 0)
            {
                return Ok(reservations);
            }
            return BadRequest("ResNew not found.");
        }

        [HttpGet("traveler/{nic}")]
        public ActionResult<List<ResNew>> GetExistingResNewsForTraveler(string nic)
        {
            List<ResNew> reservations = _reservationNewService.GetExistingResNewsForTraveler(nic);
            if (reservations.Count != 0)
            {
                return Ok(reservations);
            }
            return BadRequest("Existing ResNew not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<ResNew>> GetAllResNews()
        {
            List<ResNew> reservations = _reservationNewService.GetAllResNews();
            //foreach (var reservation in reservations)
            //{
            //    reservation.ResNewDate = reservation.ResNewDate.AddHours(5.5);
            //    reservation.BookingDate = reservation.BookingDate.AddHours(5.5);
            //}
            return Ok(reservations);
        }

        [HttpGet("traveler/history/{nic}")]
        public ActionResult<List<ResNew>> GetResNewHistoryForTraveler(string nic)
        {
            List<ResNew> reservations = _reservationNewService.GetResNewHistoryForTraveler(nic);
            if (reservations.Count != 0)
            {
                return Ok(reservations);
            }
            return BadRequest("ResNew History not found.");
        }

        [HttpPut("update/confirm/{id}")]
        public ActionResult ConfirmResNew(string id)
        {
            var traveler = _reservationNewService.GetResNewbyID(id);
            if (traveler != null)
            {
                bool statusupdated = _reservationNewService.ConfirmResNew(id);
                if (statusupdated)
                {
                    return Ok($"ResNew Confirmed successfully.");
                }
                return BadRequest("ResNew already Confirmed.");
            }
            else
            {
                return BadRequest("ResNew Confirmation failed due to Profile not found.");
            }
        }
    }
}
