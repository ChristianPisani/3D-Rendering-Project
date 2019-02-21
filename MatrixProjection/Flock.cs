using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Flock
    {
        public List<Boid> flock;
        public Vector3 bounds, pos;
        public Color color; 

        public Flock(int amount, Vector3 bounds, Color color, Vector3 pos = new Vector3())
        {
            this.bounds = bounds;
            this.pos = pos;

            //Prevent dividing by zero
            bounds.X = Math.Max(bounds.X, 1);
            bounds.Y = Math.Max(bounds.Y, 1);
            bounds.Z = Math.Max(bounds.Z, 1);

            Random rnd = new Random();
            flock = new List<Boid>();

            for (int i = 0; i < amount + 1; i++)
            {
                Vector3 rndCoords = new Vector3()
                {
                    X = rnd.Next((int)(pos.X), (int)(pos.X + bounds.X)) - (bounds.X / 2),
                    Y = rnd.Next((int)(pos.Y), (int)(pos.Y + bounds.Y)) - (bounds.Y / 2),
                    Z = rnd.Next((int)(pos.Z), (int)(pos.Z + bounds.Z)) - (bounds.Z / 2)
                };

                Boid boid = new Boid(rndCoords, new Vector3(50));

                boid.color = color;


                float maxSpeed = 5f;
                float minSpeed = 1f;
                Vector3 rndAcl = new Vector3()
                {
                    X = minSpeed + maxSpeed / rnd.Next(1, 100),
                    Y = minSpeed + maxSpeed / rnd.Next(1, 100),
                    Z = minSpeed + maxSpeed / rnd.Next(1, 100)
                };

                int dir = rnd.Next(0, 2);
                if (dir == 0) rndAcl.X *= -1;

                dir = rnd.Next(0, 2);
                if (dir == 0) rndAcl.Y *= -1;

                dir = rnd.Next(0, 2);
                if (dir == 0) rndAcl.Z *= -1;



                boid.acl = rndAcl;

                flock.Add(boid);
            }
        }

        public void Update()
        {
            List<Boid> flockCopy = new List<Boid>();

            foreach (Boid b in flock)
            {
                flockCopy.Add(b);
            }

            foreach (Boid boid in flock)
            {

                boid.acl += boid.SteeringForce(flockCopy) * 2;
                //boid.acl += boid.Cohesion(flockCopy) * 1f;
                //boid.acl += boid.Separation(flockCopy) * 3.5f;

                boid.Update();


                Vector3 vel = Vector3.Normalize(boid.vel);
                //boid.points[6] = Vector3.Normalize(boid.vel);
                //boid.points[3] = vel + new Vector3(-0.5f, 0, -0.5f);
                //boid.points[2] = vel + new Vector3(-0.5f, -0.5f, 0);
                //boid.points[1] = vel + new Vector3(-0.5f, 0, 0.5f);
                //boid.points[0] = vel + new Vector3(-0.5f, 0.5f, 0);

                //ConstrainToBounds(boid);
            }
        }

        

        public void DrawBounds(Camera camera, GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            Cube c = new Cube(pos, bounds);
            c.Draw(graphicsDevice, camera, effect);

            foreach(Boid b in flock)
            {
                //Vector3 p = MatrixHelper.getMappedPoint(b.pos, camera);
                //Vector3 p2 = MatrixHelper.getMappedPoint(b.pos + b.vel, camera);

                //Cube cb = new Cube(b.pos, new Vector3(10));
                //cb.Draw(Game1.spriteBatch, camera);

                //cb = new Cube(b.pos + Vector3.Normalize(b.vel) * 10, new Vector3(7));
                //cb.Draw(Game1.spriteBatch, camera);
                //Game1.spriteBatch.Draw(Game1.pixel, new Rectangle((int)(p.X), (int)p.Y, 10, 10), Color.Magenta);
                //Game1.spriteBatch.Draw(Game1.pixel, new Rectangle((int)(p2.X), (int)p2.Y, 5, 5), Color.LightBlue);

            }
        }

        public void ConstrainToBounds(Boid boid)
        {
            Vector3 halfBounds = new Vector3(bounds.X / 2, bounds.Y / 2, bounds.Z / 2);
            Vector3 colBounds = bounds - halfBounds;
            Vector3 boidOffSet = boid.pos + halfBounds;


            if (boid.pos.X < pos.X - halfBounds.X)
            {
                boid.pos.X = pos.X + colBounds.X;
            }

            if (boid.pos.X > pos.X + colBounds.X)
            {
                boid.pos.X = pos.X - halfBounds.X;
            }

            if (boid.pos.Y < pos.Y - halfBounds.Y)
            {
                boid.pos.Y = pos.Y + colBounds.Y;
            }

            if (boid.pos.Y > pos.Y + colBounds.Y)
            {
                boid.pos.Y = pos.Y - halfBounds.Y;
            }

            if (boid.pos.Z < pos.Z - halfBounds.Z)
            {
                boid.pos.Z = pos.Z + colBounds.Z;
            }

            if (boid.pos.Z > pos.Z + colBounds.Z)
            {
                boid.pos.Z = pos.Z - halfBounds.Z;
            }

            boid.pos.X = MathHelper.Clamp(boid.pos.X, pos.X - halfBounds.X, pos.X + colBounds.X);
            boid.pos.Y = MathHelper.Clamp(boid.pos.Y, pos.Y - halfBounds.Y, pos.Y + colBounds.Y);
            boid.pos.Z = MathHelper.Clamp(boid.pos.Z, pos.Z - halfBounds.Z, pos.Z + colBounds.Z);
        }
    }
}
