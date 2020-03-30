using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class LoginViewModel
    {
        public string AttendeeCode { get; set; }
        public string FaceData { get; set; }
        public string LoginMethod { get; set; }
    }

    public class UserAuthentication
    {
        [Required]
        public string FirebaseToken { get; set; }
    }

    public class AuthorizedUser
    {
        public UserViewModel User { get; set; }
        public string[] Roles { get; set; }
        public string AccessToken { get; set; }
    }
    public class UserViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string RollNumber { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Birthdate { get; set; }
    }

    public class AccessToken
    {
        public AttendeeViewModel Attendee { get; set; }

        [JsonProperty(PropertyName = "accessToken")]
        public string Token { get; set; }
    }


}
