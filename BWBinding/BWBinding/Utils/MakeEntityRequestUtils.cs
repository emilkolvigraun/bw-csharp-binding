using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BWBinding.Requests;

namespace BWBinding.Utils
{
    class MakeEntityRequestUtils
    {
        private string contact;
        private string comment;
        private DateTime expiry;
        private long expiryDelta;
        private List<string> revokers;
        private bool leaveOutCreationDate;

        public MakeEntityRequestUtils()
        {
            this.leaveOutCreationDate = false;
            this.revokers = new List<string>();
        }

        public void Contact(string contact)
        {
           this.contact = contact;
        }

        public void Comment(string comment)
        {
            this.comment = comment;
        }

        public void Expiry(DateTime expiry)
        {
            this.expiry = expiry;
        }

        public void ExpiryDelta(long expiryDelta)
        {
            this.expiryDelta = expiryDelta;
        }

        public void Revokers(List<string> revokers)
        {
            this.revokers = revokers;
        }

        public void LeaveOutCreationDate(bool leaveOutCreationDate)
        {
            this.leaveOutCreationDate = leaveOutCreationDate;
        }

        public void ClearRevokers()
        {
            this.revokers.Clear();
        }

        public void ClearAll()
        {
            this.leaveOutCreationDate = false;
            this.revokers.Clear();
        }

        public MakeEntityRequest Build()
        {
            return new MakeEntityRequest(contact, comment, expiry, expiryDelta, revokers, leaveOutCreationDate);
        }
    }
}
