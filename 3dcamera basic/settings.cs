//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace CamTest
//{
//    class settings
//    {
//        public SpriteFont spriteFont16;
//        public SpriteFont spriteFont50;
//        public SpriteBatch spriteBatch;
//        private Game1 g1;
//        private Basic3dExampleCamera cam;
//        private bool open = false;
//        public Texture2D SquareTexture;
//        public Texture2D FinalSquareTexture;
//        private Rectangle Square1 = new Rectangle(new Point(398, 242), new Point(2)); //crosshair
//        private Rectangle Square2 = new Rectangle(new Point(401, 245), new Point(2)); //crosshair
//        private Rectangle Square3 = new Rectangle(new Point(401, 240), new Point(2)); //crosshair
//        private Rectangle Square4 = new Rectangle(new Point(404, 242), new Point(2)); //crosshair
//        private Rectangle SensSquare = new Rectangle(new Point(560, 85), new Point(40, 25));
//        private Rectangle SliderSquare;
//        private Rectangle FinalSquare = new Rectangle(new Point(0, 0), new Point(800));
//        private Rectangle Slider = new Rectangle(new Point(250, 100), new Point(300, 2));
//        public bool GameOver;

//        public void Initialise()
//        {
//            spriteFont16 = g1.Content.Load<SpriteFont>(@"16");
//            spriteFont50 = g1.Content.Load<SpriteFont>(@"50");
//            spriteBatch = new SpriteBatch(g1.GraphicsDevice);
//        }
//        public void LoadContent()
//        {
//            g1 = new Game1();
//            cam = new Basic3dExampleCamera(g1.GraphicsDevice);
//        }
//        public void Update()
//        {
//            GameOver = g1.GameOver;
//            open = cam.settings;
//            if (open)
//            {
//                if (Mouse.GetState().Position.Y > 85 && Mouse.GetState().Position.Y < 110 && Mouse.GetState().LeftButton == ButtonState.Pressed)
//                    SliderSquare = new Rectangle(new Point(Mouse.GetState().Position.X, 95), new Point(6, 10));
//                if (SliderSquare.X < 250)
//                    SliderSquare.X = 250;
//                if (SliderSquare.X > 550)
//                    SliderSquare.X = 550;
//                cam.sensitivity = 20f;//(float)calculateSensitivity();
//            }
//        }
//        public void Draw()
//        {
//            GameOver = g1.GameOver;
//            spriteBatch.Begin(); //hud
//            if (GameOver == false)
//            {
//                spriteBatch.Draw(SquareTexture, Square1, Color.White);//crosshair
//                spriteBatch.Draw(SquareTexture, Square2, Color.White);//crosshair
//                spriteBatch.Draw(SquareTexture, Square3, Color.White);//crosshair
//                spriteBatch.Draw(SquareTexture, Square4, Color.White);//crosshair
//                spriteBatch.DrawString(spriteFont16, "Accuracy: " + g1.Accuracy + "% ", new Vector2(5, 10), Color.White);
//                spriteBatch.DrawString(spriteFont16, "Targets Hit: " + g1.TargetsHit, new Vector2(5, 30), Color.White);
//                spriteBatch.DrawString(spriteFont16, "Shots Fired: " + g1.ShotsFired, new Vector2(5, 50), Color.White);
//                spriteBatch.DrawString(spriteFont16, "XYZ: " + cam.Position, new Vector2(5, 70), Color.White);
//                spriteBatch.DrawString(spriteFont16, "Dash Ready? " + cam.dashReady, new Vector2(5, 90), Color.White);
//                spriteBatch.DrawString(spriteFont16, "AverageTime: " + g1.averagetime, new Vector2(5, 110), Color.White);
//                spriteBatch.DrawString(spriteFont16, "Score: " + g1.score, new Vector2(5, 130), Color.White);
//            }
//            else
//            {
//                spriteBatch.Draw(FinalSquareTexture, FinalSquare, Color.Black);
//                spriteBatch.DrawString(spriteFont50, "Game Over", new Vector2(220, 50), Color.White);
//                spriteBatch.DrawString(spriteFont50, "Final Score: " + g1.finalScore, new Vector2(220, 210), Color.White);
//            }
//            if (cam.settings)
//            {
//                spriteBatch.Draw(FinalSquareTexture, FinalSquare, Color.Black);
//                spriteBatch.DrawString(spriteFont50, "Settings", new Vector2(275, 0), Color.White);
//                spriteBatch.DrawString(spriteFont16, "Sensitivity:", new Vector2(150, 90), Color.White);
//                spriteBatch.Draw(SquareTexture, Slider, Color.White);
//                spriteBatch.Draw(SquareTexture, SliderSquare, Color.White);
//                spriteBatch.Draw(SquareTexture, SensSquare, Color.White);
//                spriteBatch.DrawString(spriteFont16, "" + cam.sensitivity, new Vector2(562, 86), Color.Blue);
//            }

//            spriteBatch.End();
//        }
//        private double calculateSensitivity()
//        {
//            return 20f;//((((double)SliderSquare.X - 250) / 5) + 1) / 60;
//        }
//    }
//}
