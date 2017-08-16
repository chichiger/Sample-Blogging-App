using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stateful1
{
    using System.Runtime.Serialization;
    [DataContract]
    class UploadKey : IComparable<UploadKey>, IEquatable<UploadKey>
    {
        [DataMember]
        private string username;

        public string Username { get { return this.username; }}

        [DataMember]
        private DateTime timeStamp;

        public DateTime TimeStamp { get { return this.timeStamp; } }

        public UploadKey(string username, DateTime timeStamp)
        {
            this.username = username;
            this.timeStamp = timeStamp;
        }

        public new string ToString()
        {
            return this.username + " " + this.timeStamp;
        }


        public int CompareTo(UploadKey other)
        {
            if (this.timeStamp.CompareTo(other.timeStamp) > 0)
            {
                return 1;
            }

            else if (this.timeStamp.CompareTo(other.timeStamp) == 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }

        public bool Equals(UploadKey other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.username == null)
            {
                return other.username == null;
            }
            else if (this.username.Equals(other.username))
            {
                return this.timeStamp.Equals(other.timeStamp);
            }
            else
            {
                return false;
            }


        }
    }
}
