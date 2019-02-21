﻿using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Boid : GameObject
    {
        public Vector3 vel, acl;

        public Vector3 MaxForce = new Vector3(0.2f);

        public Vector3 MaxSpeed = new Vector3(8f);
        public Vector3 MinSpeed = new Vector3(3f);

        float perceptionRadius = 500;

        private Vector3 prevPos;

        public Color color = Color.White * 0.8f;

        Vector3[] points = {
            new Vector3 (0, 0, 1),
            new Vector3 (-.5f, -.5f, -1),
            new Vector3 (.5f, -.5f, -1),
            new Vector3 (0, .5f, -1),
        };

        int[] triangles = {
            1, 3, 0,
            2, 1, 0,
            3, 2, 0,
            1, 2, 3
        };


        public Boid(Vector3 pos, Vector3 size) : base(pos, size)
        {
            prevPos = pos;
        }

        public void MapPoints()
        {
            var points = new List<Vector3>();

            points.Add(new Vector3(0f, 0.5f, 0));
            points.Add(new Vector3(0f, 0, 0.5f));
            points.Add(new Vector3(0f, -0.5f, 0));
            points.Add(new Vector3(0f, 0, -0.5f));

            points.Add(new Vector3(1f, 0, 0));


            points.Add(Vector3.Zero);
            points.Add(Vector3.Zero);
        }
        
        public void Update()
        {
            prevPos = pos;
            Vector3 prevVel = vel;
            
            vel = Vector3.Clamp(vel, -MaxSpeed, MaxSpeed);

            acl = Vector3.Clamp(acl, -MaxForce, MaxForce);

            this.pos += vel;
            this.vel += acl;

            Vector3 A = vel;
            A.Normalize();
            A *= new Vector3(0.1f);

            Vector3 pA = prevVel;
            pA.Normalize();
            pA *= new Vector3(0.1f);

            A = Vector3.Lerp(pA, A, 0.3f);

            Vector3 diff = prevVel - vel;

            float a = (float)Math.Atan2(A.Z, A.X);
            float b = (float)Math.Atan(A.Y);

            float pitch = (float)Math.Asin(A.Y);
            float yaw = (float)Math.Atan2(A.X, A.Z);

            float ang = (float)Math.Acos(Vector3.Dot(pos, A));

            Vector3 axis = Vector3.Normalize(Vector3.Cross(pos, A));

            Quaternion Q = Quaternion.CreateFromAxisAngle(axis, ang);

            //this.rotation = Matrix.CreateFromAxisAngle(axis, ang);
            this.rotation = Matrix.CreateFromYawPitchRoll(pitch, -yaw, 0);
            //this.rotation = Matrix.CreateRotationZ(b) * Matrix.CreateRotationY(a);
            //this.rotation = Matrix.CreateRotationZ(angle); //* Matrix.CreateRotationY(a);

            //this.rotation = Matrix.CreateRotationX((float)Math.Cos(angle)) * Matrix.CreateRotationY((float)Math.Sin(angle)) 
            //    * Matrix.CreateRotationZ((float)Math.Sin(angle2));
            //this.rotation = Matrix.CreateTranslation(new Vector3(1)) * Matrix.CreateLookAt(Vector3.Zero, A, new Vector3(0, 1, 0));
            //this.rotation = Matrix.CreateRotationY((float)(angle)) * Matrix.CreateRotationZ(angle) * Matrix.CreateRotationX(angle);
            //this.rotation = MatrixHelper.directionRotationMatrix(A);
            //this.rotation = Matrix.CreateFromQuaternion(Matrix.CreateLookAt(Vector3.Zero, vel, new Vector3(0, 1, 0)).Rotation);

            this.acl = Vector3.Zero;

        }

        public Vector3 SteeringForce(List<Boid> others)
        {
            Vector3 avgSpeed = Vector3.Zero;
            Vector3 cohesion = Vector3.Zero;
            Vector3 seperation = Vector3.Zero;

            int total = 0;

            foreach (Boid b in others)
            {
                float distance = Vector3.Distance(this.pos, b.pos);
                if (b != this && distance < perceptionRadius)
                {
                    avgSpeed += b.vel;
                    cohesion += b.pos;

                    Vector3 diff = pos - b.pos;
                    diff /= distance;
                    seperation += diff;

                    total++;                    
                }
            }


            /*AVOID CIRCLE ****
            float dist = Vector3.Distance(this.pos, new Vector3(0, pos.Y, 0));

            if (dist < 400f)
            {
                Vector3 diff = pos;
                diff /= (dist / 2);

                seperation += diff;
                total++;
            }
            //********************/

            if (total > 0)
            {
                avgSpeed /= total;
                avgSpeed -= this.vel;

                cohesion /= total;
                cohesion -= this.pos;

                seperation /= total;
            }

            if (avgSpeed.Length() > 0)
            {
                avgSpeed.Normalize();
            }
            if (cohesion.Length() > 0)
            {
                cohesion.Normalize();
            }
            avgSpeed *= MaxSpeed;
            avgSpeed = Vector3.Clamp(avgSpeed, -MaxForce, MaxForce);

            cohesion *= MaxSpeed;
            cohesion = Vector3.Clamp(cohesion, -MaxForce, MaxForce);

            seperation = Vector3.Clamp(seperation, -MaxForce, MaxForce);

            avgSpeed *= 2f;
            cohesion *= 1f;
            seperation *= 0.5f;

            return avgSpeed + cohesion + seperation;
        }

        public override void MapVertices()
        {
            vertices = new List<CustomVertex>();

            for (int i = 0; i < triangles.Length - 2; i += 3)
            {
                CustomVertex v = new CustomVertex();
                CustomVertex v1 = new CustomVertex();
                CustomVertex v2 = new CustomVertex();

                var index = triangles[i];
                var index1 = triangles[i + 1];
                var index2 = triangles[i + 2];

                v.Position = points[points.Length - 1 - index];
                v1.Position = points[points.Length - 1 - index1];
                v2.Position = points[points.Length - 1 - index2];

                Vector3 normal = getVertexNormal(v.Position, v1.Position, v2.Position);
                
                //v.Normal = normal;
                //v1.Normal = normal;
                //v2.Normal = normal;

                vertices.Add(v);
                vertices.Add(v1);
                vertices.Add(v2);
            }
        }
    }
}