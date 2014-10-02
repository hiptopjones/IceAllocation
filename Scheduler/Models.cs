using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    enum Division
    {
        Unknown,
        Tyke,
        Novice,
        Atom,
        Peewee,
        Bantam,
        Midget,
        Juvenile
    }

    enum Tier
    {
        Unknown,
        A,
        A1,
        A2,
        C1,
        C2,
        C3,
    }

    class Team
    {
        public Division Division
        {
            get;
            set;
        }
        
        public Tier Tier
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return string.Format("{0} {1}", this.Division, this.Tier);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    enum IceType
    {
        Available,
        Practice,
        Skills,
        Accelerator,
        HomeGame,
        AwayGame,
        Other,
    }

}
