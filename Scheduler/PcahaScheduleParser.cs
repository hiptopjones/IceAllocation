using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class PcahaScheduleParser
    {
        private string path;

        public PcahaScheduleParser(string path)
        {
            this.path = path;
        }

        // Parses the input schedule in a CSV format
        public List<TeamEvent> Parse()
        {
            List<TeamEvent> schedule = new List<TeamEvent>();

            string[] lines = File.ReadAllLines(this.path);
            
            // First line is a header
            bool isFirstLine = true;

            foreach (var line in lines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                // Game #,Day,Date,Start,End,Home Team,Away Team,Arena
                string[] fields = line.Split(new[] { ',' });
                if (fields.Length != 8)
                {
                    Console.WriteLine("Skipping non-game line: [{0}]", line);
                    continue;
                }

                string gameNumber = fields[0];
                string day = fields[1];
                string date = fields[2];
                string startTime = fields[3];
                string endTime = fields[4];
                string homeTeam = fields[5];
                string awayTeam = fields[6];
                string arena = fields[7];

                if (string.IsNullOrEmpty(date))
                {
                    // Conflict game, not handled here
                    Console.WriteLine("Skipping confict game: [{0}]", line);
                    continue;
                }

                if (!homeTeam.Contains("Richmond") && !awayTeam.Contains("Richmond"))
                {
                    // Don't care about this game if none of our teams are in it
                    Console.WriteLine("Skipping non-Richmond game: [{0}]", line);
                    continue;
                }

                if (homeTeam.Contains("Richmond") && awayTeam.Contains("Richmond"))
                {
                    Console.WriteLine("Oops, games where two Richmond teams play each other need to be handled.");
                    throw new Exception();
                }

                TeamEvent teamEvent = new TeamEvent
                {
                    StartTime = DateTime.Parse(date + " " + startTime),
                    EndTime = DateTime.Parse(date + " " + endTime),
                    Arena = ArenaMappings.PcahaArenaMap[arena],
                    Type = homeTeam.Contains("Richmond") ? IceType.HomeGame : IceType.AwayGame,
                    Team = ParseRavensTeamName(homeTeam.Contains("Richmond") ? homeTeam : awayTeam),
                    OtherInfo = ShortenAwayTeamName(homeTeam.Contains("Richmond") ? awayTeam : homeTeam)
                };

                schedule.Add(teamEvent);
            }

            return schedule;
        }

        private string RemoveGirls(string teamName)
        {
            List<string> parts = new List<string>(teamName.Split(new[] { ' ' }));
            parts.RemoveAll(p => p == "Girls" || p == "Female");

            return string.Join(" ", parts);
        }

        private string RemoveDivision(string teamName)
        {
            string[] divisions = Enum.GetNames(typeof(Division));

            List<string> parts = new List<string>(teamName.Split(new[] { ' ' }));
            parts.RemoveAll(p => divisions.Contains(p));

            return string.Join(" ", parts);
        }

        private string RemoveRichmond(string teamName)
        {
            List<string> parts = new List<string>(teamName.Split(new[] { ' ' }));
            parts.RemoveAll(p => p == "Richmond");

            return string.Join(" ", parts);
        }

        private Team ParseRavensTeamName(string teamName)
        {
            teamName = RemoveRichmond(teamName);
            teamName = RemoveGirls(teamName);

            Division division = Division.Unknown;
            Tier tier = Tier.Unknown;

            string trimmedPart = teamName.Trim();

            // Handle division
            if (trimmedPart.StartsWith("Tyke"))
            {
                division = Division.Tyke;
            }
            else if (trimmedPart.StartsWith("Novice"))
            {
                division = Division.Novice;
            }
            else if (trimmedPart.StartsWith("Atom"))
            {
                division = Division.Atom;
            }
            else if (trimmedPart.StartsWith("Peewee"))
            {
                division = Division.Peewee;
            }
            else if (trimmedPart.StartsWith("Bantam"))
            {
                division = Division.Bantam;
            }
            else if (trimmedPart.StartsWith("Midget"))
            {
                division = Division.Midget;
            }
            else if (trimmedPart.StartsWith("Juvenile"))
            {
                division = Division.Juvenile;
            }
            else
            {
                throw new ApplicationException(string.Format("unknown division: {0}", trimmedPart));
            }

            // Handle tier
            if (trimmedPart.EndsWith("A"))
            {
                tier = Tier.A;
            }
            else if (trimmedPart.EndsWith("A1"))
            {
                // Unless there are actually multiple tiers in this division, treat A1 as A
                if (division == Division.Peewee)
                {
                    tier = Tier.A1;
                }
                else
                {
                    tier = Tier.A;
                }
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
            else
            {
                throw new ApplicationException(string.Format("unknown tier: {0}", trimmedPart));
            }

            Team team = new Team();
            team.Division = division;
            team.Tier = tier;

            return team;
        }

        private string ShortenAwayTeamName(string teamName)
        {
            teamName = RemoveGirls(teamName);
            teamName = RemoveDivision(teamName);

            return teamName;
        }
    }
}
