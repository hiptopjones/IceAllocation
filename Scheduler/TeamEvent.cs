using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class TeamEvent
    {
        public IceType Type
        {
            get;
            set;
        }

        public Team Team
        {
            get;
            set;
        }

        public Arena Arena
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }

        public string OtherInfo
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}",
                (this.Team == null ? "n/a" : this.Team.ToString()),
                this.Type.ToString(),
                this.Arena.ToString(),
                this.StartTime.ToLongDateString(),
                this.StartTime.ToShortTimeString(),
                this.OtherInfo);
        }
    }
}
