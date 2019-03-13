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
        public static readonly float gravity = 1f;

        public static Vector3 SpringForce(Vector3 anchor, Vector3 pos, Vector3 vel, float Length)
        {
            Vector3 fX = new Vector3(anchor.X - pos.X,
                                     anchor.Y - pos.Y,
                                     anchor.Z - pos.Z);
            float length = fX.Length();
            float d = Length;
            float k = (-0.003f) * 50;
            float b = (0.005f) * 20;
            float lenD = length - d;
            Vector3 springForce = new Vector3(((-k * (MathHelper.Clamp(lenD, 0, d))) * (fX.X / length)) - (b * vel.X),
                                              ((-k * (MathHelper.Clamp(lenD, 0, d))) * (fX.Y / length)) - (b * vel.Y),
                                              ((-k * (MathHelper.Clamp(lenD, 0, d))) * (fX.Z / length)) - (b * vel.Z));

            float maxForce = 2;
            springForce.X = MathHelper.Clamp(springForce.X, -maxForce, maxForce);
            springForce.Y = MathHelper.Clamp(springForce.Y, -maxForce, maxForce);
            springForce.Z = MathHelper.Clamp(springForce.Z, -maxForce, maxForce);

            return springForce;
        }

        public static void StringForce(Vector3 origin, ref PhysicsObject target, float length, float elasticity)
        {
            float distanceBetween = Vector3.Distance(origin, target.pos);
            if (distanceBetween > length)
            {                
                target.pos = origin + Vector3.Normalize(target.pos - origin) * MathHelper.Lerp(distanceBetween, length, elasticity);

                target.vel = target.pos - target.oldPos;
                //ball.vel.y = min(ball.vel.y, 0);
            }
        }
    }
}
