using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scheduler
{
    // Writes out a format that can be uploaded into TeamPages
    // One file per team.
    class TeamPagesWriter
    {
        public static void Write(List<TeamEvent> times)
        {
            Dictionary<string, List<TeamEvent>> timesByTeam = new Dictionary<string, List<TeamEvent>>();

            foreach (TeamEvent ice in times)
            {
                Console.WriteLine(ice.ToString());

                string name = (ice.Team == null ? "Available" : ice.Team.Name);

                List<TeamEvent> teamTimes = null;
                if (!timesByTeam.TryGetValue(name, out teamTimes))
                {
                    teamTimes = new List<TeamEvent>();
                    timesByTeam[name] = teamTimes;
                }

                teamTimes.Add(ice);
            }

            foreach (string teamName in timesByTeam.Keys)
            {
                using (StreamWriter writer = new StreamWriter(string.Format("{0}_schedule.csv", teamName)))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Type");
                    builder.Append(",Date");
                    builder.Append(",Start");
                    builder.Append(",Finish");
                    builder.Append(",Opponent");
                    builder.Append(",Home");
                    builder.Append(",Location");
                    writer.WriteLine(builder.ToString());

                    string type = null;
                    string opponent = null;

                    foreach (TeamEvent ice in timesByTeam[teamName])
                    {
                        //if (ice.Type == IceType.HomeGame)
                        //{
                        //    continue;
                        //}

                        builder = new StringBuilder();

                        switch (ice.Type)
                        {
                            case IceType.Practice:
                                type = "Practice";
                                opponent = "Ignore";
                                break;

                            case IceType.Skills:
                                type = "Skills Development";
                                opponent = "Skills Session";
                                break;

                            case IceType.HomeGame:
                            case IceType.AwayGame:
                                type = "League Game";
                                opponent = ice.OtherInfo;
                                break;

                            case IceType.Accelerator:
                                // Ignore
                                continue;

                            case IceType.Other:
                            default:
                                type = "Other";
                                opponent = ice.OtherInfo;
                                break;
                        }

                        // Type
                        builder.Append(type);

                        // Date
                        builder.Append(string.Format(",{0}", ice.StartTime.ToString("dd-MMM-yyyy")));
                        
                        // Start Time
                        builder.Append(string.Format(",{0}", ice.StartTime.ToString("hh:mm tt")));

                        // End Time
                        builder.Append(string.Format(",{0}", ice.EndTime.ToString("hh:mm tt")));

                        // Opponent
                        builder.Append(string.Format(",{0}", opponent));

                        // Home
                        if (ice.Type == IceType.HomeGame)
                        {
                            builder.Append(",vs.");
                        }
                        else if (ice.Type == IceType.AwayGame)
                        {
                            builder.Append(",@");
                        }
                        else
                        {
                            builder.Append(",");
                        }

                        // Location
                        builder.Append(string.Format(",{0}", ArenaMapping[ice.Arena]));

                        writer.WriteLine(builder.ToString());
                    }
                }
            }
        }

        private static Dictionary<Arena, string> ArenaMapping = new Dictionary<Arena, string>() 
        {
            { Arena.AbbotsfordCentreIceBlue, "Abbotsford Centre Ice - 1 (Blue)" },  
            { Arena.AbbotsfordMRC, "Abbotsford MRC" },  
            { Arena.AbbotsfordMSA, "Abbotsford MSA" },  
            { Arena.BellinghamSportsplex, "Bellingham Sportsplex" },  
            { Arena.BurnabyLake, "Burnaby Lake Arena" },  
            { Arena.Kensington, "Burnaby Kensington Arena" },  
            { Arena.ChilliwackTwin2, "Chilliwack Twin Rinks - #2" },  
            { Arena.Cloverdale, "Cloverdale Arena" },  
            { Arena.CoquitlamMain, "Coquitlam 1 Main" },  
            { Arena.CoquitlamRec, "Coquitlam 2 Poirier Sport and Rec. Centre" },  
            { Arena.GeorgePreston, "George Preston Recreation Centre (Langley Civic Centre)" },  
            { Arena.PlanetIceDeltaCanadian, "Great Pacific Forum - Canadian" },  
            { Arena.HarryJerome, "Harry Jerome / Lonsdale Rec Centre" },  
            { Arena.Hillcrest, "Hillcrest Arena / Vancouver Olympic Centre (Riley Park)" },  
            { Arena.IceSportsNorthShoreRed, "Ice Sports North Van - #1 Red" },  
            { Arena.IceSportsNorthShoreBlue, "Ice Sports North Van - #2 Blue" },  
            { Arena.IceSportsNorthShoreGreen, "Ice Sports North Van - #3 Green" },  
            { Arena.Killarney, "Killarney Arena" },  
            { Arena.LangleySportsplex1, "Langley Sportsplex - 1 Green" },  
            { Arena.LangleySportsplex2, "Langley Sportsplex - 2 Blue" },  
            { Arena.LangleyTwin1, "Langley Twin" },  
            { Arena.MinoruSilver, "Minoru - Silver" },  
            { Arena.Newton, "Newton Community Centre" },  
            { Arena.NorthShoreWinterClub, "North Shore Winter club" },  
            { Arena.NorthSurrey2, "North Surrey - #2" },  
            { Arena.Agrodome, "Pacific National Exhibition - Agrodome" }, 
            { Arena.PittMeadowsBlue, "Pitt Meadows Arenas - Volkswagen Blue" }, 
            { Arena.PlanetIceCoquitlam1, "Planet Ice Coquitlam - Mars-1" }, 
            { Arena.PlanetIceCoquitlam2, "Planet Ice Coquitlam - Pluto-2" }, 
            { Arena.PlanetIceCoquitlam3, "Planet Ice Coquitlam - Venus-3" }, 
            { Arena.PlanetIceCoquitlam4, "Planet Ice Coquitlam - Saturn-4" }, 
            { Arena.PlanetIceMapleRidge1, "Planet Ice Ridge Meadows - Cam Neely #1" }, 
            { Arena.PlanetIceMapleRidge2, "Planet Ice Ridge Meadows - Ice #2" }, 
            { Arena.PocoBlue, "Port Coquitlam - Blue (new)" }, 
            { Arena.PocoGreen, "Port Coquitlam - Green (old)" },
            { Arena.PortMoody2, "Port Moody - 2 (new)" }, 
            { Arena.Coliseum, "Richmond Ice Centre - Coliseum" }, 
            { Arena.Forum, "Richmond Ice Centre - Forum" }, 
            { Arena.Garage, "Richmond Ice Centre - Garage" }, 
            { Arena.Gardens, "Richmond Ice Centre - Garden" }, 
            { Arena.Igloo, "Richmond Ice Centre - Igloo" }, 
            { Arena.Pond, "Richmond Ice Centre - Pond" }, 
            { Arena.OvalNorth, "Richmond Oympic Oval - North" }, 
            { Arena.OvalSouth, "Richmond Oympic Oval - South" }, 
            { Arena.Sunset, "Sunset Arena" }, 
            { Arena.SurreyLeisure1, "Surrey Sports & Leisure Centre (Fleetwood) - #1" }, 
            { Arena.SurreyLeisure2, "Surrey Sports & Leisure Centre (Fleetwood) - #2" }, 
            { Arena.SurreyLeisure3, "Surrey Sports & Leisure Centre (Fleetwood) - #3" }, 
            { Arena.Tilbury, "Tilbury Ice" }, 
            { Arena.WestVan, "West Vancouver Rec Centre" }
        };
    }
}
