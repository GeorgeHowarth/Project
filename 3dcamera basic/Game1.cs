using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

/// <summary>
/// This is the main type for your game.
/// </summary>
namespace CamTest
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect beffect;
        Basic3dExampleCamera cam;
        Matrix camWorldObjectToVisualize = Matrix.Identity;

        Grid3dOrientation worldGrid = new Grid3dOrientation(20, 20, .01f);
        OrientationLines orientationLines = new OrientationLines(.2f, 1f);

        Texture2D textureForward;
        Texture2D textureRight;
        Texture2D textureUp;
        Texture2D crosshair;
        Vector2 crosshairLocation = new Vector2(370, 200);
        bool canshoot = true;
        int timeuntilnextbullet = 0;
        int uprecoil = -5;
        Random rnd = new Random();
        Vector3[] PositionsArray = new Vector3[2];
        bool positions = true;
        //shooting

        private BasicEffect basicEffect; 
        //shooting


        List<Bullet> bulletList = new List<Bullet>();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            beffect = new BasicEffect(GraphicsDevice);
            cam = new Basic3dExampleCamera(GraphicsDevice, Window);
            cam.Position = new Vector3(0, 1, 10);
            cam.Target = Vector3.Zero;

            textureForward = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Red, Color.Red);
            textureRight = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Yellow, Color.Yellow);
            textureUp = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Blue, Color.Blue);

            beffect.VertexColorEnabled = true;
            beffect.TextureEnabled = true;
            beffect.World = Matrix.Identity;
            beffect.Projection = cam.ProjectionMatrix;

            crosshair = Content.Load<Texture2D>(@"crosshair");

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = Matrix.CreateLookAt(new Vector3(50, 50, 50), new Vector3(0, 0, 0), Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            IsMouseVisible = false;

    }

        protected override void UnloadContent()
        {
            textureForward.Dispose();
            textureRight.Dispose();
            textureUp.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (positions)
            {
                PositionsArray[0] = cam.Position;
                positions = false;
            }
            else
            {
                PositionsArray[1] = cam.Position;
                positions = true;
            }
            MouseState state = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F12))
                graphics.ToggleFullScreen();

            cam.Update(gameTime);
            beffect.View = cam.ViewMatrix;

            camWorldObjectToVisualize = Matrix.CreateWorld(Vector3.One, cam.LookAtDirection, Vector3.Up);
            //shooting
            foreach(Bullet bullet in bulletList)
            {
                //bullet.Position = cam.bulletPosition;
                bullet.bulletsWorld = Matrix.CreateTranslation(bullet.Position);
            }

            base.Update(gameTime);

            if (state.LeftButton == ButtonState.Pressed && canshoot)
            {
                Bullet newBullet = new Bullet();
                newBullet.Position= cam.Position;
                newBullet.shooting = true;
                newBullet.Direction = cam.camerasWorld.Forward;
                newBullet.IsVisible = true;
                newBullet.model = Content.Load<Model>(@"Sphere");

                bulletList.Add(newBullet);
                canshoot = false;
                timeuntilnextbullet = 0;
                //start recoil code
                double random = rnd.Next(-2, 2);
                uprecoil = -5;
                if(PositionsArray[0] != PositionsArray[1]) // if moving
                {
                    random = random * 4;
                    uprecoil = -10;
                }
                if (cam.crouching)
                {
                    random = random / 2;
                    uprecoil = -2;
                }
                else if (cam.jumping)
                {
                    random = random * 6;
                    uprecoil = -15;
                }

                int leftorright = Convert.ToInt32(Math.Round(random));
                cam.RotateUpOrDown(gameTime, uprecoil);       //upwards recoil is 5 when standing, 2 when crouching
                cam.RotateLeftOrRight(gameTime, leftorright);     //right recoil is random between 2 to the left and 2 to the right, halfed and then rounded when crouching
                //end recoil code
            }
            if (!canshoot)
            {
                timeuntilnextbullet++;
                if (timeuntilnextbullet >= 5)  //firerate
                    canshoot = true;
            }


            foreach(Bullet bullet in bulletList)
            {
                if(bullet.shooting)
                {
                    if (bullet.bulletCount >= 20)
                        bullet.shooting = false;

                    bullet.bulletsWorld = Matrix.CreateTranslation(bullet.Position);
                    bullet.bulletsWorld = Matrix.CreateWorld(bullet.bulletsWorld.Translation, bullet.bulletsWorld.Forward, Vector3.Up);
                    bullet.Position += (bullet.Direction * bullet.Velocity) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    bullet.bulletCount++;
                }
                else
                {
                    bullet.IsVisible = false;
                }
            }
            foreach(Bullet bullet in bulletList)
            {
                bullet.bulletsWorld = Matrix.CreateTranslation(bullet.Position);
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue);

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            worldGrid.DrawWithBasicEffect(GraphicsDevice, beffect, Matrix.Identity, 30f, textureForward, textureRight, textureUp);

            //orientationLines.DrawWithBasicEffect(GraphicsDevice, beffect, camWorldObjectToVisualize);
            orientationLines.DrawWithBasicEffect(GraphicsDevice, beffect, Matrix.Identity);
            
            spriteBatch.Begin();
            spriteBatch.Draw(crosshair, crosshairLocation, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);

            //shooting
            foreach(Bullet bullet in bulletList)
            {
                if (bullet.IsVisible)
                foreach (ModelMesh mesh in bullet.model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = bullet.bulletsWorld;
                        effect.View = cam.ViewMatrix;
                        effect.Projection = cam.ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }

        }

        // this just makes a texture so it doesn't have to be loaded.
        public static Texture2D CreateCheckerBoard(GraphicsDevice device, int w, int h, Color c0, Color c1)
        {
            Color[] data = new Color[w * h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int index = y * w + x;
                    Color c = c0;
                    if ((y % 2 == 0))
                    {
                        if ((x % 2 == 0))
                            c = c0;
                        else
                            c = c1;
                    }
                    else
                    {
                        if ((x % 2 == 0))
                            c = c1;
                        else
                            c = c0;
                    }
                    data[index] = c;
                }
            }
            Texture2D tex = new Texture2D(device, w, h);
            tex.SetData<Color>(data);
            return tex;
        }
    }
}

