using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    class Line : GameObject
    {
        public Vector3 end;

        //Points that represents top of cube, which will be moved to create the line
        int[] topPoints =
        {
            2, 3, 4, 5
        };

        public Line(Vector3 pos, Vector3 end) : base(pos, new Vector3(10, 10, 10))
        {
            this.end = end;

            foreach(int i in topPoints)
            {
                var vertex = vertices[i];
                vertex.Position = new Vector4(end, 1);
                vertices[i] = vertex;
            }
        }

        public override void Update(double gameTime)
        {
            foreach (int i in topPoints)
            {
                var vertex = vertices[i];
                vertex.Position = new Vector4(end, 1);
                vertices[i] = vertex;
            }

            base.Update(gameTime);
        }
    }
}
