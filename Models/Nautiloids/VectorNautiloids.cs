using System.Collections.Generic;

namespace MiningWithNautiloids.Nautiloids
{
    public class VectorNautiloid : Nautiloid
    {

        public VectorNautiloid(int id, IEnumerable<Nautiloid> neighbors)
         : base(id, neighbors, 1, 5, 2)
        {
        }

    }
}