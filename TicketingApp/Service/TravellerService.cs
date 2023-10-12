using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TicketingApp.Models;

public class TravelerService
{
    private readonly IMongoCollection<Traveller> _travellerCollection;

    public TravelerService(IMongoDatabase database)
    {
        _travellerCollection = database.GetCollection<Traveller>("travellers");
    }

    public bool CreateTraveler(Traveller traveler)
    {
        Traveller newTraveller = new Traveller
        {
            NIC = traveler.NIC,
            Name = traveler.Name,
            Phone = traveler.Phone,
            DOB = traveler.DOB,
            Email = traveler.Email,
            Password = traveler.Password,
            ConfirmPassword = traveler.ConfirmPassword,
            Status = 1
        };

        var TravellerFilter = Builders<Traveller>.Filter.Eq(ts => ts.NIC, newTraveller.NIC);

        var traveller2 = _travellerCollection.Find(TravellerFilter).FirstOrDefault();

        if (traveller2 == null)
        {
            _travellerCollection.InsertOne(newTraveller);
            return true;
        }
        return false;
    }

    public bool UpdateTraveler(string nic, Traveller traveler)
    {
        var existingTraveler = _travellerCollection
                .Find(r => r.NIC == nic && r.Status == 1)
                .FirstOrDefault();

        if (existingTraveler != null)
        {
            existingTraveler.NIC = nic;
            _travellerCollection.ReplaceOne(r => r.NIC == nic, existingTraveler);

            return true;
        }
        return false;
    }

    public bool DeleteTraveler(string nic)
    {
        var existingTraveler = _travellerCollection
                 .Find(r => r.NIC == nic)
                 .FirstOrDefault();

        if (existingTraveler != null)
        {
            _travellerCollection.DeleteOne(r => r.NIC == nic);
            return true;
        }
        return false;

    }

    public bool ActivateTraveler(string nic)
    {
        var existingTraveler = _travellerCollection
             .Find(r => r.NIC == nic)
             .FirstOrDefault();

        if (existingTraveler != null)
        {
            var update = Builders<Traveller>.Update.Set(t => t.Status, 1);
            existingTraveler.NIC = nic;
            _travellerCollection.UpdateOne(r => r.NIC == nic, update);
            return true;
        }
        return false;
    }

    public bool DeactivateTraveler(string nic)
    {
        var existingTraveler = _travellerCollection
             .Find(r => r.NIC == nic)
             .FirstOrDefault();

        if (existingTraveler != null)
        {
            var update = Builders<Traveller>.Update.Set(t => t.Status, 0);
            existingTraveler.NIC = nic;
            _travellerCollection.UpdateOne(r => r.NIC == nic, update);
            return true;
        }
        return false;
    }

    public List<Traveller> GetAllTravelers()
    {
        return _travellerCollection.AsQueryable().ToList();
    }

    public bool AuthenticateTraveler(string nic, string password)
    {
        var existingTraveler = _travellerCollection
            .Find(r => r.NIC == nic)
            .FirstOrDefault();

        if (existingTraveler != null && existingTraveler.Password == password)
        {
            return true;
        }
        return false;
    }

    public string GenerateToken(string nic)
    {
        // Generate a secure key
        var key = GenerateSecureKey();

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, nic)
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
            var key = new byte[32]; // 256 bits for a secure key
            rng.GetBytes(key);
            return key;
        }
    }
}
