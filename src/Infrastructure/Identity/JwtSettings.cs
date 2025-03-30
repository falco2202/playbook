using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInDays { get; set; }
    }
}
