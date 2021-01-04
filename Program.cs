using System;
using System.IO;

namespace lab
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using var sr = new StreamReader("in.txt");
                string line;
                var numberLine = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;

                    Console.WriteLine($"{numberLine++}) {line}");

                    BuilderPNF builder;

                    try
                    {
                        builder = new BuilderPNF(line);
                    }
                    catch(ArgumentException)
                    {
                        Console.WriteLine("Incorrect input\n");
                        continue;
                    }

                    builder.EventBuildСonjunct += EventBuildConjunct;
                    builder.EventBuildDisjoint += EventBuildDisjoint;

                    var result = builder.BuildTruthTable();
                        
                    Console.WriteLine("\nBuild truth table");

                    foreach (var let in builder.ListVariables)
                        Console.Write(@let);

                    Console.Write("\tF" + Environment.NewLine);

                    foreach (var item in result)
                        Console.WriteLine(item.Key + "\t" + item.Value);

                    Console.WriteLine($"PDNF\n--------------\n");
                    var pdnf = builder.BuildPDNF() ?? "Not exists";
                    Console.WriteLine("\n" + pdnf + "\n--------------");

                    Console.WriteLine($"\nPKNF\n--------------");
                    var pknf = builder.BuildPKNF() ?? "Not exists";
                    Console.WriteLine("\n" + pknf + "\n--------------");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found");
            }
        }

        static void EventBuildConjunct(object sender, BuildNormalFormEventArgs args)
        {
            Console.WriteLine("Build conjunct");

            foreach (var item in args.ListItems)
                Console.WriteLine($"Addition {item}");
        }

        static void EventBuildDisjoint(object sender, BuildNormalFormEventArgs args)
        {
            Console.WriteLine("Build disjoint");

            foreach (var item in args.ListItems)
                Console.WriteLine($"Addition {item}");
        }
    }
}
