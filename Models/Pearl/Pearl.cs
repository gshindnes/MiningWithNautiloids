using System.Collections.Generic;
using MiningWithNautiloids.Models;
using MiningWithNautiloids.Models.Pearl;
using Priority_Queue;
using System.Linq;
using System;

namespace MiningWithNautiloids.Model.Pearl
{
    public class Pearl : FastPriorityQueueNode
    {
        public long Id;
        public IDictionary<Color, int> Layers;

        public Pearl(long id, IEnumerable<PearlAttributes> layers)
        {
            Id = id;
            foreach(var layer in layers)
            {
                Layers[layer.Color] = layer.Thickness;
            }
        }

        public Pearl(long id, IDictionary<Color, int> layers)
        {
            Id = id;
            Layers = layers;
        }

        public void NomAtLayer(Color color, int amount)
        {
            Layers[color] = Math.Max(Layers[color] - amount, 0);
        }

        public int Thickness()
        {
            return Layers.Values.Sum();
        }

        public Color MinThickness()
        {
            return Layers.Where(layer => layer.Value != 0).OrderBy(layer => layer.Value).First().Key;
        }
    }
}
