using System;
using System.Collections.Generic;
using System.Linq;
using MiningWithNautiloids.Model.Pearl;
using MiningWithNautiloids.Models;
using MiningWithNautiloids.Nautiloids;
using Newtonsoft.Json.Linq;

namespace MiningWithNautiloids
{
    class Program
    {
        public static void Main(string[] args)
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var root = JObject.Parse(line);
                IEnumerable<Nautiloid> nautiloids = InitializeWorkers(root);
                IList<string> steps = new List<string>();
                foreach (var nautiloid in nautiloids)
                {
                    string step = nautiloid.ComputeStep();
                    if (!step.Equals(String.Empty))
                    {
                        steps.Add(step);
                    }
                }
                // Console.WriteLine(root);
                string json_output = "{" + String.Join(',', steps) + "}";
                Console.Out.WriteLine(json_output);
            }
        }

        public static IList<Nautiloid> InitializeWorkers(JObject root)
        {
            IList<Nautiloid> workers = new List<Nautiloid>();
            foreach (var worker in root["workers"])
            {
                Nautiloid nautiloid = null;
                string flavor = (string)worker["flavor"];
                var desk = InitializeDesk(worker["desk"]);
                if (flavor == "General")
                {
                    nautiloid = new GeneralNautiloid((int)worker["id"], new List<Nautiloid>());
                }
                else if (flavor == "Matrix")
                {
                    nautiloid = new MatrixNautiloid((int)worker["id"], new List<Nautiloid>());
                }
                else if (flavor == "Vector")
                {
                    nautiloid = new VectorNautiloid((int)worker["id"], new List<Nautiloid>());
                }

                if(nautiloid != null)
                {
                    nautiloid.AddToDesk(desk);
                    workers.Add(nautiloid);
                }
            }

            foreach(var neighbor in root["neighbor_map"])
            {
                int neighborId1 = (int) neighbor[0];
                int neighborId2 = (int) neighbor[1];
                Nautiloid nautiloid1 = workers.Where(worker => worker.Id == neighborId1).First();
                Nautiloid nautiloid2 = workers.Where(worker => worker.Id == neighborId2).First();
                nautiloid1.AddNeighbor(nautiloid2);
                nautiloid2.AddNeighbor(nautiloid1);
            }

            return workers;
        }

        public static List<Pearl> InitializeDesk(JToken desk)
        {
            List<Pearl> deskOfPearls = new List<Pearl>();
            foreach(var pearl in desk)
            {
                deskOfPearls.Add(InitializePearl(pearl));
            }
            return deskOfPearls;
        }

        public static Pearl InitializePearl(JToken pearl)
        {
            var layers = InitializeLayers(pearl["layers"]);
            var id = (long) pearl["id"];
            return new Pearl(id, layers);
        }

        public static IDictionary<Color, int> InitializeLayers(JToken layers)
        {
            int red = 0; int blue = 0; int green = 0;
            foreach(var layer in layers)
            {
                var color = (string)layer["color"];
                if(color.Equals("Red", StringComparison.CurrentCultureIgnoreCase))
                {
                    red = (int)layer["thickness"];
                }
                if (color.Equals("Green", StringComparison.CurrentCultureIgnoreCase))
                {
                    green = (int)layer["thickness"];
                }
                if (color.Equals("Blue", StringComparison.CurrentCultureIgnoreCase))
                {
                    blue = (int)layer["thickness"];
                }
            }

            return new Dictionary<Color, int>()
            {
                {Color.Red, red },
                {Color.Blue, blue },
                {Color.Green, green }
            };
        }
    }
}
