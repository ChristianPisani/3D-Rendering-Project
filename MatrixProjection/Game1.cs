using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixProjection
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        //Textures
        public static Texture2D pixel;
        public static SpriteFont gameFont;

        //Game variables                
        public static Camera camera;

        float angle = 0.0f;

        public static Rectangle gameBounds = new Rectangle(0, 0, 400, 400);

        public static GameWindow gameWindow;

        //Objects
        public List<GameObject> gameObjects;
        public List<Flock> flocks;
        public Player player;

        //Gamestates
        bool Paused = false;
        bool PausePressed = false;

        bool Filled = false;
        bool FilledPressed = false;

        //Object composition
        Vector2 mid;
        int size = 50;
        int amount = 10;

        //Shaders
        Effect vertexShader;
        Effect pixelShader;


        Effect effect;

        Line l;


        RasterizerState rasterizerState = new RasterizerState();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 2500;
            graphics.PreferredBackBufferHeight = 1800;
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = Content.Load<Texture2D>("Pixel");
            gameFont = Content.Load<SpriteFont>("font1");
            vertexShader = Content.Load<Effect>("vertexShader");
            pixelShader = Content.Load<Effect>("pixelShader");
            effect = vertexShader;

            //effect = new BasicEffect(graphics.GraphicsDevice);
            effect.CurrentTechnique = effect.Techniques["BasicColorDrawing"];
            //effect.CurrentTechnique = effect.Techniques["DepthMap"];            
            effect.Parameters["Projection"].SetValue(camera.projectionMatrix);
            //effect.Parameters["Color"].SetValue(Color.SeaShell.ToVector4());
            effect.Parameters["LightPos"].SetValue(new Vector3(100000, 50000, 100000));
            effect.Parameters["LightPower"].SetValue(0.8f);
            effect.Parameters["LightColor"].SetValue(new Vector4(0.75f, 0.65f, .6f, 1));
            effect.Parameters["AmbientLightColor"].SetValue(new Vector4(.8f, .8f, .8f, 1f));

            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = rasterizerState;
        }

        protected override void Initialize()
        {
            camera = new Camera(GraphicsDevice);

            gameObjects = new List<GameObject>();

            mid = new Vector2((amount / 2) * size, (amount / 2) * size); //Vector for dist and stuff

            for (int x = 0; x < amount; x++)
            {
                for (int y = 0; y < amount; y++)
                {
                    float yPos = (Vector2.Distance(mid, new Vector2(x * size, y * size))) / 2;

                    //gameObjects.Add(new Cube(new Vector3(x * 250 + size, -1000, y * 250 + size), new Vector3(size, size, size)));
                }
            }


            Cube c = new Cube(new Vector3(50, 50, 50), new Vector3(100, 100, 100));
            //gameObjects.Add(c);

            Cylinder cyl = new Cylinder(new Vector3(-500, -500, -500), new Vector3(400f, 500f, 400f), 20);
            //gameObjects.Add(cyl);

            int numBoxes = 10;
            float radius = 300;
            float step = (float)(Math.PI * 2) / numBoxes;

            // gameObjects.Add(new Cube(new Vector3(0, 25, 1000), new Vector3(50)));

            for (float x = 0; x < Math.PI * 2; x += step)
            {
                Vector3 orig = new Vector3(0, 25, 1000);
                Cube cube = new Cube(
                    new Vector3((float)Math.Sin(x) * radius, (float)Math.Cos(x) * radius, 1000),
                    new Vector3(50)
                );

                Cube cube2 = new Cube(
                    new Vector3((float)Math.Sin(x) * radius, 25, (float)Math.Cos(x) * radius + 1000),
                    new Vector3(50)
                );

                cube.Rotation = MatrixHelper.RotateTowardMatrix(cube.pos, orig);
                cube2.Rotation = MatrixHelper.RotateTowardMatrix(cube2.pos, orig);

                //gameObjects.Add(cube);
                //gameObjects.Add(cube2);
            }


            //gameObjects.Add(new Plane(new Vector3(0, 0, 0), new Vector2(400, 400), 20));
            //gameObjects.Add(new Plane(new Vector3(450, 0, 0), new Vector2(400, 400), 2));

            flocks = new List<Flock>();

            flocks.Add(new Flock(300, new Vector3(5000, 5000, 5000), Color.White, Vector3.Zero));
            flocks.Add(new Flock(200, new Vector3(5000, 5000, 4999), Color.Aqua, Vector3.Zero));

            foreach (Flock flock in flocks)
            {
                gameObjects.AddRange(flock.flock);
            }


            player = new Player(new Vector3(0, -400, 0), new Vector3(20, 40, 20), 1);
            player.SetOrigin(new Vector3(0, -.5f, 0));
            player.color = Color.Red;
            gameObjects.Add(player);

            l = new Line(player.pos, player.pos, 2);


            Plane newPlane = new Plane(new Vector3(100, 100, 100), new Vector2(1000, 1000), 4);
            newPlane.color = Color.DarkKhaki;
            newPlane.Rotation = Matrix.CreateRotationY(MathHelper.ToRadians(45)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90));

            Vector3 transformedNormal; //Vector3.TransformNormal(newPlane.normal, Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(MathHelper.ToRadians(90)));
                                       //transformedNormal = Vector3.Cross(newPlane.normal, new Vector3(0, -1, 0));
            transformedNormal = newPlane.Rotation.Right;
            transformedNormal.Normalize();
            if (transformedNormal == Vector3.Zero) transformedNormal = new Vector3(1, 0, 0);
            transformedNormal.Normalize();
            Line planeNormal = new Line(newPlane.pos, newPlane.pos + transformedNormal * 200, 5);

            gameObjects.Add(planeNormal);

            Line newLine = new Line(new Vector3(100, -1000, 0), new Vector3(100, 100, 200), 3);
            gameObjects.Add(newLine);

            Cube newCube2 = new Cube(new Vector3(-700, 0, 801), new Vector3(20));
            newCube2.pos = newPlane.pos + transformedNormal * 501;
            Vector3 place = new Vector3(1, newPlane.size.Z / 2, 1);
            Vector3 planePos = newPlane.pos + transformedNormal * (newPlane.size.X / 2);
            Vector3 planePos2 = newPlane.pos - transformedNormal * (newPlane.size.X / 2);

            Cube kk = new Cube(planePos, new Vector3(10));
            kk.color = Color.Magenta;
            gameObjects.Add(kk);

            if (Vector3.Dot(newCube2.pos - planePos, transformedNormal) > 0 ||
                Vector3.Dot(newCube2.pos - planePos2, -transformedNormal) > 0)
            {
                newCube2.color = Color.Red;
            }
            else
            {
                newCube2.color = Color.Green;
            }

            Vector3 transformedNormal2 = Vector3.Cross(newPlane.normal, new Vector3(-1, 0, -1));
            transformedNormal2 = newPlane.Rotation.Forward;
            transformedNormal2.Normalize();
            Vector3 planePos3 = newPlane.pos + transformedNormal2 * (newPlane.size.Z / 2);
            Vector3 planePos4 = newPlane.pos - transformedNormal2 * (newPlane.size.Z / 2);


            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    Cube newKube = new Cube((-transformedNormal * 1000) + (-transformedNormal2 * 1000) + newPlane.pos + (transformedNormal * 100 * x) + (transformedNormal2 * 100 * y), new Vector3(20));

                    if (Vector3.Dot(newKube.pos - planePos, transformedNormal) > 0 ||
                        Vector3.Dot(newKube.pos - planePos2, -transformedNormal) > 0)
                    {
                        newKube.color = Color.Red;
                    }
                    else
                    {
                        newKube.color = Color.Green;
                    }


                    if (Vector3.Dot(newKube.pos - planePos3, transformedNormal2) > 0 ||
                        Vector3.Dot(newKube.pos - planePos4, -transformedNormal2) > 0)
                    {
                        newKube.color = Color.Red;
                    }
                    else if (newKube.color != Color.Red)
                    {
                        newKube.color = Color.Green;
                    }

                    gameObjects.Add(newKube);

                }
            }

            if (Vector3.Dot(newCube2.pos - planePos, transformedNormal2) > 0 ||
                Vector3.Dot(newCube2.pos - planePos, transformedNormal2) > 0)
            {
                newCube2.color = Color.Red;
            }
            else if (newCube2.color != Color.Red)
            {
                newCube2.color = Color.Green;
            }

            gameObjects.Add(newCube2);

            try
            {
                Cube newCube = new Cube((Vector3)IntersectionChecks.LinePlane(newLine, newPlane), new Vector3(10));
                gameObjects.Add(newCube);
            }
            catch (Exception e) { }

            gameObjects.Add(newPlane);



            CreateCity();

            base.Initialize();
        }

        public void CreateCity()
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int size = 40000;
                    Cube c = new Cube(new Vector3(x * size, 0, y * size), new Vector3(size, 1, size));

                    gameObjects.Add(c);


                    var rnd = new Random();

                    int divisions = 10;
                    int buildingSize = size / divisions;
                    for (int xx = 0; xx < divisions; xx++)
                    {
                        for (int yy = 0; yy < divisions; yy++)
                        {
                            if (rnd.Next(0, 10) <= 3)
                            {
                                var buildingHeight = rnd.Next(10000, 30000);
                                c = new Cube(new Vector3(x * size + xx * buildingSize, -buildingHeight / 2, y * size + yy * buildingSize), new Vector3(buildingSize, buildingHeight, buildingSize));
                                //c.Rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(10));

                                gameObjects.Add(c);

                                int chanceOfNewPartition = 90;
                                var partitionSize = buildingSize;
                                var partitionHeight = buildingHeight;
                                while (rnd.Next(0, 100) < chanceOfNewPartition)
                                {
                                    partitionSize = Math.Abs(partitionSize - rnd.Next(0, 1000));
                                    partitionHeight = rnd.Next(100, partitionHeight);
                                    Cube partition = new Cube(new Vector3(c.pos.X, c.pos.Y - c.size.Y / 2 + 5, c.pos.Z), new Vector3(partitionSize, partitionHeight, partitionSize));
                                    gameObjects.Add(partition);
                                    chanceOfNewPartition -= 10;
                                }

                                foreach (Plane p in c.GetPlanes())
                                {
                                    Line l = new Line(p.pos, p.pos + p.normal * 500, 10);

                                    //gameObjects.Add(p);
                                }
                            }
                        }
                    }
                }
            }
        }


        protected override void UnloadContent()
        {

        }

        Vector3? intersection = null;
        protected override void Update(GameTime gameTime)
        {
            gameWindow = Window;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                PausePressed = true;
            }
            else
            {
                if (PausePressed)
                {
                    Paused = !Paused;
                    PausePressed = false;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                FilledPressed = true;
            }
            else
            {
                if (FilledPressed)
                {
                    RasterizerState r = new RasterizerState();
                    r.CullMode = rasterizerState.CullMode;

                    if (Filled)
                    {
                        r.FillMode = FillMode.Solid;
                        GraphicsDevice.RasterizerState = r;
                    }
                    else
                    {
                        r.FillMode = FillMode.WireFrame;
                        r.CullMode = CullMode.None;
                        GraphicsDevice.RasterizerState = r;
                    }

                    Filled = !Filled;
                    FilledPressed = false;
                }
            }
            camera.controlsEnabled = false;
            Vector3 oldPos = new Vector3(player.pos.X, player.pos.Y, player.pos.Z);
            camera.Follow(player.pos, new Vector3(0, 20, 1000), 1);
            camera.RotateAround(player.pos, player.angle + camera.angle, 1f);
            camera.LookToward(Vector3.Lerp(oldPos, player.pos, 0.01f));
            camera.Update();

            if (Paused)
            {
                return;
            }

            angle += 0.05f;

            foreach (Flock flock in flocks)
            {
                flock.Update();
            }

            List<Cube> cubesInRange = gameObjects.Where(m => m is Cube && (m as Cube).inRangeOfPlayer).Cast<Cube>().ToList();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Cube cube && !(gameObject is Line))
                {
                    //cube.pos.Y = ((float)Math.Sin(angle + cube.pos.X + cube.pos.Z)) * 100;

                    float yPos = (Vector2.Distance(mid, new Vector2(gameObject.pos.X, gameObject.pos.Z)));
                    //cube.pos.Y = (float)Math.Sin(yPos / (Math.Sin(angle) * 200)) * 100;
                    //cube.pos.Y += (float)Math.Sin(angle + (yPos/150)) * 10;                    

                    cube.Update(gameTime.TotalGameTime.TotalMilliseconds, player.pos);
                }
                else if (gameObject is Player p)
                {
                    p.Update(gameTime.TotalGameTime.TotalMilliseconds, cubesInRange);
                }
                else
                {
                    gameObject.Update(gameTime.TotalGameTime.TotalMilliseconds);

                }
            }

            l.pos = player.oldPos;
            l.end = player.pos;

            if ((l.end - l.pos).Length() < 10)
            {
                l.end += player.vel;
            }

            l.Update(gameTime.TotalGameTime.TotalMilliseconds);


            List<Cube> cubesInRangeCircle = gameObjects.Where(m => m is Cube && Vector2.Distance(new Vector2(m.pos.X, m.pos.Z), new Vector2(player.pos.X, player.pos.Z)) < 2000).Cast<Cube>().ToList();

            player.onGround = false;
            foreach (Cube c in cubesInRange)
            {
                bool collided = false;

                foreach (Plane plane in c.GetPlanes())
                {
                    intersection = IntersectionChecks.LinePlane(l, plane) ?? null;
                    if (intersection != null)
                    {
                        Cylinder cyl = new Cylinder((Vector3)intersection, new Vector3(10, 10, 10), 10);
                        cyl.color = Color.Magenta;
                        //gameObjects.Add(cyl);
                        Vector3 oldPlayerPos = player.oldPos;

                        Vector3 newPos = (Vector3)intersection - plane.normal * new Vector3(1, GameConstants.gravity, 1);

                        if (Vector2.Distance(new Vector2(newPos.X, newPos.Z), new Vector2(player.pos.X, player.pos.Z)) > player.size.X)
                        {
                            // Resolve collision with sphere collider
                            //newPos += Vector3.Normalize()
                        }

                        
                            player.pos = newPos;

                        if (plane.normal.Y > 0)
                        {
                            player.canJump = true;
                            player.curJumpFrames = 0;
                        }

                        //player.pos.Y = intersection.Value.Y;


                        //player.vel = oldPlayerPos - player.pos;
                        var storedVelY = player.vel.Y;

                        Vector3 undesiredMotion = plane.normal * (Vector3.Dot(player.vel, plane.normal));
                        Vector3 desiredMotion = ((player.vel) - (undesiredMotion)) * new Vector3(0.95f, 1, 0.95f);


                        player.vel = desiredMotion;

                        if(player.jumpPressed)
                        {
                            player.vel.Y = storedVelY;
                        }
                        //player.vel.Y = Math.Min(0, player.vel.Y);


                        collided = true;
                        break;

                        //player.vel = Vector3.Reflect(player.vel, plane.normal) * 0.1f;
                    }
                }

                if (collided)
                {
                    break;
                }

            }



            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);
            //GraphicsDevice.Clear(Color.White);

            effect.Parameters["View"].SetValue(camera.viewMatrix);
            effect.Parameters["CameraPos"].SetValue(camera.pos);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(graphics.GraphicsDevice, camera, effect);
            }
            l.Draw(graphics.GraphicsDevice, camera, effect);



            /*spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);

            double angle = Math.Atan2((0 - player.pos.Z), (0 - player.pos.X) - Game1.camera.angle.X);

            spriteBatch.DrawString(gameFont, (player.vel.Y).ToString(), new Vector2(30, 30), Color.White);

            spriteBatch.End();*/

            base.Draw(gameTime);
        }
    }
}

