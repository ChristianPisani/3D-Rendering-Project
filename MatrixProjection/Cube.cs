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
    public class Cube : GameObject
    {        
        int[] triangles = {
                0, 2, 1, //face front
	    		0, 3, 2,
                2, 3, 4, //face top
			    2, 4, 5,
                1, 2, 5, //face right
	    		1, 5, 6,
                0, 7, 4, //face left
			    0, 4, 3,
                5, 4, 7, //face back
	    		5, 7, 6,
                0, 6, 7, //face bottom
		    	0, 1, 6
            };

        public bool inRangeOfPlayer = false;

        public bool Selected = false;

        public Cube(Vector3 pos, Vector3 size) : base(pos, size)
        {

        }

        public Cube(Vector3 pos, Vector3 size, Vector3 origin) : base(pos, size, origin)
        {

        }

        public void Update(double gameTime, Vector3 playerpos)
        {
            inRangeOfPlayer = false;
            color = Color.Gray;
            if(Vector2.Distance(new Vector2(pos.X, pos.Z), new Vector2(playerpos.X, playerpos.Z)) < 20000)
            {
                //color = Color.Red;
                inRangeOfPlayer = true;
            }
            if(Selected)
            {
                color = Color.Green;
            }
            Selected = false;

            base.Update(gameTime);
        }        
    }
}
