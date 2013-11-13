using System;

namespace AppLimit.CloudComputing.SharpBox.Exceptions
{
    /// <summary>
    /// The SharpBoxException class implements the error code and error
    /// message of a specific sharp box error. Catch this kind of exceptions
    /// to get all sharpbox related error in the application
    /// </summary>
    public class SharpBoxException : Exception
    {
        /// <summary>
        /// This property contains the errorcode of the specific sharpbox error
        /// </summary>
        public SharpBoxErrorCodes ErrorCode { get; private set; }

        /// <summary>
        /// The constructure if the SharpBoxException class. The error code wil
        /// be resolved into an text message automatically
        /// </summary>
        /// <param name="errorCode"></param>
        public SharpBoxException(SharpBoxErrorCodes errorCode)
            : this(errorCode, null)
        {                        
        }

        /// <summary>
        /// The constructure if the SharpBoxException class. The error code wil
        /// be resolved into an text message automatically
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="innerException"></param>
        public SharpBoxException(SharpBoxErrorCodes errorCode, Exception innerException)
            : base(GetErrorMessage(errorCode), innerException)
        {
            ErrorCode = errorCode;            
        }

        private static String GetErrorMessage(SharpBoxErrorCodes errorCode)
        {                                                
            // get the key
            String key = errorCode.ToString();

            // Load the value of string value for Client
            try
            {

                return ErrorMessages.ResourceManager.GetString(key);
            }
            catch (Exception)
            {
                return "n/a";
            }
        }
    }
}
