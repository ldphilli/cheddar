using System;
using System.IdentityModel.Tokens.Jwt;
namespace Cheddar.Api.Shared {
    public class jwtManagementToken {

        public string GetUserIdFromToken(string token) {
            Console.WriteLine(token);
            var decodedToken = new JwtSecurityToken(jwtEncodedString: token);
            string userId = decodedToken.Subject;
            return userId;
        }
    }
}