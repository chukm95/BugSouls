using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Fonts
{
    internal class FontMap
    {
        public const string defaultChars = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        
        public FontChar this[char c]
        {
            get
            {
                if(characters.ContainsKey(c))
                {
                    return characters[c];
                }
                else
                {
                    return new FontChar();
                }
            }
        }

        public bool Loaded
        {
            get => loaded;
        }

        public int Size
        {
            get => size;
        }

        public int Padding
        {
            get => padding;
        }

        private string name;
        private string path;
        private int size;
        private int padding;
        Dictionary<char, FontChar> characters;
        private int textureId;
        private bool loaded;

        public FontMap(string name, string path, int size)
        {
            this.name = name;
            this.path = path;
            this.size = size;
            padding = 2;
            characters = new Dictionary<char, FontChar>();
            textureId = -1;
            loaded = LoadFont();
        }

        private bool LoadFont()
        {
            //check if the file exists
            if (!File.Exists(path))
                return false;

            FontChar[] characters = new FontChar[defaultChars.Length];
            SKRect[] bounds;
            float[] advances;
            int height = padding;
            int totalWidth = padding;
            byte[] data;

            int imageWidth;
            int imageHeight;

            using (SKFont skFont = new SKFont(SKTypeface.FromFile(path), size))
            {
                using (SKPaint skPaint = new SKPaint(skFont))
                {
                    skPaint.TextAlign = SKTextAlign.Left;
                    skPaint.LcdRenderText = false;
                    skPaint.FakeBoldText = true;
                    skPaint.IsAntialias = false;
                    skPaint.Color = new SKColor(255, 255, 255, 255);
                    advances = skPaint.GetGlyphWidths(defaultChars, out bounds);
                    for (int i = 0; i < defaultChars.Length; i++)
                    {
                        SKRect sr = bounds[i];
                        characters[i] = new FontChar()
                        {
                            character = defaultChars[i],
                            offsetX = sr.Location.X,
                            offsetY = sr.Location.Y,
                            advance = advances[i],
                            width = sr.Width,
                            height = sr.Height,
                            top = sr.Top,
                            bottom = sr.Bottom,
                            left = sr.Left,
                            right = sr.Right,
                            tex_x = totalWidth,
                            tex_y = padding,
                            tex_z = sr.Width,
                            tex_w = sr.Height
                        };


                        if (height < (int)Math.Ceiling(sr.Height))
                            height = (int)Math.Ceiling(sr.Height);
                       
                        totalWidth += (int)Math.Ceiling(sr.Width) + padding;
                    }

                    using (SKBitmap skBitmap = new SKBitmap(totalWidth + padding, size + padding, SKColorType.Rgba8888, SKAlphaType.Opaque))
                    {                        
                        using (SKCanvas skCanvas = new SKCanvas(skBitmap))
                        {                            
                            skPaint.Color = new SKColor(255, 255, 255, 255);
                            float offset = padding;
                            skCanvas.Clear(new SKColor(0, 0, 0, 0));

                            float tex_coord_width = 1f / (float)skBitmap.Width;
                            float tex_coord_height = 1f / (float)skBitmap.Height;

                            for (int i = 0; i < defaultChars.Length; i++)
                            {
                                FontChar fc = characters[i];
                                fc.pxCoords = new Vector4(fc.tex_x, padding, fc.tex_z, fc.tex_w);
                                fc.texCoords = fc.pxCoords * new Vector4(tex_coord_width, tex_coord_height, tex_coord_width, tex_coord_height);
                                this.characters.Add(fc.character, fc);
                                skCanvas.DrawText(defaultChars[i].ToString(), offset - fc.offsetX, (padding + Math.Abs(fc.top)), skPaint);
                                offset += (int)Math.Ceiling(bounds[i].Width) + padding;
                            }

                        }
                        imageWidth = skBitmap.Width;
                        imageHeight = skBitmap.Height;
                        data = skBitmap.Bytes;
                    }
                }
            }

            textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, imageWidth, imageHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            return true;
        }

        public Vector4 MeassureString(string text)
        {
            float widthTotal = 0;
            float heightTotal = size + padding;
            float widthLine = 0;
            int advance = 0;

            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                FontChar fc = this[c];
                if(c!='\n')
                {
                    widthLine = advance + fc.width + padding + padding;
                    advance += (int)fc.advance;
                }
                else if(c=='\n')
                {
                    //widthLine += padding;
                    if(widthLine >= widthTotal)
                    {
                        widthTotal = widthLine;
                    }
                    widthLine = 0;
                    advance = 0;
                    heightTotal += size + padding;
                }
            }

            widthLine += padding;
            if (widthLine >= widthTotal)
            {
                widthTotal = widthLine;
            }

            return new Vector4(-padding, size - padding - padding, widthTotal, heightTotal + padding);
        }

        public void BindTexture(int texUnit)
        {
            GL.BindTextureUnit(texUnit, textureId);
        }

        public void Dispose()
        {
            GL.DeleteTexture(textureId);
        }
    }
}
