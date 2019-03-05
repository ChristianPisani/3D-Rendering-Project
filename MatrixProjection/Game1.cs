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


            player = new Player(new Vector3(0, -40, 0), new Vector3(20, 40, 20), 1);
            player.SetOrigin(new Vector3(0, -.5f, 0));
            player.color = Color.Red;
            gameObjects.Add(player);


            Plane plane = new Plane(new Vector3(100, 100, 100), new Vector2(1000, 1000), 1);
            plane.Rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(90));
            gameObjects.Add(plane);
            Line line = new Line(new Vector3(260, 0, 60), new Vector3(50, 100, 50), 5);            
            gameObjects.Add(line);

            Cube kube = new Cube(IntersectionChecks.LinePlane(line, plane), new Vector3(10));
            
            gameObjects.Add(kube);

            //CreateCity();

            base.Initialize();
        }

        public void CreateCity()
        {
            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    int size = 30000;
                    Cube c = new Cube(new Vector3(x * size, 0, y * size), new Vector3(size, 1, size), new Vector3(0, 0.5f, 0));

                    gameObjects.Add(c);


                    var rnd = new Random();

                    int divisions = 10;
                    int buildingSize = size / divisions;
                    for(int xx = 0; xx < divisions; xx++)
                    {
                        for(int yy = 0; yy < divisions; yy++)
                        {
                            if(rnd.Next(0, 10) == 1)
                            {
                                var buildingHeight = rnd.Next(5000, 10000);
                                c = new Cube(new Vector3(x * size + xx * buildingSize, -buildingHeight/2, y * size + yy * buildingSize), new Vector3(buildingSize, buildingHeight, buildingSize));
                                gameObjects.Add(c);
                            }
                        }
                    }
                }
            }
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
            effect.Parameters["LightPos"].SetValue(new Vector3(4000, 4000, 4000));
            effect.Parameters["LightPower"].SetValue(0.5f);
            effect.Parameters["LightColor"].SetValue(new Vector4(0.6f, 0.6f, .6f, 1));
            effect.Parameters["AmbientLightColor"].SetValue(new Vector4(.4f, .4f, .4f, 1f));
            //effect.AmbientLightColor = new Vector3(.7f, .2f, .4f);
            //effect.Texture = pixel;
            //effect.EmissiveColor = new Vector3(1, 0, 0);
            //effect.TextureEnabled = true;
            //effect.EnableDefaultLighting();

            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = rasterizerState;
        }


        protected override void UnloadContent()
        {

        }

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

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Cube cube && !(gameObject is Line))
                {
                    //cube.pos.Y = ((float)Math.Sin(angle + cube.pos.X + cube.pos.Z)) * 100;

                    float yPos = (Vector2.Distance(mid, new Vector2(gameObject.pos.X, gameObject.pos.Z)));
                    //cube.pos.Y = (float)Math.Sin(yPos / (Math.Sin(angle) * 200)) * 100;
                    //cube.pos.Y += (float)Math.Sin(angle + (yPos/150)) * 10;                    

                    cube.Update(gameTime.TotalGameTime.TotalMilliseconds, player.pos);
                } else if(gameObject is Player p)
                {
                    p.Update(gameTime.TotalGameTime.TotalMilliseconds, gameObjects.Where(m => m is Cube && (m as Cube).inRangeOfPlayer).Cast<Cube>().ToList());
                } else
                {
                    gameObject.Update(gameTime.TotalGameTime.TotalMilliseconds);

                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Teal);
            //GraphicsDevice.Clear(Color.White);

            effect.Parameters["View"].SetValue(camera.viewMatrix);
            effect.Parameters["CameraPos"].SetValue(camera.pos);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(graphics.GraphicsDevice, camera, effect);
            }


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);

            double angle = Math.Atan2((0 - player.pos.Z), (0 - player.pos.X) - Game1.camera.angle.X);

            spriteBatch.DrawString(gameFont, (Vector3.Dot(Vector3.Normalize(camera.lookAt), Vector3.Normalize(Vector3.One))).ToString(), new Vector2(30, 30), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

