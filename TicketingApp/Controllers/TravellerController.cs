using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketingApp.Models;
using TicketingApp.Service;

namespace TicketingApp.Controllers
{
    [Route("api/travelers")]
    [ApiController]
    public class TravelerController : ControllerBase
    {
        private readonly TravelerService _travelerService;

        public TravelerController(TravelerService travelerService)
        {
            _travelerService = travelerService;
        }

        [HttpPost]
        [Route("login")]
        public ActionResult TravelerLogin(string nic, string password)
        {
            // Validate the input parameters (e.g., nic and password)
            if (string.IsNullOrWhiteSpace(nic) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("NIC and password are required.");
            }

            // Call your authentication service to verify the traveler's credentials
            bool authenticated = _travelerService.AuthenticateTraveler(nic, password);

            if (authenticated)
            {
                // Generate and return an authentication token (JWT) as a response
                string token = _travelerService.GenerateToken(nic);

                return Ok(new { Token = token, Message = "Login successful." });
            }

            return Unauthorized("Invalid NIC or password.");
        }


        [HttpPost]
        [Route("add")]
        public ActionResult CreateTraveler(Traveller traveler)
        {
            bool created = _travelerService.CreateTraveler(traveler);
            if (created)
            {
                return Ok("Traveler profile created successfully.");
            }
            return BadRequest("Traveler profile creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateTraveler(string nic, Traveller traveler)
        {
            bool updated = _travelerService.UpdateTraveler(nic, traveler);
            if (updated)
            {
                return Ok("Traveler profile updated successfully.");
            }
            return BadRequest("Traveler profile update failed due to validation constraints or profile not found.");
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteTraveler(string nic)
        {
            bool canceled = _travelerService.DeleteTraveler(nic);
            if (canceled)
            {
                return Ok("Traveler profile deleted successfully.");
            }
            return BadRequest("Traveler profile delete failed due to validation constraints or Profile not found.");
        }

        [HttpPut("update/active/{id}")]
        public ActionResult ActivateTraveler(string nic)
        {
            bool statusupdated = _travelerService.ActivateTraveler(nic);
            if (statusupdated)
            {
                return Ok($"Traveler account activated successfully.");
            }
            return BadRequest("Traveler account activation failed due to Profile not found.");
        }

        [HttpPut("update/deactivate/{id}")]
        public ActionResult DeactiveTraveler(string nic)
        {
            bool statusupdated = _travelerService.DeactivateTraveler(nic);
            if (statusupdated)
            {
                return Ok($"Traveler account deactivated successfully.");
            }
            return BadRequest("Traveler account deactivation failed due to Profile not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<Traveller>> GetAllTravelers()
        {
            List<Traveller> travellers = _travelerService.GetAllTravelers();
            if (travellers.Count != 0)
            {
                return Ok(travellers);
            }
            return BadRequest("Travellers not found.");
        }
    }
}
