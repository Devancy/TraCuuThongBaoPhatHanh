using System;

namespace TraCuuThongBaoPhatHanh_v2
{
    public class InvalidResponseJsonException : Exception
    {
        public InvalidResponseJsonException() : base()
        {

        }

        public InvalidResponseJsonException(string message) : base($"Invalid_response_json_format: {message}")
        {

        }
    }
}
