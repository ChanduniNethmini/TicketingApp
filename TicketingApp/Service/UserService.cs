using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using TicketingApp.Models;

public class UserService
{
    private readonly IMongoCollection<Users> _userCollection;
    private readonly IMongoCollection<TrainSchedule> _trainScheduleCollection;

    public UserService(IMongoDatabase database)
    {
        _userCollection = database.GetCollection<Users>("users");
        _trainScheduleCollection = database.GetCollection<TrainSchedule>("trainSchedule");
    }

    public bool CreateUser(Users users)
    {
        Users newUsers = new Users
        {
            NIC = users.NIC,
            Name = users.Name,
            Phone = users.Phone,
            DOB = users.DOB,
            Email = users.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(users.Password),
            ConfirmPassword = BCrypt.Net.BCrypt.HashPassword(users.ConfirmPassword),
            Status = 1,
            Role = users.Role,
        };

        var UserFilter = Builders<Users>.Filter.Eq(u => u.NIC, newUsers.NIC);

        var UserFilter2 = _userCollection.Find(UserFilter).FirstOrDefault();

        if (UserFilter2 == null)
        {
            _userCollection.InsertOne(newUsers);
            return true;
        }
        return false;
    }

    public bool UpdateUser(string id, Users traveler)
    {
        var existingUser = _userCollection
                .Find(u => u.ID == id && u.Status == 1)
                .FirstOrDefault();

        if (existingUser != null)
        {
            existingUser.ID = id;
            _userCollection.ReplaceOne(r => r.ID == id, existingUser);

            return true;
        }
        return false;
    }

    public bool DeleteUser(string id)
    {
        var userToDelete = _userCollection
                 .Find(r => r.ID == id)
                 .FirstOrDefault();

        if (userToDelete != null)
        {
            _userCollection.DeleteOne(r => r.ID == id);
            return true;
        }
        return false;

    }

    public bool ActivateUser(string id)
    {
        var userToActivate = _userCollection
             .Find(r => r.ID == id)
             .FirstOrDefault();

        if (userToActivate != null)
        {
            var update = Builders<Users>.Update.Set(t => t.Status, 1);
            userToActivate.ID = id;
            _userCollection.UpdateOne(r => r.ID == id, update);
            return true;
        }
        return false;
    }

    public bool DeactivateUser(string id)
    {
        var userToDeactivate = _userCollection
             .Find(r => r.ID == id)
             .FirstOrDefault();

        if (userToDeactivate != null)
        {
            var update = Builders<Users>.Update.Set(t => t.Status, 0);
            userToDeactivate.ID = id;
            _userCollection.UpdateOne(r => r.ID == id, update);
            return true;
        }
        return false;
    }

    public List<Users> GetAllUsers()
    {
        return _userCollection.AsQueryable().ToList();
    }

    public bool AuthenticateUser(string email, string password)
    {
        var existingUser = _userCollection
            .Find(r => r.Email == email)
            .FirstOrDefault();

        if (existingUser != null && BCrypt.Net.BCrypt.Verify(password, existingUser.Password))
        {
            return true;
        }
        return false;
    }

    public string GenerateToken(string email)
    {
        var existingUser = _userCollection
            .Find(r => r.Email == email)
            .FirstOrDefault();

        if (existingUser.Role == "BOfficer")
        {
            var filter = Builders<TrainSchedule>.Filter.And(
            Builders<TrainSchedule>.Filter.Eq("IsActive", 1),
            Builders<TrainSchedule>.Filter.Lte("StartTime", DateTime.Now)
            );

            List<TrainSchedule> TrainSchedules = _trainScheduleCollection.Find(filter).ToList();

            foreach (TrainSchedule trainSchedule in TrainSchedules)
            {
                if (DateTime.TryParse(trainSchedule.StartTime, out DateTime StartTime))
                {
                    if (StartTime < DateTime.Now)
                    {
                        var updateFilter = Builders<TrainSchedule>.Filter.Eq(r => r.ID, trainSchedule.ID);
                        var update = Builders<TrainSchedule>.Update.Set(r => r.IsActive, 0);
                        _trainScheduleCollection.UpdateOne(updateFilter, update);
                    }
                }
            }
        }


        // Generate a secure key
        var key = GenerateSecureKey();

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private byte[] GenerateSecureKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var key = new byte[32];
            rng.GetBytes(key);
            return key;
        }
    }
}
