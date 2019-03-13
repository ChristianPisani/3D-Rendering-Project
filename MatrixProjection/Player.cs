using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Player : PhysicsObject
    {
        //Controls
        KeyboardState ks;
        Keys leftKey = Keys.A;
        Keys rightKey = Keys.D;
        Keys upKey = Keys.W;
        Keys downKey = Keys.S;
        Keys jumpKey = Keys.Space;
        public bool canJump = true;
        public int jumpFrames = 10;
        public int curJumpFrames = 0;
        bool jumpPressed = false;

        Vector3 speed = new Vector3(2);
        float maxVel = 50;
        float jumpStrength = 2;

        float webLength = 400;
        Vector3 anchor = new Vector3(600, 0, 0);

        public Vector2 angle = new Vector2(0, 0);

        Line web;

        readonly Vector3 baseForwardVector = new Vector3(0, 0, -1);
        readonly Vector3 baseSideVector = new Vector3(-1, 0, 0);
        Vector3 forwardVector = new Vector3(0, 0, -1);
        Vector3 sideVector = new Vector3(-1, 0, 0);

        public Player(Vector3 pos, Vector3 size, float mass) : base(pos, size, mass)
        {
            web = new Line(pos, new Vector3(2, webLength, 2), 2);            
            web.color = Color.LightCoral;       
        }

        public void Update(double gameTime, List<Cube> colObjects)
        {
            base.Update(gameTime);

            float closestDist = float.MaxValue;
            float closestDot = 100000000000000000;
            Cube closestCube = new Cube(Vector3.Zero, Vector3.Zero);
            foreach (Cube c in colObjects)
            {
                var dist = c.pos - pos;
                   
                var dot = Vector3.Dot(Vector3.Normalize(dist), forwardVector);
                if (c.size.Y > 100 && dist.Length() > 500 && dist.Length() < closestDist && dot > 0 && dot > 0.5f)
                {
                    closestDist = dist.Length();
                    closestDot = dot;
                    closestCube = c;                    
                    
                }                
            }
            closestCube.Selected = true;
            if (!ks.IsKeyDown(Keys.E))
            {
                //anchor = new Vector3(pos.X + forwardVector.X * 400, pos.Y - 400, pos.Z + forwardVector.Z * 400);
                anchor = closestCube.pos - new Vector3(0, closestCube.size.Y/2, 0);
            }

            HandleInput();            

            vel.X = MathHelper.Clamp(vel.X, -maxVel, maxVel);
            vel.Z = MathHelper.Clamp(vel.Z, -maxVel, maxVel);
            
            if (pos.Y >= 0 || onGround)
            {
                canJump = true;
                curJumpFrames = 0;
            }
            else
            {
                curJumpFrames++;
            }
            if ((curJumpFrames > 0 && ks.IsKeyUp(jumpKey)))
            {
                canJump = false;                
            }

            pos.Y = Math.Min(0, pos.Y);
            
            //pos.X = MathHelper.Clamp(pos.X, 0, 1000);
            //pos.Z = MathHelper.Clamp(pos.Z, 0, 1000);


            web.pos = pos - new Vector3(0, size.Y / 2f, 0);
            web.end = anchor;
            web.Update(gameTime);

            if (!canJump || onGround)
            {
                var v = Vector3.One;
                if (vel.X != 0 || vel.Z != 0) v = vel;
                //rotation = MatrixHelper.RotateTowardMatrix(pos, pos + vel);
            } else
            {
               //rotation = Matrix.CreateRotationY(0);
            }
        }

        public override void Draw(GraphicsDevice graphicsDevice, Camera camera, Effect effect)
        {
            float length = (pos - anchor).Length();
            webLength = MathHelper.Lerp(length, 4000, 0.005f);
            //double angle = Math.Atan2(anchor.Y - bounds.Center.Y, anchor.X - bounds.Center.X);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed || ks.IsKeyDown(Keys.E))
            {
                //Draw web
                web.Draw(graphicsDevice, camera, effect);
            }

            base.Draw(graphicsDevice, camera, effect);            
        }       

        public void HandleInput()
        {
            ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (ks.IsKeyDown(Keys.E))
            {
                //Vector3 sp = GameConstants.SpringForce(anchor, pos, new Vector3(vel.X/3, vel.Y, vel.Z));
                //ApplyForce(sp);

                //Vector3 f = (anchor - pos) / 100;
                //Vector3 f = GameConstants.SpringForce(anchor, pos, new Vector3(vel.X/3, vel.Y, vel.Z), MathHelper.Clamp(webLength * 0.9f, 1000, 100000));
                //f.Y = Math.Min(f.Y, 0);
                //f = Vector3.Clamp(f, new Vector3(-5), new Vector3(5));
                //ApplyForce(f);

                //ApplyForce(new Vector3(forwardVector.X, 0, forwardVector.Z) * 10);
                //ApplyForce(new Vector3(0, GameConstants.gravity, 0));

                StringForce(anchor, webLength, 0.5f);
            }
            if(!ks.IsKeyDown(Keys.E))
            {
                //anchor = pos + (vel * 100);
                //anchor.Y = pos.Y - 400;
                webLength = 0;
            }

            forwardVector = baseForwardVector;
            sideVector = baseSideVector;

            var rotationMatrix = Matrix.CreateRotationY(angle.X);
            forwardVector = Vector3.Transform(forwardVector, rotationMatrix) * speed;
            sideVector = Vector3.Transform(sideVector, rotationMatrix) * speed;

            if (Keyboard.GetState().IsKeyDown(upKey))
            {
                ApplyForce(forwardVector);
            }

            if (Keyboard.GetState().IsKeyDown(downKey))
            {
                ApplyForce(-forwardVector);
            }

            float angularSpeed = 0.05f;
            rotation = Matrix.CreateRotationY(angle.X);
            if (Keyboard.GetState().IsKeyDown(leftKey))
            {
                angle.X -= angularSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(rightKey))
            {
                angle.X += angularSpeed;
            }



            if (ks.IsKeyDown(jumpKey))
            {
                jumpPressed = true;

                if (canJump)
                {
                    if (curJumpFrames >= jumpFrames)
                    {
                        canJump = false;
                    }

                    if (curJumpFrames == 0)
                    {
                        ApplyForce(new Vector3(0, -vel.Y, 0));
                    }

                    curJumpFrames++;

                    ApplyForce(new Vector3(forwardVector.X * jumpStrength, -jumpStrength, forwardVector.Z * jumpStrength));
                }
                else
                {
                    //angle = MathHelper.ToRadians(-360);
                }
            } 
            if(ks.IsKeyUp(jumpKey))
            {
                if(jumpPressed)
                {
                    ApplyForce(Vector3.Normalize(vel) * 15);

                    jumpPressed = false;
                }
            }
        }
    }
}
