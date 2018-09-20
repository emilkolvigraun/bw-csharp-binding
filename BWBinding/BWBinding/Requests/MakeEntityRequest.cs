using System;
using System.Collections.Generic;

namespace BWBinding.Requests
{
    public class MakeEntityRequest
    {
        public string contact { private set; get; }
        public string comment { private set; get; }
        public long expiry { private set; get; }
        public long expiryDelta = 0;
        public List<string> revokers { private set; get; }
        public bool leaveOutCreationDate { private set; get; }

        public MakeEntityRequest(string contact, string comment, DateTime expiry, long expiryDelta,
            List<string> revokers, bool leaveOutCreationDate)
        {
            this.contact = contact;
            this.comment = comment;
            this.expiry = expiry.Millisecond;
            this.expiryDelta = expiryDelta;
            this.revokers = revokers;
            this.leaveOutCreationDate = leaveOutCreationDate;
        }

        public DateTime GetDateTime
        {
            get
            {
                return new DateTime(expiry); 
            }
        }

    }
}
