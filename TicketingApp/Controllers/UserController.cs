using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TicketingApp.Models;
using TicketingApp.Service;

namespace TicketingApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public ActionResult UserLogin(string email, string password)
        {
            // Validate the input parameters (e.g., username and password)
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Email and password are required.");
            }

            // Call your authentication service to verify the user's credentials
            List<Users> authenticatedUsers = _userService.AuthenticateUser(email, password);

            if (authenticatedUsers.Count > 0)
            {
                var authenticatedUser = authenticatedUsers[0];

                string userRole = authenticatedUser.Role;

                // Generate and return an authentication token (JWT) as a response
                string token = _userService.GenerateToken(email);

                return Ok(new { Token = token, userRole, Message = "Login successful." });
            }

            return Unauthorized("Invalid email or password.");
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateUser(Users user)
        {
            bool created = _userService.CreateUser(user);
            if (created)
            {
                return Ok("User profile created successfully.");
            }
            return BadRequest("User profile creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateUser(string id, Users user)
        {
            bool updated = _userService.UpdateUser(id, user);
            if (updated)
            {
                return Ok("User profile updated successfully.");
            }
            return BadRequest("User profile update failed due to validation constraints or profile not found.");
        }

        [HttpDelete("delete/{id}")]
        public ActionResult DeleteUser(string id)
        {
            bool deleted = _userService.DeleteUser(id);
            if (deleted)
            {
                return Ok("User profile deleted successfully.");
            }
            return BadRequest("User profile deletion failed due to validation constraints or profile not found.");
        }

        [HttpPut("update/active/{id}")]
        public ActionResult ActivateUser(string id)
        {
            bool statusUpdated = _userService.ActivateUser(id);
            if (statusUpdated)
            {
                return Ok($"User account activated successfully.");
            }
            return BadRequest("User account activation failed due to profile not found.");
        }

        [HttpPut("update/deactivate/{id}")]
        public ActionResult DeactivateUser(string id)
        {
            bool statusUpdated = _userService.DeactivateUser(id);
            if (statusUpdated)
            {
                return Ok($"User account deactivated successfully.");
            }
            return BadRequest("User account deactivation failed due to profile not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<Users>> GetAllUsers()
        {
            List<Users> users = _userService.GetAllUsers();
            if (users.Count != 0)
            {
                return Ok(users);
            }
            return BadRequest("Users not found.");
        }
    }
}
