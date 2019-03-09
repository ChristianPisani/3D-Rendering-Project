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
        public static Vector3? LinePlane(Line line, Plane plane)
        {
            Vector3 intersection = Vector3.Zero;

            Vector3 lineVector = line.end - line.pos;

            Vector3 diff = line.pos - plane.pos;
            var prod1 = Vector3.Dot(diff, plane.normal);
            var prod2 = Vector3.Dot(lineVector, plane.normal);

            if(prod2 == 0)
            {
                return null;
            }

            var prod3 = prod1 / prod2;
            intersection = line.pos - lineVector * prod3;


            Vector3 transformedNormal = Vector3.Cross(plane.normal, new Vector3(0, -1, 0));
            transformedNormal.Normalize();
            Vector3 offset = plane.pos + transformedNormal * (plane.size.X / 2);
            if (Vector3.Dot(intersection - plane.pos - offset, transformedNormal) > 0 ||
                Vector3.Dot(intersection - plane.pos + offset, -transformedNormal) > 0)
            {
                return null;
            }

            transformedNormal = Vector3.Cross(plane.normal, new Vector3(-1, 0, 0));
            transformedNormal.Normalize();
            offset = plane.pos + transformedNormal * (plane.size.Z / 2); ;
            if (Vector3.Dot(intersection - plane.pos + offset, transformedNormal) > 0 ||
                Vector3.Dot(intersection - plane.pos - offset, transformedNormal) > 0)
            {
                return null;
            }

            if ((line.pos - intersection).Length() > lineVector.Length() ||
                (line.end - intersection).Length() > lineVector.Length())
            {
                return null;
            }

            if(float.IsNegativeInfinity(intersection.X))
            {
                return null;
            }

            return intersection;
        }

        public static Vector3? LineCube(Line line, Cube cube)
        {
            Vector3? intersection = null;

            Plane plane = new Plane(cube.pos + new Vector3(cube.size.X / 2, 0, 0), new Vector2(cube.size.X, cube.size.Y), 1);



            return intersection;
        }
    }
}
