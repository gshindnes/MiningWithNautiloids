using System.Collections.Generic;

namespace MiningWithNautiloids.Nautiloids
{
    public class MatrixNautiloid : Nautiloid
    {
        public MatrixNautiloid(int id, IEnumerable<Nautiloid> neighbors)
         : base(id, neighbors, 1, 2, 10)
        {
        }
    }
}