using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Textures
{
    internal class Texture
    {
        public string Path
        {
            get => path;
        }

        public bool Loaded
        {
            get => loaded;
        }

        private string path;
        private bool loaded;

        private int width;
        private int height;
        private byte[] pixels;
        private int textureId;
        

        public Texture(string path)
        {
            this.path = path;
            textureId = -1;
            loaded = Load();
        }

        private bool Load()
        {
            if(File.Exists(path))
            {
                using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(File.Open(path, FileMode.Open)))
                {
                    //get image properties
                    width = image.Width;
                    height = image.Height;
                    //get pixels, 4 bytes per pixel
                    pixels = new byte[width * height * 4];

                    //index of where we add the pixel data
                    int pixelIndex = 0;
                    //loop through image data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //get all pixel bytes
                            pixels[pixelIndex] = image[x, y].R;
                            pixels[pixelIndex + 1] = image[x, y].G;
                            pixels[pixelIndex + 2] = image[x, y].B;
                            pixels[pixelIndex + 3] = image[x, y].A;
                            //increment pixel index
                            pixelIndex += 4;
                        }
                    }
                }

                textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.4f);
                Console.WriteLine($"Texture {path} loaded succesfully!");

                return true;
            }

            Console.WriteLine($"Failed to load texture {path}!");
            return false;
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
