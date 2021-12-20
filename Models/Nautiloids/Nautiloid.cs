using System;
using System.Collections.Generic;
using System.Linq;
using MiningWithNautiloids.Model.Pearl;
using MiningWithNautiloids.Models;
using Priority_Queue;

namespace MiningWithNautiloids.Nautiloids
{
    public abstract class Nautiloid
    {
        public int Id;
        public IEnumerable<Nautiloid> Neighbors;
        public IDictionary<Color, int> ColorNom;
        public FastPriorityQueue<Pearl> DeskOfPearls;
        public int points;

        private readonly Random generator = new Random();
        public Nautiloid(int id, IEnumerable<Nautiloid> neighbors, int r, int g, int b)
        {
            Id = id;
            Neighbors = neighbors;
            ColorNom = new Dictionary<Color, int>() { { Color.Red, r }, { Color.Green, g }, { Color.Blue, b } };
            DeskOfPearls = new FastPriorityQueue<Pearl>(100);
        }

        public void AddToDesk(Pearl pearlToAdd)
        {
            DeskOfPearls.Enqueue(pearlToAdd, Priority(pearlToAdd));
        }

        public void AddToDesk(List<Pearl> pearlsToAdd)
        {
            foreach (var pearl in pearlsToAdd)
            {
                DeskOfPearls.Enqueue(pearl, Priority(pearl));
            }
        }

        public string ComputeStep()
        {
            this.RemoveNoLayeredPearls();
            if(this.DeskOfPearls.Count() == 0)
            {
                return String.Empty;
            }

            var topPearl = this.DeskOfPearls.First;

            if (topPearl.Thickness() == 0)
            {
                foreach (var neighbor in Neighbors)
                {
                    if (neighbor.Id == 0)
                    {
                        return Pass(neighbor);
                    }
                }
                var randomNeighborIndex = generator.Next(Neighbors.Count());
                var randomNeighbor = Neighbors.Skip(randomNeighborIndex - 1).First();
                return Pass(randomNeighbor);
            }
            else
            {
                var color = this.ChooseColorToNom(topPearl);
                var colorThickness = topPearl.Layers[color];
                if (colorThickness <= ColorNom[color])
                {
                    return Nom();
                }
                else
                {
                    foreach (var neighbor in Neighbors)
                    {
                        if (neighbor.DeskOfPearls.Count() < 3)
                        {
                            return Pass(neighbor);
                        }
                    }
                    return Nom();
                }
            }
        }

        public string Pass(Nautiloid nautiloid)
        {
            Pearl pearl = this.DeskOfPearls.Dequeue();
            nautiloid.AddPearlToDesk(pearl);

            return $"\"{this.Id}\":{{\"Pass\":{{\"pearl_id\":{pearl.Id}, \"to_worker\":{nautiloid.Id}}}}}";
        }

        public string Nom()
        {
            Pearl pearl = this.DeskOfPearls.Dequeue();
            Color color = this.ChooseColorToNom(pearl);
            pearl.NomAtLayer(color, ColorNom[color]);
            this.DeskOfPearls.Enqueue(pearl, Priority(pearl));
            return $"\"{this.Id}\":{{\"Nom\":{pearl.Id}}}";
        }

        public void AddPearlToDesk(Pearl pearl)
        {
            DeskOfPearls.Enqueue(pearl, Priority(pearl)); 
        }

        public bool IsGateKeeper()
        {
            return Id == 0;
        }

        public Color ChooseColorToNom(Pearl pearl)
        {
            Color color = Color.Red;
            float prop = 0;
            foreach (var nomCapability in ColorNom)
            {
                var nautiloidColor = nomCapability.Key;
                var nautiloidAmount = nomCapability.Value;
                var currentProp = ((float)nautiloidAmount / (float)pearl.Layers[nautiloidColor]);
                if (currentProp > prop)
                {
                    prop = currentProp;
                    color = nautiloidColor;
                }
            }
            return color;
        }

        public void RemoveNoLayeredPearls()
        {
            if(IsGateKeeper())
            {
                while(this.DeskOfPearls.Count() > 0 && this.DeskOfPearls.First().Thickness() == 0)
                {
                    if (this.DeskOfPearls.First().Thickness() == 0)
                    {
                        this.DeskOfPearls.Dequeue();
                        this.points++;
                    }
                }
            }
            else
            {
                return;
            }
        }

        public void AddNeighbor(Nautiloid neighbor)
        {
            if(Neighbors.Count(nghbr => nghbr.Id == neighbor.Id) == 0)
            {
                Neighbors = Neighbors.Append(neighbor);
            }
        }

        private int Priority(Pearl pearl)
        {
            int priority = 0;
            foreach(var layer in pearl.Layers)
            {
                priority += layer.Value / ColorNom[layer.Key];
            }
            return priority;
        }
    }
}