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
        bool canJump = true;
        int jumpFrames = 10;
        int curJumpFrames = 0;
        bool jumpPressed = false;

        Vector3 speed = new Vector3(1, 1, 1);
        float maxVel = 8;
        float jumpStrength = 2;

        float webLength = 400;
        Vector3 anchor = new Vector3(600, 0, 0);

        public Vector2 angle = new Vector2(0, 0);

        Cube web;
        Cube webEnd;

        public Player(Vector3 pos, Vector3 size, float mass) : base(pos, size, mass)
        {
            web = new Cube(pos, new Vector3(2, webLength, 2));
            web.color = Color.Blue;
            webEnd = new Cube(pos, new Vector3(5, 5, 5));
            webEnd.color = Color.Blue;
        }

        public override void Update(double gameTime)
        {
            base.Update(gameTime);

            HandleInput();

            vel.X = MathHelper.Clamp(vel.X, -maxVel, maxVel);
            vel.Z = MathHelper.Clamp(vel.Z, -maxVel, maxVel);
            
            if (pos.Y >= 0)
            {
                canJump = true;
                curJumpFrames = 0;
                vel.X = MathHelper.Lerp(vel.X, 0, 0.2f);
                vel.Z = MathHelper.Lerp(vel.Z, 0, 0.2f);
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


            web.pos = pos; //new Vector3(0, webLength / 2, 0) ;
            webEnd.pos = anchor;
            web.size.Y = webLength;
            web.scale = Matrix.CreateScale(web.size);
            web.rotation = Matrix.CreateTranslation(new Vector3(0, -webLength/2, 0)) * MatrixHelper.RotateTowardMatrix(pos, anchor) * Matrix.CreateRotationX(MathHelper.ToRadians(90)); 
        }

        public override void Draw(GraphicsDevice graphicsDevice, Camera camera, Effect effect)
        {
            float length = (pos - anchor).Length();
            webLength = length;
            //double angle = Math.Atan2(anchor.Y - bounds.Center.Y, anchor.X - bounds.Center.X);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed || ks.IsKeyDown(Keys.E))
            {
                //Draw web
                web.Draw(graphicsDevice, camera, effect);
                webEnd.Draw(graphicsDevice, camera, effect);
            }            

            base.Draw(graphicsDevice, camera, effect);            
        }       

        public void HandleInput()
        {
            ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (ks.IsKeyDown(Keys.E))
            {
                Vector3 sp = GameConstants.SpringForce(anchor, pos, new Vector3(vel.X/3, vel.Y, vel.Z));
                ApplyForce(sp);
            }
            if(!ks.IsKeyDown(Keys.E))
            {
                anchor = pos + (vel * 100);
                anchor.Y = pos.Y - 400;
                webLength = 0;
            }

            Vector3 forwardVector = new Vector3(0, 0, -1);
            Vector3 sideVector = new Vector3(-1, 0, 0);
            
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

                    ApplyForce(new Vector3(0, -jumpStrength, 0));
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
