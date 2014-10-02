using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("Arguments:");
			foreach (string arg in args)
			{
				Console.WriteLine("'{0}'", arg);
			}

            if (args.Length < 1)
            {
                PrintUsage();
                return;
            }

            List<TeamEvent> iceTimes = new List<TeamEvent>();
            for (int i = 0; i < args.Length; i++)
            {
                //RavensScheduleParser parser = new RavensScheduleParser(args[i]);
                PcahaScheduleParser parser = new PcahaScheduleParser(args[i]);
                iceTimes.AddRange(parser.Parse());
            }

            TeamPagesWriter.Write(iceTimes);
            //SportZoneWriter.Write(iceTimes);

            Console.Write("Hit any key to continue...");
            Console.Read();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    Don't be a dumbass!");
        }
    }
}
