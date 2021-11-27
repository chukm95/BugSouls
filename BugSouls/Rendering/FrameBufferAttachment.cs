using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugSouls.Rendering
{
    internal class FrameBufferAttachment
    {
        public FramebufferAttachment FramebufferAttachment
        {
            get => framebufferAttachmentType;
        }

        public DrawBuffersEnum DrawBuffersEnum 
        { 
            get => drawBuffersEnum; 
        }

        private FramebufferAttachment framebufferAttachmentType;
        private DrawBuffersEnum drawBuffersEnum;
        private PixelInternalFormat internalPixelFormat;
        private PixelFormat pixelFormat;
        private PixelType pixelType;
        private FrameBuffer parentBuffer;
        private int textureId;        

        public FrameBufferAttachment(FramebufferAttachment framebufferAttachmentType, DrawBuffersEnum drawBuffersEnum, PixelInternalFormat internalPixelFormat, PixelFormat pixelFormat, PixelType pixelType)
        {
            this.framebufferAttachmentType = framebufferAttachmentType;
            this.drawBuffersEnum = drawBuffersEnum;
            this.internalPixelFormat = internalPixelFormat;
            this.pixelFormat = pixelFormat;
            this.pixelType = pixelType;
        }

        internal void Initialize(FrameBuffer parentBuffer)
        {
            //set the parentBuffer
            this.parentBuffer = parentBuffer;
            //generate the attachment texture
            textureId = GL.GenTexture();
            //Bind this generated texture
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            //now set the texture minification and magnification params
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //set the texture format
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalPixelFormat, parentBuffer.Width, parentBuffer.Height, 0, pixelFormat, pixelType, IntPtr.Zero);
            //add to the framebuffer
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, framebufferAttachmentType, textureId, 0);

        }

        internal void Resize()
        {
            //Bind this generated texture
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            //set the texture format
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalPixelFormat, parentBuffer.Width, parentBuffer.Height, 0, pixelFormat, pixelType, IntPtr.Zero);
        }

        public static void Bind(FrameBufferAttachment attachment, int unit)
        {
            GL.BindTextureUnit(unit, attachment.textureId);
        }

        public void Bind(int unit)
        {
            GL.BindTextureUnit(unit, textureId);
        }

        public void CleanUp()
        {
            GL.DeleteTexture(textureId);
        }
    }
}
