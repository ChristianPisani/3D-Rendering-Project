/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Object
    {
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 acl;
        public float mass;
        public Color color;
        public Rectangle bounds;
        public float angle;
        bool onGround = false;

        public Object(Vector2 pos, float mass, int w, int h)
        {
            this.pos = pos;
            this.vel = Vector2.Zero;
            this.acl = Vector2.Zero;
            this.mass = mass;
            this.color = new Color(r: 130, g: 200, b: 255);
            this.angle = 0;
            this.bounds = new Rectangle((int)pos.X, (int)pos.Y, w, h);
        }

        public void ApplyForce(Vector2 force)
        {
            Vector2 f = force;
            f = Vector2.Divide(f, this.mass);
            float fC = 100;
            Vector2 forceLimit = new Vector2(MathHelper.Clamp(f.X, -fC, fC), MathHelper.Clamp(f.Y, -fC, fC));
            this.acl += forceLimit;
        }

        public virtual void Update()
        {
            if (!onGround)
            {
                ApplyForce(new Vector2(0, GameConstants.gravity));
            }

            vel += acl;
            pos += vel;
            bounds.X = (int)pos.X;
            bounds.Y = (int)pos.Y;

            pos.X = MathHelper.Clamp(pos.X, 0, Game1.gameBounds.Width - bounds.Width);
            pos.Y = MathHelper.Clamp(pos.Y, 0, Game1.gameBounds.Height - bounds.Height);

            if(pos.Y >= Game1.gameBounds.Height - bounds.Height)
            {
                onGround = true;
            } else
            {
                onGround = false;
            }

            this.acl = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D tex, Camera camera)
        {
            spriteBatch.Draw(tex,
                    pos - camera.pos + new Vector2(bounds.Width / 2, bounds.Height / 2),
                    bounds,
                    color,
                    angle,
                    new Vector2(bounds.Width/2, bounds.Height/2),
                    Vector2.One,
                    SpriteEffects.None,
                    1);
        }
    }
}
*/