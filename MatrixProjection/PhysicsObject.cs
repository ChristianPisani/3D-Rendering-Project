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

        public float timeStep = 1;

        public List<Cube> collideables;

        Line velLine;

        public PhysicsObject(Vector3 pos, Vector3 size, float mass) : base(pos, size)
        {
            this.vel = Vector3.Zero;
            this.acl = Vector3.Zero;
            this.mass = mass;
            this.oldPos = pos;

            velLine = new Line(pos, pos + vel, 1);

            collideables = new List<Cube>();
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
            if (!onGround)
            {
                ApplyForce(new Vector3(0, GameConstants.gravity, 0));
            }

            prevVel = vel;
            vel += acl;

            onGround = false;

            prevAcl = acl;
            this.acl = Vector3.Zero;

            base.Update(gameTime);

            for (int i = 0; i < timeStep; i++)
            {
                oldPos = new Vector3(pos.X, pos.Y, pos.Z);


                pos += (vel / timeStep);

                CollisionDetection(gameTime);
            }
        }

        public void CollisionDetection(double gameTime)
        {
            velLine.pos = oldPos;
            velLine.end = pos;

            if((velLine.end - velLine.pos).Length() < 10)
            {
                velLine.end += vel;
            }

            velLine.Update(gameTime);

            Vector3? intersection = null;

            foreach (Cube c in collideables)
            {
                bool collided = false;

                foreach (Plane plane in c.GetPlanes())
                {
                    intersection = IntersectionChecks.LinePlane(velLine, plane) ?? null;
                    if (intersection != null)
                    {
                        Vector3 newPos = (Vector3)intersection - plane.normal * new Vector3(1, GameConstants.gravity, 1);

                        pos = newPos;

                        var storedVelY = vel.Y;

                        Vector3 undesiredMotion = plane.normal * (Vector3.Dot(vel, plane.normal));
                        Vector3 desiredMotion = ((vel) - (undesiredMotion)) * new Vector3(0.95f, 1, 0.95f);

                        vel = desiredMotion;

                        if (plane.normal.Y > 0 && this is Player player)
                        {
                            player.canJump = true;
                            player.curJumpFrames = 0;

                            if (player.jumpPressed)
                            {
                                player.vel.Y = storedVelY;
                            }
                        }

                        collided = true;
                        break;
                    }
                }

                if (collided)
                {
                    break;
                }
            }
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
