// some grids and stuff to see get some bearings.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CamTest
{

    public class Grid3dOrientation
    {
        public Grid3d gridForward;
        public Grid3d gridRight;
        public Grid3d gridUp;
        

        /// <summary>
        /// Draws 3 3d grids, linewith should be very small like .001
        /// </summary>
        public Grid3dOrientation(int x, int y, float lineWidth)
        {
            gridForward = new Grid3d(x, y, lineWidth, true, 0);
            gridRight = new Grid3d(x, y, lineWidth, true, 1);
            gridUp = new Grid3d(x, y, lineWidth, true, 2);
        }

        /// <summary>
        /// Draws this world grid with basic effect.
        /// </summary>
        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect, Matrix world, float scale, Texture2D forwardTexture, Texture2D upTexture, Texture2D rightTexture)
        {
            // Draw a 3d full orientation grid
            //gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            effect.World = Matrix.CreateScale(scale) * world;
            bool isLighting = effect.LightingEnabled;
            effect.LightingEnabled = false;
            effect.Texture = upTexture;
            gridForward.DrawWithBasicEffect(gd, effect);
            effect.Texture = forwardTexture;
            gridRight.DrawWithBasicEffect(gd, effect);
            effect.Texture = rightTexture;
            gridUp.DrawWithBasicEffect(gd, effect);
            if (isLighting)
                effect.LightingEnabled = true;
        }

        /// <summary>
        /// The method expects that the shader can accept a parameter named TextureA.
        /// </summary>
        public void Draw(GraphicsDevice gd, Effect effect, Texture2D forwardTexture, Texture2D upTexture, Texture2D rightTexture)
        {
            // Draw a 3d full orientation grid
            gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            effect.Parameters["TextureA"].SetValue(upTexture);
            gridForward.Draw(gd, effect);
            effect.Parameters["TextureA"].SetValue(forwardTexture);
            gridRight.Draw(gd, effect);
            effect.Parameters["TextureA"].SetValue(rightTexture);
            gridUp.Draw(gd, effect);
        }

        public void Draw(GraphicsDevice gd, Effect effect, int part0to2)
        {
            if (part0to2 == 0)
            {
                gridForward.Draw(gd, effect);
            }
            else
            {
                if (part0to2 == 1)
                    gridRight.Draw(gd, effect);
                else
                    gridUp.Draw(gd, effect);
            }
        }
    }

    public class Grid3d
    {
        int width;
        int height;
        public VertexPositionTexture[] vertices;
        public int[] indices;

        /// <summary>
        /// Creates a grid for 3d modelspace.
        /// The Width Height is doubled into negative and positive.
        /// linesize should be a very small value less then 1;
        /// flip options range from 0 to 2
        /// </summary>
        public Grid3d(int rows, int columns, float lineSize, bool centered, int flipOption)
        {
            rows *= 1;
            columns *= 1;
            Vector3 centerOffset = Vector3.Zero;
            if (centered)
                centerOffset = new Vector3(-.5f, -.5f, 0f);
            width = rows;
            height = columns;
            int len = width * 4 + height * 4;
            float xratio = 1f / width;
            float yratio = 1f / height;
            vertices = new VertexPositionTexture[len];
            indices = new int[(width * 6 + height * 6) * 2];
            int vIndex = 0;
            int iIndex = 0;
            for (int x = 0; x < width; x++)
            {
                int svIndex = vIndex;
                Vector3 xpos = new Vector3(xratio * x, 0f, 0f);
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(0f, 0f, 0f) + xpos + centerOffset,
                    new Vector2(0f, 0f));
                vIndex++;
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(0f, 1f, 0f) + xpos + centerOffset,
                    new Vector2(0f, 1f));
                vIndex++;
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(lineSize, 0f, 0f) + xpos + centerOffset,
                    new Vector2(1f, 0f));
                vIndex++;
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(lineSize, 1f, 0f) + xpos + centerOffset,
                    new Vector2(1f, 1f));
                vIndex++;
                // triangle 1
                indices[iIndex + 0] = svIndex + 0; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 2;
                // triangle 2
                indices[iIndex + 3] = svIndex + 2; indices[iIndex + 4] = svIndex + 1; indices[iIndex + 5] = svIndex + 3;
                // triangle 3 backface
                indices[iIndex + 0] = svIndex + 2; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 0;
                // triangle 4 backface
                indices[iIndex + 3] = svIndex + 3; indices[iIndex + 4] = svIndex + 2; indices[iIndex + 5] = svIndex + 1;
                iIndex += 6 * 2;
            }
            for (int y = 0; y < height; y++)
            {
                int svIndex = vIndex;
                Vector3 ypos = new Vector3(0f, yratio * y, 0f);
                vertices[vIndex] = new VertexPositionTexture(new Vector3(0f, 0f, 0f) + ypos + centerOffset, new Vector2(0f, 0f)); vIndex++;
                vertices[vIndex] = new VertexPositionTexture(new Vector3(0f, lineSize, 0f) + ypos + centerOffset, new Vector2(0f, 1f)); vIndex++;
                vertices[vIndex] = new VertexPositionTexture(new Vector3(1f, 0f, 0f) + ypos + centerOffset, new Vector2(1f, 0f)); vIndex++;
                vertices[vIndex] = new VertexPositionTexture(new Vector3(1f, lineSize, 0f) + ypos + centerOffset, new Vector2(1f, 1f)); vIndex++;
                // triangle 1
                indices[iIndex + 0] = svIndex + 0; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 2;
                // triangle 2
                indices[iIndex + 3] = svIndex + 2; indices[iIndex + 4] = svIndex + 1; indices[iIndex + 5] = svIndex + 3;
                // triangle 3 backface
                indices[iIndex + 0] = svIndex + 2; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 0;
                // triangle 4 backface
                indices[iIndex + 3] = svIndex + 3; indices[iIndex + 4] = svIndex + 2; indices[iIndex + 5] = svIndex + 1;
                iIndex += 6 * 2;
            }
            Flip(flipOption);
        }

        void Flip(int flipOption)
        {
            if (flipOption == 1)
            {
                int index = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(0f, p.X, p.Y);
                        index++;
                    }
                }
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(0f, p.X, p.Y);
                        index++;
                    }
                }
            }
            if (flipOption == 2)
            {
                int index = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(p.Y, 0f, p.X);
                        index++;
                    }
                }
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(p.Y, 0f, p.X);
                        index++;
                    }
                }
            }
        }

        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect)
        {
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = false;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionTexture.VertexDeclaration);
            }
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionTexture.VertexDeclaration);
            }
        }
    }

    public class OrientationLines
    {
        VertexPositionColor[] vertices;
        int[] indices;

        public OrientationLines()
        {
            CreateOrientationLines(.1f, 1.0f);
        }
        public OrientationLines(float linewidth, float lineDistance)
        {
            CreateOrientationLines(linewidth, lineDistance);
        }

        private void CreateOrientationLines(float linewidth, float lineDistance)
        {
            var center = new Vector3(0, 0, 0);
            var scaledup = Vector3.Up * linewidth;
            var scaledforward = Vector3.Forward * linewidth;
            var forward = Vector3.Forward * lineDistance;
            var right = Vector3.Right * lineDistance;
            var up = Vector3.Up * lineDistance;

            var r = new Color(1.0f, 0.0f, 0.0f, .8f);
            var g = new Color(0.0f, 1.0f, 0.0f, .8f);
            var b = new Color(0.0f, 0.0f, 1.0f, .8f);

            vertices = new VertexPositionColor[9];
            indices = new int[18];

            // forward
            vertices[0].Position = forward; vertices[0].Color = g;
            vertices[1].Position = scaledup; vertices[1].Color = g;
            vertices[2].Position = center; vertices[2].Color = g;

            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 0; indices[4] = 2; indices[5] = 1;

            // right
            vertices[3].Position = right; vertices[3].Color = b;
            vertices[4].Position = scaledup; vertices[4].Color = b;
            vertices[5].Position = center; vertices[5].Color = b;

            indices[6] = 3; indices[7] = 4; indices[8] = 5;
            indices[9] = 3; indices[10] = 5; indices[11] = 4;

            // up square
            vertices[6].Position = up; vertices[6].Color = r;
            vertices[7].Position = center; vertices[7].Color = r;
            vertices[8].Position = scaledforward; vertices[8].Color = r;

            indices[12] = 6; indices[13] = 7; indices[14] = 8;
            indices[15] = 6; indices[16] = 8; indices[17] = 7;
        }

        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect, Matrix world)
        {
            effect.World = world;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColor.VertexDeclaration);
            }
        }
        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColor.VertexDeclaration);
            }
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColor.VertexDeclaration);
            }
        }
    }
}