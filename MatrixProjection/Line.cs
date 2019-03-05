using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    class Line : Cube
    {
        public Vector3 end;

        public Line(Vector3 pos, Vector3 end, int thickness) : base(pos, new Vector3(thickness, thickness, thickness))
        {
            this.end = end;
            SetOrigin(new Vector3(0, -.5f, 0));
        }

        public override void Update(double gameTime)
        {
            size.Y = (end - pos).Length();
            scale = Matrix.CreateScale(size);
            Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * MatrixHelper.RotateTowardMatrix(pos, end);

            base.Update(gameTime);
        }
    }
}
