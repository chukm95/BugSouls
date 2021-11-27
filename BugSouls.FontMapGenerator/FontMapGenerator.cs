using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.FontMapGenerator
{
    internal class FontMapGenerator
    {
        public static readonly string defaultChars = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        public static readonly float padding = 5;

        private struct FontChar
        {
            public char c;
            public SizeF size;
            public float offset;
        }

        public static void GenerateFontMap(string file, int size)
        {
            if(!File.Exists(file))
            {
                string[] fs = Directory.GetFiles("./");
                foreach(string f in fs)
                    Console.WriteLine(f);
                Console.WriteLine($"File {file} doesn't exist!");
                return;
            }    

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(file);

            FontChar[] chars = new FontChar[defaultChars.Length];
            Font font = new Font(pfc.Families[0], size);
            SizeF totalSize;

            StringFormat sf = new StringFormat(StringFormatFlags.NoClip);
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near;

            using (Bitmap bmp = new Bitmap(1, 1))
            {
                using(Graphics g = Graphics.FromImage(bmp))
                {
                    totalSize = g.MeasureString(defaultChars, font);
                    totalSize.Width = 0;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                    for (int i = 0; i < defaultChars.Length; i++)
                    {
                        SizeF charSize = g.MeasureString(defaultChars[i].ToString(), font, (int)size, sf);
                        
                        chars[i].c = defaultChars[i];
                        chars[i].size = charSize;
                        chars[i].offset = (int)(totalSize.Width + 0.5f);
                        Console.WriteLine($"char {defaultChars[i]} width = {(int)(charSize.Width + 0.5f)}");

                        totalSize.Width += (int)(charSize.Width + 0.5f);
                    }
                }
            }

            using (Bitmap bmp = new Bitmap((int)(totalSize.Width + 1f), (int)(totalSize.Height + 1f) * 2))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    float offset = 0;
                    foreach (FontChar fc in chars)
                    {
                        g.DrawString(fc.c.ToString(), font, Brushes.White, new PointF(offset, 0), sf);
                        offset += fc.size.Width;
                    }
                    g.DrawString(defaultChars, font, Brushes.White, new PointF(0, totalSize.Height), sf);
                }
                bmp.Save("./generated.png", ImageFormat.Png);
            }
        }
    }
}
