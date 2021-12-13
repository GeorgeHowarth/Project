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
        private float TargetsHit = 0;
        private float ShotsFired = 0;
        private float Accuracy;
        private SpriteFont spriteFont16;
        private SpriteFont spriteFont50;
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private BasicEffect beffect;
        private Basic3dExampleCamera cam;
        private Matrix camWorldObjectToVisualize = Matrix.Identity;
        private Texture2D SquareTexture;
        private Texture2D FinalSquareTexture;
        private Rectangle Square1 = new Rectangle(new Point(398, 242), new Point(2)); //crosshair
        private Rectangle Square2 = new Rectangle(new Point(401, 245), new Point(2)); //crosshair
        private Rectangle Square3 = new Rectangle(new Point(401, 240), new Point(2)); //crosshair
        private Rectangle Square4 = new Rectangle(new Point(404, 242), new Point(2)); //crosshair
        private Rectangle FinalSquare = new Rectangle(new Point(0, 0), new Point(5000000));
        private readonly Grid3dOrientation worldGrid = new Grid3dOrientation(20, 20, .01f);
        private readonly OrientationLines orientationLines = new OrientationLines(.2f, 1f);
        private Texture2D textureForward;
        private Texture2D textureRight;
        private Texture2D textureUp;
        private bool canshoot = true;
        private int timeuntilnextbullet = 0;
        private int uprecoil;
        private readonly Random rnd = new Random();
        private bool temp = true;
        private BasicEffect basicEffect;
        private int score = 0;
        private double averagetime = 0;
        private int totaltime;
        private bool GameOver = false;
        private int finalScore;
        //shooting


        private readonly List<Bullet> bulletList = new List<Bullet>();
        private readonly Target[] Buttons = new Target[4];
        private readonly List<Target> targetList = new List<Target>();
        private int targetCount = 0;

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
            startButton.Position = new Vector3(0,3,-10);
            startButton.model = Content.Load<Model>(@"start");
            Buttons[0] = startButton;

            Target settingsButton = new Target();
            //creates the settings button
            settingsButton.Position = new Vector3(-10, 3, -10);
            settingsButton.model = Content.Load<Model>(@"settings");
            Buttons[1] = settingsButton;

            Target quitButton = new Target();
            //creates the quit button
            quitButton.Position = new Vector3(10, 3, -10);
            quitButton.model = Content.Load<Model>(@"quit");
            Buttons[2] = quitButton;

            Target room = new Target();
            //placeholder for floor at actual game area
            room.IsVisible = true;
            room.Position = new Vector3(100, -25, 0);
            room.model = Content.Load<Model>(@"cube");
            Buttons[3] = room;

            spriteFont16 = Content.Load<SpriteFont>(@"16");
            spriteFont50 = Content.Load<SpriteFont>(@"50");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            beffect = new BasicEffect(GraphicsDevice);
            cam = new Basic3dExampleCamera(GraphicsDevice);
            cam.Position = new Vector3(0, 1, 10);
            cam.Target = Vector3.Zero;

            SquareTexture = new Texture2D(GraphicsDevice, 1, 1);
            SquareTexture.SetData(new Color[] { Color.White });
            FinalSquareTexture = new Texture2D(GraphicsDevice, 1, 1);
            FinalSquareTexture.SetData(new Color[] { Color.Black });

            textureForward = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Red, Color.Red);      // creates red X CheckerBoard
            textureRight = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Yellow, Color.Yellow);  // creates yellow Y CheckerBoard
            textureUp = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.Blue, Color.Blue);         // creates blue Z CheckerBoard

            beffect.VertexColorEnabled = true;
            beffect.TextureEnabled = true;
            beffect.World = Matrix.Identity;
            beffect.Projection = cam.ProjectionMatrix;

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
            if(Keyboard.GetState().IsKeyDown(Keys.M) && temp)
            {
                GameOver = !GameOver;
                finalScore = Convert.ToInt32(score * (Accuracy / 100) * (averagetime / 5));
                temp = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.M) && !temp)
                temp = true;

            totaltime++;
            MouseState state = Mouse.GetState();
            if (targetCount < 5)
            {
                CreateNewTarget();
                targetCount++;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.K)) //reset targets
            {
                targetList.Clear();
                targetCount = 0;
                temp = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L) && temp)
            {
                targetList.RemoveAt(0);
                targetCount--;
                temp = false;
            }
            //if (Keyboard.GetState().IsKeyUp(Keys.L) && !temp)
            //    temp = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                cam.Position = new Vector3(0, 2, 10);
            
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
                double random = rnd.Next(-2, 5);
                uprecoil = -2;
                if (cam.moving) // if moving, recoil is increased
                {
                    random *= 2;
                    uprecoil = -6;
                }
                if (cam.crouching)    //if crouching recoil is reduced
                {
                    random /= 2;
                    uprecoil = -1;
                }
                else if (cam.jumping)  //if jumping recoil is massively increased
                {
                    random *= 3;
                    uprecoil = -10;
                }

                int leftorright = Convert.ToInt32(Math.Round(random));
                cam.RotateUpOrDown(gameTime, uprecoil);       //upwards recoil is 5 when standing
                cam.RotateLeftOrRight(gameTime, leftorright);     //right recoil is random between 2 to the left and 2 to the right but changed based on movement
                                                                      //end recoil code
               ShotsFired++;
               Accuracy = UpdateAccuracy(ShotsFired, TargetsHit);
            }
            if (!canshoot)
            {
                timeuntilnextbullet++;
                if (timeuntilnextbullet >= 7.5f)  //firerate
                    canshoot = true;
            }

            UpdateObjects(gameTime);
            // button "collision"
            foreach (Bullet bullet in bulletList)
            {
                //start
                if (bullet.bulletPosition.X > -1 && bullet.bulletPosition.X < 3 && bullet.bulletPosition.Y > 1 && bullet.bulletPosition.Y < 4 && bullet.Position.Z > -12 && bullet.Position.Z < -8 && bullet.IsVisible) //placeholder for collision
                {
                    cam.Position = new Vector3(100, 2, 0); // starts the game
                    bullet.IsVisible = false;
                    TargetsHit = 0;
                    ShotsFired = 0;
                    totaltime = 0;
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
            foreach (Bullet bullet in bulletList)
            {
                if (bullet.IsVisible)
                {
                    foreach (Target target in targetList)
                    {
                        if (target.IsVisible && Vector3.Distance(bullet.bulletPosition, target.targetPosition) < 1.5f)  //checks if bullet is within a circle of radius 2 and if so removes the target and spawns a new one
                        {
                            target.IsVisible = false;
                            bullet.IsVisible = false;
                            targetCount--;
                            TargetsHit++;
                            Accuracy = UpdateAccuracy(ShotsFired, TargetsHit);
                            score += 100 * (Math.Abs(target.leftright) + Math.Abs(target.backforward) / 2);
                            averagetime = UpdateAverageTime(totaltime, TargetsHit);
                        }
                    }
                }
            }
            
        }

        public void UpdateObjects(GameTime gt) 
        {
            foreach (Bullet bullet in bulletList)               //this piece of code handles how each bullet moves
            {
                if (bullet.shooting)
                {
                    if (bullet.bulletCount >= 55)
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
            foreach (Target target in targetList)               //this piece of code handles how each target moves
            {
                if (target.IsVisible)
                {
                    target.targetsWorld = Matrix.CreateTranslation(target.Position);
                    target.targetsWorld = Matrix.CreateWorld(target.targetsWorld.Translation, target.targetsWorld.Forward, Vector3.Up);
                    if (target.Counter < 120)
                    {
                        target.Position += (new Vector3(target.leftright,0,target.backforward)* 1.5f) * (float)gt.ElapsedGameTime.TotalSeconds;
                        target.Counter++;
                    }
                    else if (target.Counter < 240)
                    {
                        target.Position += (new Vector3(target.leftright,0,target.backforward) * -1.5f) * (float)gt.ElapsedGameTime.TotalSeconds;
                        target.Counter++;
                    }
                    else if (target.Counter >= 240)
                    {
                        target.Counter = 0;
                    }
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
            //ShotsFired++;
            Bullet newBullet = new Bullet();                         //creates a new bullet using the bullet class
            newBullet.Position = cam.Position;
            newBullet.Direction = cam.camerasWorld.Forward;
            newBullet.model = Content.Load<Model>(@"Sphere");

            bulletList.Add(newBullet);
        }
        public void CreateNewTarget()           //creates a new target using the target class
        {
            Target temp = new Target();
            temp.Position = new Vector3(rnd.Next(75, 125), rnd.Next(0, 10), rnd.Next(-25, 25));
            temp.model = Content.Load<Model>(@"globe-sphere");
            temp.leftright = rnd.Next(-10, 10);
            temp.backforward = rnd.Next(-10, 10);
            targetList.Add(temp);
        }

        public int UpdateAccuracy(float ShotsFired,float TargetsHit)
        {
            return (int)((TargetsHit / ShotsFired) * 100);
        }

        public double UpdateAverageTime(double averagetime, double temptime)
        {
            return ((averagetime + temptime) / TargetsHit) / 60;
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

            foreach (Target button in Buttons)
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
            spriteBatch.Begin(); //hud
            if(GameOver == false)
            {
                spriteBatch.Draw(SquareTexture, Square1, Color.White);//crosshair
                spriteBatch.Draw(SquareTexture, Square2, Color.White);//crosshair
                spriteBatch.Draw(SquareTexture, Square3, Color.White);//crosshair
                spriteBatch.Draw(SquareTexture, Square4, Color.White);//crosshair
                spriteBatch.DrawString(spriteFont16, "Accuracy: " + Accuracy + "% ", new Vector2(5, 10), Color.White);
                spriteBatch.DrawString(spriteFont16, "Targets Hit: " + TargetsHit, new Vector2(5, 30), Color.White);
                spriteBatch.DrawString(spriteFont16, "Shots Fired: " + ShotsFired, new Vector2(5, 50), Color.White);
                spriteBatch.DrawString(spriteFont16, "XYZ: " + cam.Position, new Vector2(5, 70), Color.White);
                spriteBatch.DrawString(spriteFont16, "Dash Ready? " + cam.dashReady, new Vector2(5, 90), Color.White);
                spriteBatch.DrawString(spriteFont16, "AverageTime: " + averagetime, new Vector2(5, 110), Color.White);
                spriteBatch.DrawString(spriteFont16, "Score: " + score, new Vector2(5, 130), Color.White);
            }
            else
            {
                spriteBatch.Draw(FinalSquareTexture, FinalSquare, Color.Black);
                spriteBatch.DrawString(spriteFont50, "Game Over", new Vector2(220, 50), Color.White);
                spriteBatch.DrawString(spriteFont50, "Final Score: " + finalScore, new Vector2(220, 210), Color.White);
            }

            spriteBatch.End();
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

