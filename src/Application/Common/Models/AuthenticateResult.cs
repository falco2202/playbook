using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class AuthenticateResult
    {
        public bool Succeeded { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public IEnumerable<string> Errors { get; private set; }
        private AuthenticateResult() { }

        public static AuthenticateResult Success(string accessToken, string refreshToken)
        {
            return new AuthenticateResult
            {
                Succeeded = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public static AuthenticateResult Failure(string error)
        {
            return new AuthenticateResult
            {
                Succeeded = false,
                Errors = new[] { error }
            };
        }

        public static AuthenticateResult Failure(IEnumerable<string> errors)
        {
            return new AuthenticateResult
            {
                Succeeded = false,
                Errors = errors
            };
        }


    }
}
