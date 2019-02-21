using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public static class GameConstants
    {
        public static readonly float gravity = 0.7f;

        public static Vector2 SpringForce(Vector2 anchor, Vector2 pos, Vector2 vel)
        {
            Vector2 fX = new Vector2(anchor.X - pos.X,
                                  anchor.Y - pos.Y);
            float length = fX.Length();
            float d = 100;
            float k = (-0.003f) * 20;
            float b = (0.005f) * 20;
            float lenD = length - d;
            Vector2 springForce = new Vector2(((-k * (MathHelper.Clamp(lenD, 0, d))) * (fX.X / length)) - (b * vel.X),
                                             ((-k * (MathHelper.Clamp(lenD, 0, d))) * (fX.Y / length)) - (b * vel.Y));

            float maxForce = 2;
            springForce.X = MathHelper.Clamp(springForce.X, -maxForce, maxForce);
            springForce.Y = MathHelper.Clamp(springForce.Y, -maxForce, maxForce);

            return springForce;
        }
    }
}
