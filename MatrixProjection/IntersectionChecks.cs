using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    class IntersectionChecks
    {
        public static Vector3 LinePlane(Line line, Plane plane)
        {
            Vector3 intersection = Vector3.Zero;

            Vector3 lineVector = line.end - line.pos;

            Vector3 diff = line.pos - plane.pos;
            var prod1 = Vector3.Dot(diff, plane.normal);
            var prod2 = Vector3.Dot(lineVector, plane.normal);
            var prod3 = prod1 / prod2;
            intersection = line.pos - lineVector * prod3;

            return intersection;
        }        
    }
}
