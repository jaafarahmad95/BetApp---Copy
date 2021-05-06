using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace testtest.helpers
{
    public class Passhash
    {
        private HashAlgorithm _sha;
        public Passhash()
        {
            _sha = SHA256.Create();
        }
        public string gethash(string pass)
        {
           var z = Encoding.ASCII.GetBytes(pass);
           byte[] x = _sha.ComputeHash(z);
           var pass2 =  Convert. ToBase64String(x);
           return pass2;

        }
    }
}
