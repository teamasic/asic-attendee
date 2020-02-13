using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework
{
    public class Constants
    {
        public class Constant
        {
            public const int GROUP_NAME_MAX_LENGTH = 100;
        }

        public class ErrorMessage
        {
            public const string GROUP_NOT_FOUND = "This group cannot be found.";
            public const string GROUP_NAME_TOO_LONG = "Group name must be between 1 and 100 characters.";

            public const string LOGIN_FAIL = "User credential is not valid.";
            public const string CANNOT_GET_LOCAL_IP_ADDRESS = "Cannot get local ip address.";

            public static string ATTENDEE_NOT_FOUND = "Attendee not found";
        }
    }
}
