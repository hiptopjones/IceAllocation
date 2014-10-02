using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scheduler
{
    class RavensScheduleParser
    {
        private string path;

        public RavensScheduleParser(string path)
        {
            this.path = path;
        }

        private string NextField(string input, ref int startIndex)
        {
            string output = null;
            int endIndex = startIndex;

            if (startIndex < input.Length)
            {
                if (input[startIndex] == '"')
                {
                    // move past opening quote
                    startIndex++;

                    endIndex = input.IndexOf('"', startIndex);
                    if (endIndex < 0)
                    {
                        throw new ApplicationException("expected closing quote, got end of line");
                    }

                    output = input.Substring(startIndex, endIndex - startIndex);

                    // move past closing quote
                    startIndex = endIndex + 1;

                    // TODO: assert that we're at a comma, or at the end of the line
                }
                else
                {
                    endIndex = input.IndexOf(',', startIndex);
                    if (endIndex < 0)
                    {
                        endIndex = input.Length;
                    }

                    output = input.Substring(startIndex, endIndex - startIndex);

                    startIndex = endIndex;
                }

                // move past comma
                startIndex++;
            }

            //Console.WriteLine("Field: '{0}'", output);

            return output;
        }

        // Parses the input schedule in a CSV format
        public List<TeamEvent> Parse()
        {
            // These are temporary lists so that we can build up slots as we run line by line
            List<string>[] stringAccumulators = new List<string>[7];
            List<TeamEvent>[] iceAccumulators = new List<TeamEvent>[7];

            // Expected that this will be set properly below
            DateTime scheduleStart = DateTime.Today;

            bool parseDone = false;

            string[] lines = File.ReadAllLines(this.path);
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (parseDone)
                {
                    break;
                }

                string line = lines[i];
                int currentIndex = 0;

                if (i == 0)
                {
                    // If this is the first line, expect a date in the first field
                    string field = NextField(line, ref currentIndex);
                    if (field == null)
                    {
                        throw new ApplicationException("unexpected null field in first line");
                    }

                    scheduleStart = ExtractScheduleDate(field);
                }
                else if (i >= 4)
                {
                    // We are inside of the data area

                    // First field is ignored
                    NextField(line, ref currentIndex);

                    // Second through Eighth field are days of the week
                    for (int j = 0; j < 7; j++)
                    {
                        // Ensure the accumulator lists are not null
                        if (stringAccumulators[j] == null)
                        {
                            stringAccumulators[j] = new List<string>();
                            iceAccumulators[j] = new List<TeamEvent>();
                        }

                        List<string> accumulator = stringAccumulators[j];

                        string field = NextField(line, ref currentIndex);

                        // Use the bottom row of day-of-week names as a sentinel to know when to exit
                        if (field == "Monday")
                        {
                            parseDone = true;
                            break;
                        }

                        // If the field is actually missing, synthesize an empty one to make subsequent code work
                        if (field == null)
                        {
                            field = string.Empty;
                        }

                        field = field.Trim();

                        // Never add empty fields, unless the list already has non-empty fields
                        if (field != string.Empty || accumulator.Count > 0)
                        {
                            accumulator.Add(field);
                            
                            // If we have full information, create relevant IceSlots
                            if (accumulator.Count == 4)
                            {
                                TeamEvent[] iceTimes = this.BuildIceTimes(scheduleStart, j, accumulator);
                                iceAccumulators[j].AddRange(iceTimes);

                                accumulator.Clear();
                            }
                        }
                    }
                }
            }

            // Now transfer to a linear list
            List<TeamEvent> schedule = new List<TeamEvent>();

            foreach (List<TeamEvent> accumulator in iceAccumulators)
            {
                schedule.AddRange(accumulator);
            }

            return schedule;
        }

        private DateTime ExtractScheduleDate(string field)
        {
            string[] parts = field.Split(new char[] { '-' });

            if (parts.Length != 3)
            {
                throw new ApplicationException(string.Format("expected three parts in first line, got {0}: '{1}'", parts.Length));
            }

            try
            {
                // Pull the date from the second part, that's our starting date for the schedule
                return DateTime.Parse(parts[1]);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("unable to parse date field: {0}", parts[1]), ex);
            }
        }

        private TeamEvent[] BuildIceTimes(DateTime scheduleStart, int dayOffset, List<string> accumulator)
        {
            List<TeamEvent> schedule = new List<TeamEvent>();

            // Accumulator should have:
            //  * team (for game or practice) or name (for skills, etc)
            //  * time range
            //  * rink
            //  * opponent (for game) or team (for skills, etc.)
            // Last slot is optional, everything else is required
            
            string slot1 = accumulator[0];
            string slot2 = accumulator[1];
            string slot3 = accumulator[2];
            string slot4 = accumulator[3];

            IceType type = IceType.Practice;
            string otherInfo = string.Empty;

            Team[] teams = this.ParseTeams(slot1);
            if (teams.Length == 0)
            {
                switch (slot1)
                {
                    case "Development":
                        type = IceType.Skills;
                        break;

                    case "Skill Accelerator":
                        type = IceType.Accelerator;
                        break;

                    case "Available":
                        type = IceType.Available;
                        break;

                    default:
                        type = IceType.Other;
                        otherInfo = slot4;
                        break;
                }

                if (type == IceType.Skills || type == IceType.Accelerator)
                {
                    // The team names should be in the fourth line
                    teams = this.ParseTeams(slot4);
                    if (teams == null)
                    {
                        throw new ApplicationException(string.Format("skills should have teams in fourth slot: {0}", slot4));
                    }
                }
            }
            else
            {
                if (slot4.StartsWith("v.") || slot4.StartsWith("vs."))
                {
                    type = IceType.HomeGame;
                    otherInfo = slot4.Substring(slot4.IndexOf('.') + 1).Trim();
                }
                else if (slot4.StartsWith("@"))
                {
                    type = IceType.AwayGame;
                    otherInfo = slot4.Substring(slot4.IndexOf('@') + 1).Trim();
                }
            }

            if (string.IsNullOrEmpty(slot2))
            {
                Console.WriteLine("Unrecognized block: {0} {1} {2} {3}", slot1, slot2, slot3, slot4);
            }
            else
            {
                DateTime[] range = this.ParseRange(scheduleStart, dayOffset, slot2);
                Arena arena = ArenaMappings.RavensArenaMap[slot3];

                if (teams.Length > 0)
                {
                    foreach (Team team in teams)
                    {
                        TeamEvent ice = new TeamEvent();
                        ice.Type = type;
                        ice.Team = team;
                        ice.StartTime = range[0];
                        ice.EndTime = range[1];
                        ice.Arena = arena;
                        ice.OtherInfo = otherInfo;

                        schedule.Add(ice);
                    }
                }
                else
                {
                    TeamEvent ice = new TeamEvent();
                    ice.Type = type;
                    ice.StartTime = range[0];
                    ice.EndTime = range[1];
                    ice.Arena = arena;
                    ice.OtherInfo = otherInfo;

                    schedule.Add(ice);
                }
            }

            return schedule.ToArray();
        }

        private DateTime[] ParseRange(DateTime scheduleStart, int dayOffset, string str)
        {
            string[] parts = str.Split(new char[] { '-' });

            if (parts.Length != 2)
            {
                throw new ApplicationException(string.Format("unexpected range formatting: {0}", str));
            }

            DateTime startTime = DateTime.Parse(parts[0]);
            DateTime endTime = DateTime.Parse(parts[1]);

            // If they are more than 12 hours apart, then startTime needs to be moved to PM
            if ((endTime - startTime) > TimeSpan.FromHours(12))
            {
                startTime += TimeSpan.FromHours(12);
            }

            // Adjust these times for the start of the schedule
            startTime = scheduleStart.Date + (startTime - DateTime.Today) + TimeSpan.FromDays(dayOffset);
            endTime = scheduleStart.Date + (endTime - DateTime.Today) + TimeSpan.FromDays(dayOffset);

            return new DateTime[] { startTime, endTime };
        }

        // Teams must be separated by a delimiter
        private Team[] ParseTeams(string str)
        {
            List<Team> teams = new List<Team>();
            
            string[] parts = str.Split(new char[] { '/', '+' });

            // Scope this out here so that it persists to second part and beyond if necessary
            Division division = Division.Unknown;

            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                Tier tier = Tier.Unknown;

                if (trimmedPart.StartsWith("Tyke"))
                {
                    division = Division.Tyke;
                }
                else if (trimmedPart.StartsWith("Novice"))
                {
                    division = Division.Novice;
                }
                else if (trimmedPart.StartsWith("Atom") || (trimmedPart.StartsWith("A") && trimmedPart.Length > 2 && trimmedPart.Length < 5))
                {
                    // Matches Atom or AC2, but not the A2 in "Peewee A1/A2"
                    division = Division.Atom;
                }
                else if (trimmedPart.StartsWith("Peewee") || (trimmedPart.StartsWith("P") && trimmedPart.Length < 5))
                {
                    // Matches Peewee or PC1
                    division = Division.Peewee;
                }
                else if (trimmedPart.StartsWith("Bantam") || (trimmedPart.StartsWith("B") && trimmedPart.Length < 5))
                {
                    // Matches Bantam or BA
                    division = Division.Bantam;
                }
                else if (trimmedPart.StartsWith("Midget") || (trimmedPart.StartsWith("M") && trimmedPart.Length < 5))
                {
                    // Matches Midget or MC1
                    division = Division.Midget;
                }
                else if (trimmedPart.StartsWith("Juvenile"))
                {
                    division = Division.Juvenile;
                }

                if (trimmedPart.EndsWith("A"))
                {
                    tier = Tier.A;
                }
                else if (trimmedPart.EndsWith("A1"))
                {
                    tier = Tier.A1;
                }
                else if (trimmedPart.EndsWith("A2"))
                {
                    tier = Tier.A2;
                }
                else if (trimmedPart.EndsWith("C1"))
                {
                    tier = Tier.C1;
                }
                else if (trimmedPart.EndsWith("C2"))
                {
                    tier = Tier.C2;
                }
                else if (trimmedPart.EndsWith("C3"))
                {
                    tier = Tier.C3;
                }

                if (division != Division.Unknown)
                {
                    if (tier == Tier.Unknown)
                    {
                        // When a division is specified by itself, it means all teams
                        // TODO: run this based on some configuration data about what teams are present, not hard-coded

                        if (division == Division.Tyke)
                        {
                            // Tyke only has C1, C2
                            this.AddTeam(teams, division, Tier.C1);
                            this.AddTeam(teams, division, Tier.C2);
                        }
                        else if (division == Division.Novice || division == Division.Atom)
                        {
                            // Novice and Atom have C1, C2, C3
                            this.AddTeam(teams, division, Tier.C1);
                            this.AddTeam(teams, division, Tier.C2);
                            this.AddTeam(teams, division, Tier.C3);
                        }
                        else if (division == Division.Peewee)
                        {
                            // Peewee has two rep teams
                            this.AddTeam(teams, division, Tier.A1);
                            this.AddTeam(teams, division, Tier.A2);
                            this.AddTeam(teams, division, Tier.C1);
                        }
                        else if (division == Division.Bantam || division == Division.Midget)
                        {
                            // Bantam/Midget have rep teams
                            this.AddTeam(teams, division, Tier.A);
                            this.AddTeam(teams, division, Tier.C1);
                            this.AddTeam(teams, division, Tier.C2);
                        }
                        else if (division == Division.Juvenile)
                        {
                            // Juvenile has only a C1
                            this.AddTeam(teams, division, Tier.C1);
                        }
                        else
                        {
                            throw new ApplicationException(string.Format("unknown division: {0}", division.ToString()));
                        }
                    }
                    else
                    {
                        this.AddTeam(teams, division, tier);
                    }
                }
            }

            return teams.ToArray();
        }

        private void AddTeam(List<Team> teams, Division division, Tier tier)
        {

            Team team = new Team();
            team.Division = division;
            team.Tier = tier;

            teams.Add(team);
        }
    }
}
