using System.Collections.Generic;

namespace MiningWithNautiloids.Nautiloids
{
    public class GeneralNautiloid : Nautiloid
    {
        public GeneralNautiloid(int id, IEnumerable<Nautiloid> neighbors)
         : base(id, neighbors, 1, 1, 1)
        {
        }
    }
}