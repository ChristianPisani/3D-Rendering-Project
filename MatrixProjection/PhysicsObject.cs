using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class PhysicsObject : GameObject
    {
        public Vector3 oldPos;
        public Vector3 vel, prevVel;
        public Vector3 acl, prevAcl;
        public float mass;
        public bool onGround = false;

        public PhysicsObject(Vector3 pos, Vector3 size, float mass) : base(pos, size)
        {            
            this.vel = Vector3.Zero;
            this.acl = Vector3.Zero;
            this.mass = mass;
            this.oldPos = pos;
        }

        public void ApplyForce(Vector3 force)
        {
            Vector3 f = force;
            f = Vector3.Divide(f, this.mass);
            float fC = 100;
            Vector3 forceLimited = new Vector3(MathHelper.Clamp(f.X, -fC, fC), MathHelper.Clamp(f.Y, -fC, fC), MathHelper.Clamp(f.Z, -fC, fC));
            this.acl += forceLimited;
        }

        public override void Update(double gameTime)
        {
            oldPos = new Vector3(pos.X, pos.Y, pos.Z);

            if (!onGround)
            {
                ApplyForce(new Vector3(0, GameConstants.gravity, 0));
            }

            prevVel = vel;
            vel += acl;
            pos += vel;

            onGround = false;

            prevAcl = acl;
            this.acl = Vector3.Zero;

            base.Update(gameTime);
        }

        public void StringForce(Vector3 origin, float length, float elasticity)
        {
            float distanceBetween = Vector3.Distance(origin, pos);
            if (distanceBetween > length)
            {
                pos = origin + Vector3.Normalize(pos - origin) * MathHelper.Lerp(distanceBetween, length, elasticity);

                vel = pos - oldPos;
            }
        }
    }
}
