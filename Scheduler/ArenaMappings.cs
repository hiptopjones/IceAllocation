using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class ArenaMappings
    {
        // Used for mapping from Ravens schedules to a common arena identifier
        public static readonly Dictionary<string, Arena> RavensArenaMap = new Dictionary<string, Arena>()
        {
            { "Igloo", Arena.Igloo },
            { "Garage", Arena.Garage },
            { "Pond", Arena.Pond },
            { "Coliseum", Arena.Coliseum },
            { "Forum", Arena.Forum },
            { "Gardens", Arena.Gardens },
            { "Silver", Arena.Silver },
            { "Stadium", Arena.Stadium },
            { "Oval South", Arena.OvalSouth },
            { "Oval North", Arena.OvalNorth }
        };

        // Used for mapping from PCAHA schedules to a common arena identifier
        public static readonly Dictionary<string, Arena> PcahaArenaMap = new Dictionary<string, Arena>()
        {
            { "Abbotsford Centre Ice 1 Blue", Arena.AbbotsfordCentreIceBlue },
            { "Bellingham Sportsplex", Arena.BellinghamSportsplex },
            { "Burnaby Lake Arena", Arena.BurnabyLake },
            { "Chilliwack Twin Rinks 2", Arena.ChilliwackTwin2 },
            { "Cloverdale Arena", Arena.Cloverdale },
            { "Coquitlam Main", Arena.CoquitlamMain },
            { "Coquitlam Recreation", Arena.CoquitlamRec },
            { "George Preston Recreation Centre", Arena.GeorgePreston },
            { "Harry Jerome Rec Centre", Arena.HarryJerome },
            { "Hillcrest Centre Arena", Arena.Hillcrest },
            { "Ice Sports North Shore 1 Red", Arena.IceSportsNorthShoreRed },
            { "Ice Sports North Shore 2 Blue", Arena.IceSportsNorthShoreBlue },
            { "Ice Sports North Shore 3 Green", Arena.IceSportsNorthShoreGreen },
            { "Kensington Arena", Arena.Kensington },
            { "Killarney Community Centre", Arena.Killarney },
            { "Langley Sportsplex 1", Arena.LangleySportsplex1 },
            { "Langley Sportsplex 2", Arena.LangleySportsplex2 },
            { "Langley Twin Rinks 1", Arena.LangleyTwin1 },
            { "MSA Arena - Abbotsford", Arena.AbbotsfordMSA },
            { "Matsqui Recreation Centre", Arena.AbbotsfordMRC },
            { "Minoru Arena Silver Spectrum", Arena.MinoruSilver },
            { "Newton Arena", Arena.Newton },
            { "North Shore Winter Club", Arena.NorthShoreWinterClub },
            { "North Surrey Arena 2", Arena.NorthSurrey2 },
            { "PNE Agrodome", Arena.Agrodome },
            { "Pitt Meadows Arena Volkswagen Blue", Arena.PittMeadowsBlue },
            { "Planet Ice Coquitlam 1 Mars", Arena.PlanetIceCoquitlam1 },
            { "Planet Ice Coquitlam 2 Pluto", Arena.PlanetIceCoquitlam2 },
            { "Planet Ice Coquitlam 3 Venus", Arena.PlanetIceCoquitlam3 },
            { "Planet Ice Coquitlam 4 Saturn", Arena.PlanetIceCoquitlam4 },
            { "Planet Ice Delta Canadian", Arena.PlanetIceDeltaCanadian },
            { "Planet Ice Maple Ridge 1 Cam Neely", Arena.PlanetIceMapleRidge1 },
            { "Planet Ice Maple Ridge 2", Arena.PlanetIceMapleRidge2 },
            { "Port Coquitlam Blue", Arena.PocoBlue },
            { "Port Coquitlam Green", Arena.PocoGreen },
            { "Port Moody 2", Arena.PortMoody2 },
            { "Richmond Ice Centre Coliseum", Arena.Coliseum },
            { "Richmond Ice Centre Forum", Arena.Forum },
            { "Richmond Ice Centre Garage", Arena.Garage },
            { "Richmond Ice Centre Igloo", Arena.Igloo },
            { "Richmond Ice Centre Pond", Arena.Pond },
            { "Richmond Olympic Oval North", Arena.OvalNorth },
            { "Richmond Olympic Oval South", Arena.OvalSouth },
            { "Sunset Arena", Arena.Sunset },
            { "Surrey Sport & Leisure Centre Arena 1 Blue", Arena.SurreyLeisure1 },
            { "Surrey Sport & Leisure Centre Arena 2 Purple", Arena.SurreyLeisure2 },
            { "Surrey Sport & Leisure Centre Arena 3 Green", Arena.SurreyLeisure3 },
            { "Tilbury Ice", Arena.Tilbury },
            { "West Vancouver Ice Arena", Arena.WestVan }
        };
    }
}
