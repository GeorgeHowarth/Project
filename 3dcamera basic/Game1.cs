using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        bool canshoot = true;
        int timeuntilnextbullet = 0;
        int uprecoil;
        Random rnd = new Random();

        private BasicEffect basicEffect; 
        //shooting


        List<Bullet> bulletList = new List<Bullet>();
        Target[] Buttons = new Target[4];
        List<Target> targetList = new List<Target>();
        int targetCount = 5;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            Target startButton = new Target();
            //creates the start button
            startButton.IsVisible = true;
            startButton.Position = new Vector3(0,3,-10);
            startButton.model = Content.Load<Model>(@"start");
            Buttons[0] = startButton;

            Target settingsButton = new Target();
            //creates the settings button
            settingsButton.IsVisible = true;
            settingsButton.Position = new Vector3(-10, 3, -10);
            settingsButton.model = Content.Load<Model>(@"settings");
            Buttons[1] = settingsButton;

            Target quitButton = new Target();
            //creates the quit button
            quitButton.IsVisible = true;
            quitButton.Position = new Vector3(10, 3, -10);
            quitButton.model = Content.Load<Model>(@"quit");
            Buttons[2] = quitButton;

            Target room = new Target();
            room.IsVisible = true;
            room.Position = new Vector3(100, -25, 0);
            room.model = Content.Load<Model>(@"cube");
            Buttons[3] = room;

            for(int i = 0; i<5; i++)
            {
                CreateNewTarget();
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            beffect = new BasicEffect(GraphicsDevice);
            cam = new Basic3dExampleCamera(GraphicsDevice, Window);
            cam.Position = new Vector3(0, 1, 10);
            cam.Target = Vector3.Zero;

            textureForward = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Red, Color.Red);      // creates red X CheckerBoard
            textureRight = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Yellow, Color.Yellow);  // creates yellow Y CheckerBoard
            textureUp = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Blue, Color.Blue);         // creates blue Z CheckerBoard

            beffect.VertexColorEnabled = true;
            beffect.TextureEnabled = true;
            beffect.World = Matrix.Identity;
            beffect.Projection = cam.ProjectionMatrix;

            //crosshair = Content.Load<Texture2D>(@"crosshair");

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
            if(targetCount <5)
            {
                CreateNewTarget();
            }

            MouseState state = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //if (Keyboard.GetState().IsKeyDown(Keys.F12))
            //    graphics.ToggleFullScreen();

            cam.Update(gameTime);
            beffect.View = cam.ViewMatrix;

            camWorldObjectToVisualize = Matrix.CreateWorld(Vector3.One, cam.LookAtDirection, Vector3.Up);
            //shooting
            base.Update(gameTime);

            if (state.LeftButton == ButtonState.Pressed && canshoot)
            {
                CreateNewBullet();
                canshoot = false;             //controls the fire rate
                timeuntilnextbullet = 0;
                //start recoil code
                double random = rnd.Next(-2, 2);
                uprecoil = -2;
                if(cam.moving) // if moving, recoil is increased
                {
                    random = random * 2;
                    uprecoil = -6;
                }
                if (cam.crouching)    //if crouching recoil is reduced
                {
                    random = random / 2;
                    uprecoil = -1;
                }
                else if (cam.jumping)  //if jumping recoil is massively increased
                {
                    random = random * 3;
                    uprecoil = -10;
                }

                int leftorright = Convert.ToInt32(Math.Round(random));
                cam.RotateUpOrDown(gameTime, uprecoil);       //upwards recoil is 5 when standing
                cam.RotateLeftOrRight(gameTime, leftorright);     //right recoil is random between 2 to the left and 2 to the right but changed based on movement
                //end recoil code
            }
            if (!canshoot)
            {
                timeuntilnextbullet++;
                if (timeuntilnextbullet >= 5)  //firerate
                    canshoot = true;
            }

            UpdateObjects(gameTime);

            foreach(Bullet bullet in bulletList)
            {
                //start
                if (bullet.bulletPosition.X > -1 && bullet.bulletPosition.X < 3 && bullet.bulletPosition.Y > 1 && bullet.bulletPosition.Y < 4 && bullet.Position.Z > -12 && bullet.Position.Z < -8 && bullet.IsVisible) //placeholder for collision
                {
                    cam.Position = new Vector3(100, 2, 0); // moves the player when "start" button hit
                    bullet.IsVisible = false;
                }
                //settings
                if (bullet.bulletPosition.X > -11 && bullet.bulletPosition.X < -7 && bullet.bulletPosition.Y > 1 && bullet.bulletPosition.Y < 4 && bullet.Position.Z > -12 && bullet.Position.Z < -8 && bullet.IsVisible) //placeholder for collision
                {
                    Buttons[1].IsVisible = !Buttons[1].IsVisible;
                    bullet.IsVisible = false;
                }
                //quit
                if (bullet.bulletPosition.X > 9 && bullet.bulletPosition.X < 13 && bullet.bulletPosition.Y > 1 && bullet.bulletPosition.Y < 4 && bullet.Position.Z > -12 && bullet.Position.Z < -8 && bullet.IsVisible) //placeholder for collision
                {
                    Exit(); //closes game when quit button hit
                }
            }
        }

        public void UpdateObjects(GameTime gt) 
        {
            foreach (Bullet bullet in bulletList)               //this piece of code handles how each bullet moves
            {
                if (bullet.shooting)
                {
                    if (bullet.bulletCount >= 20)
                        bullet.shooting = false;

                    bullet.bulletsWorld = Matrix.CreateTranslation(bullet.Position);
                    bullet.bulletsWorld = Matrix.CreateWorld(bullet.bulletsWorld.Translation, bullet.bulletsWorld.Forward, Vector3.Up);
                    bullet.Position += (bullet.Direction * bullet.Velocity) * (float)gt.ElapsedGameTime.TotalSeconds;
                    bullet.bulletCount++;
                }
                else
                {
                    bullet.IsVisible = false;
                }
            }
            foreach (Target button in Buttons)
            {
                button.targetsWorld = Matrix.CreateWorld(Matrix.CreateTranslation(button.Position).Translation, button.targetsWorld.Forward, Vector3.Up);
            }
            foreach (Target target in targetList)
            {
                target.targetsWorld = Matrix.CreateWorld(Matrix.CreateTranslation(target.Position).Translation, target.targetsWorld.Forward, Vector3.Up);
            }
        }

        public void CreateNewBullet()
        {
            Bullet newBullet = new Bullet();                         //creates a new bullet using the bullet class
            newBullet.Position = cam.Position;
            newBullet.shooting = true;
            newBullet.Direction = cam.camerasWorld.Forward;
            newBullet.IsVisible = true;
            newBullet.model = Content.Load<Model>(@"Sphere");

            bulletList.Add(newBullet);
        }
        public void CreateNewTarget()
        {
            Target temp = new Target();
            temp.IsVisible = true;
            temp.Position = new Vector3(rnd.Next(75, 125), rnd.Next(0, 10), rnd.Next(-25, 25));
            temp.model = Content.Load<Model>(@"globe-sphere");
            targetList.Add(temp);
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

            orientationLines.DrawWithBasicEffect(GraphicsDevice, beffect, Matrix.Identity);
            base.Draw(gameTime);

            //shooting

            foreach(Target button in Buttons)
            {
                if(button.IsVisible)
                foreach (ModelMesh mesh in button.model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = button.targetsWorld;
                        effect.View = cam.ViewMatrix;
                        effect.Projection = cam.ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
            foreach (Bullet bullet in bulletList)
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
            foreach (Target target in targetList)
            {
                if (target.IsVisible)
                    foreach (ModelMesh mesh in target.model.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.World = target.targetsWorld;
                            effect.View = cam.ViewMatrix;
                            effect.Projection = cam.ProjectionMatrix;
                        }
                        mesh.Draw();
                    }
            }
        }

        //creates the checkerboard pattern
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
            Texture2D tex = new Texture2D(device,w,h);
            tex.SetData<Color>(data);
            return tex;
        }
    }
}

