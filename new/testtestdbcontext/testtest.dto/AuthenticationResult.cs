using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.dto
{
    public class AuthenticationResult
    {   public string Username { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        
    }
}
