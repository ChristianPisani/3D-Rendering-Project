/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Player : Object
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

        Point speed = new Point(6, 6);
        float maxVel = 22;
        float jumpStrength = 4;

        float webLength = 0;
        Vector2 anchor = new Vector2(600, 0);

        public Player(Vector2 pos, float mass, int w, int h) : base(pos, mass, w, h)
        {
            
        }

        public override void Update()
        {
            base.Update();

            HandleInput();

            vel.X = MathHelper.Clamp(vel.X, -maxVel, maxVel);

            float dAngle = (float)Math.Atan2(-(double)Vector2.Normalize(vel).X, 1);
            angle = MathHelper.Lerp(angle, dAngle, 0.1f);            

            if (pos.Y >= Game1.gameBounds.Height - bounds.Height)
            {
                canJump = true;
                curJumpFrames = 0;
                vel.X = MathHelper.Lerp(vel.X, 0, 0.2f);
            }
            else
            {
                curJumpFrames++;
            }
            if ((curJumpFrames > 0 && ks.IsKeyUp(jumpKey)))
            {
                canJump = false;                
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D tex, Camera camera)
        {
            //float length = MathHelper.Lerp(webLength, (bounds.Center.ToVector2() - anchor).Length(), 0.1f);
            float length = (bounds.Center.ToVector2() - anchor).Length();
            webLength = length;
            double angle = Math.Atan2(anchor.Y - bounds.Center.Y, anchor.X - bounds.Center.X);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed || ks.IsKeyDown(Keys.E))
            {
                spriteBatch.Draw(tex,
                    bounds.Center.ToVector2() - camera.pos,
                    new Rectangle(0, 0, (int)length, 2),
                    Color.White,
                    (float)angle,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    1);
            }

            base.Draw(spriteBatch, tex, camera);            
        }       

        public void HandleInput()
        {
            ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed || ks.IsKeyDown(Keys.E))
            {
                Vector2 sp = GameConstants.SpringForce(anchor, pos, new Vector2(vel.X/3, vel.Y));
                ApplyForce(sp);
            } else
            {
                anchor.X = ms.Position.X + Game1.camera.pos.X;
                anchor.Y = ms.Position.Y + Game1.camera.pos.Y;
                webLength = 0;
            }

            if (ks.IsKeyDown(leftKey))
            {
                ApplyForce(new Vector2(-speed.X, 0));                
            }
            if (ks.IsKeyDown(rightKey))
            {
                ApplyForce(new Vector2(speed.X, 0));
            }
            if (ks.IsKeyDown(upKey))
            {
                //ApplyForce(new Vector2(0, -speed.Y));                
            }
            if (ks.IsKeyDown(downKey))
            {
                //ApplyForce(new Vector2(0, speed.Y));                
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
                        ApplyForce(new Vector2(0, -vel.Y));
                    }

                    curJumpFrames++;

                    ApplyForce(new Vector2(0, -jumpStrength));
                }
                else
                {
                    angle = MathHelper.ToRadians(-360);
                }
            } 
            if(ks.IsKeyUp(jumpKey))
            {
                if(jumpPressed)
                {
                    ApplyForce(Vector2.Normalize(vel) * 15);

                    jumpPressed = false;
                }
            }
        }
    }
}
*/