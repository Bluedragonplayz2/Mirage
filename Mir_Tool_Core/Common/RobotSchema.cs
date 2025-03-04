namespace Mir_Utilities.Common;
using System;
using System.Security.Cryptography;
using System.Text;

public class RobotSchema
{
    public class Robot
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        private string _username;
        private string _password;
        public string Username
        {
            get=>_username;
            set
            {
                _username = value;
                
            }
        }

        public string Password
        {
            get=>_password;
            set
            {
                _password = value;
                
            }
        }

        private bool _hashValid;
        private string _authId;
        public string AuthId 
        { 
            get
            {
                if (!_hashValid)
                {
                    _authId = GenerateHash();
                    _hashValid = true;
                }
                return _authId;

            }
        }
        public Robot(string name, string ip, string port, string username, string password)
        {
            Name = name;
            Ip = ip;
            Port = port;
            Username = username;
            Password = password;
        }

        private string GenerateHash()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert hash to hexadecimal string
                StringBuilder hashHex = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashHex.Append(b.ToString("x2"));
                }

                // Concatenate username, colon, and hash
                string concatenated = $"{Username}:{hashHex}";

                // Convert concatenated string to Base64
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(concatenated));

                // Return with "Basic " prefix
                return $"Basic {base64}";
            }
        }
    }
}