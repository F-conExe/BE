using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Const
    {
        #region Error Codes

        public static int ERROR_EXCEPTION = -4;
        public static int ERROR_UNAUTHORIZED = -401;
        public static string ERROR_UNAUTHORIZED_MSG = "Unauthorized access";
        public static int ERROR_FORBIDDEN = -403;
        public static string ERROR_FORBIDDEN_MSG = "Access forbidden";
        public static int ERROR_NOT_FOUND = -404;
        public static string ERROR_NOT_FOUND_MSG = "Resource not found";
        public static int ERROR_BAD_REQUEST = -400;
        public static string ERROR_BAD_REQUEST_MSG = "Bad request";

        #endregion

        #region Success Codes

        public static int SUCCESS_CREATE_CODE = 1;
        public static string SUCCESS_CREATE_MSG = "Save data success";
        public static int SUCCESS_READ_CODE = 1;
        public static string SUCCESS_READ_MSG = "Get data success";
        public static int SUCCESS_UPDATE_CODE = 1;
        public static string SUCCESS_UPDATE_MSG = "Update data success";
        public static int SUCCESS_DELETE_CODE = 1;
        public static string SUCCESS_DELETE_MSG = "Delete data success";
        public static int SUCCESS_AUTHENTICATION_CODE = 1;
        public static string SUCCESS_AUTHENTICATION_MSG = "Login success";

        #endregion

        #region Fail Code

        public static int FAIL_CREATE_CODE = -1;
        public static string FAIL_CREATE_MSG = "Save data fail";
        public static int FAIL_READ_CODE = -1;
        public static string FAIL_READ_MSG = "Get data fail";
        public static int FAIL_UPDATE_CODE = -1;
        public static string FAIL_UPDATE_MSG = "Update data fail";
        public static int FAIL_DELETE_CODE = -1;
        public static string FAIL_DELETE_MSG = "Delete data fail";
        public static int FAIL_AUTHENTICATION_CODE = -401;
        public static string FAIL_AUTHENTICATION_MSG = "Login fail";
        public static int FAIL_VALIDATION_CODE = -400;
        public static string FAIL_VALIDATION_MSG = "Validation error";

        #endregion

        #region Warning Code

        public static int WARNING_NO_DATA_CODE = 4;
        public static string WARNING_NO_DATA_MSG = "No data available";
        public static int WARNING_INCOMPLETE_DATA_CODE = 3;
        public static string WARNING_INCOMPLETE_DATA_MSG = "Incomplete data";

        #endregion


    }
}
