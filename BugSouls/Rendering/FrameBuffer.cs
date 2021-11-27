using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugSouls.Rendering
{
    internal class FrameBuffer
    {
        public int Width
        {
            get => width;
        }

        public int Height
        {
            get => height;
        }

        private int width;
        private int height;
        private FrameBufferAttachment[] attachments;
        private DrawBuffersEnum[] drawBuffersEnums;
        private int fboId;

        public FrameBuffer(int width, int height, params FrameBufferAttachment[] attachments)
        {
            //validate
            if (attachments == null || attachments.Length == 0)
                throw new ArgumentException("A framebuffer is expected to have atleast one attachment");

            //set framebuffer sizes
            this.width = width;
            this.height = height;

            //create a drawbuffersenum
            drawBuffersEnums = new DrawBuffersEnum[attachments.Length];

            //create a framebuffer object
            fboId = GL.GenFramebuffer();

            //bind the framebuffer target
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);

            //initialize
            for(int i = 0; i < attachments.Length; i++)
            {
                //init attachment to this fbo
                attachments[i].Initialize(this);
                drawBuffersEnums[i] = attachments[i].DrawBuffersEnum;
            }

            //unbind the framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //store all the attachments
            this.attachments = attachments;
        }

        public void ResizeWidth(int width)
        {
            Resize(width, height);
        }

        public void ResizeHeight(int height)
        {
            Resize(width, height);
        }

        public void Resize(int width, int height)
        {
            //set the new framebuffer dimensions
            this.width = width;
            this.height = height;
            //resize all attachments            
            for (int i = 0; i < attachments.Length; i++)
            {
                //resize all attachments
                attachments[i].Resize();
            }
        }        

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
            GL.DrawBuffers(attachments.Length, drawBuffersEnums);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void CleanUp()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            for (int i = 0; i < attachments.Length; i++)
            {
                //remove all attachments
                attachments[i].CleanUp();
            }
            GL.DeleteFramebuffer(fboId);
        }
    }
}
