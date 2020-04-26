﻿using Newtonsoft.Json;
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
        public string Method { get; set; }
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
        public string Code { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class AccessToken
    {
        public AttendeeViewModel Attendee { get; set; }

        [JsonProperty(PropertyName = "accessToken")]
        public string Token { get; set; }
    }


}
