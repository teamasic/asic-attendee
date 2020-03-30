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
            public const string LOGIN_BY_USERNAME_PASSWORD = "1";
            public const string LOGIN_BY_FACE = "2";
            public const string GET_DATA_BY_ATTENDEE_CODE = "3";
            public const string AES_KEY_PATH = "aes";
        }

        public class NetworkRoute
        {
            public const string LOGIN = "login";
            public const string REFRESH_ATTENDANCE_DATA = "refresh";
            public const string CHANGE_REQUEST = "change";


        }


        public class ErrorMessage
        {
            public const string GROUP_NOT_FOUND = "This group cannot be found.";
            public const string GROUP_NAME_TOO_LONG = "Group name must be between 1 and 100 characters.";

            public const string LOGIN_FAIL = "User credential is not valid.";
            public const string CANNOT_GET_LOCAL_IP_ADDRESS = "Cannot get local ip address.";

            public static string ATTENDEE_NOT_FOUND = "Attendee not found";

            public static string NETWORK_ERROR = "Cannot connect to supervisor";

            public const string CHANGE_REQUEST_FAIL = "Failed to create change request.";

            public const string NOT_VALID_USER = "Not valid attendee";
        }

        public enum RolesEnum
        {
            ATTENDEE = 1,
            ADMIN = 2,
            SUPERVISOR = 3
        }
    }
}
