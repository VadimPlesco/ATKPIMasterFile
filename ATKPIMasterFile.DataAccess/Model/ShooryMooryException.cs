using System;

namespace ATKPIMasterFile.DataAccess.Model
{
    public class ShooryMooryException : Exception
    {
        private readonly ResponseCode _responseCode;
        public ResponseCode ResponseCode
        {
            get { return _responseCode; }
        }

        public ShooryMooryException(ResponseCode code, string description = null)
            : base("ShooryMooryException: " + code + (description == null ? "" : ("; Description: " + description)))
        {
            _responseCode = code;
        }

        public string FindedUserAddress { get; set; }

        public string FindedInterestAddress { get; set; }
    }

    public class ContactsLimitExceededException : ShooryMooryException
    {
        public TimeSpan TimeToRenewal { get; private set; }

        public ContactsLimitExceededException(ResponseCode code, TimeSpan timeToRenewal, string description = null) : base(code, description)
        {
            TimeToRenewal = timeToRenewal;
        }
    }

    public class MessageLimitPerContactExceededException : ShooryMooryException
    {
        public MessageLimitPerContactExceededException(ResponseCode code, string description = null)
            : base(code, description)
        {
        }
    }
}