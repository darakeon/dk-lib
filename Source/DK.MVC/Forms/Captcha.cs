using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace DK.MVC.Forms
{
    public class Captcha
    {
        public Captcha(Boolean newText)
        {
            if (newText || String.IsNullOrEmpty(text))
            {
                generateString();
            }
        }



        public String Image
        {
            get { return generateImage(text); }
        }



        public bool ValidateAndRenew(String typed)
        {
            var valid = typed == text;

            generateString();

            return valid;
        }



        private static HttpSessionState session
        {
            get { return HttpContext.Current.Session; }
        }

        private static string text
        {
            get { return session["Captcha"] != null ? session["Captcha"].ToString() : null; }
        }



        private void generateString()
        {
            var random = new Random();
            var captcha = String.Empty;

            for (var i = 0; i < 6; i++)
            {
                var positionNewChar = random.Next(0, possibleCharacters.Length);

                captcha += possibleCharacters[positionNewChar];
            }

            session.Add("Captcha", captcha);
        }



        private String generateImage(String captcha)
        {
            using (var bitmap = new Bitmap(imageWidth, imageHeight))
            {
                drawCaptcha(captcha, bitmap);

                var filename = Guid.NewGuid() + ".png";
                var dir = Path.Combine("Assets", "Images", "Generated", "Captcha");
                var path = Path.Combine(dir, filename);

                bitmap.Save(path, ImageFormat.Png);

                cleanOldFiles(dir);

                return path;
            }
        }

        private void drawCaptcha(String captcha, Image bitmap)
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(imageColor);
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                using (var font = new Font(fontType, fontSize, FontStyle.Bold))
                {
                    graphics.DrawString(captcha, font, Brushes.White, textTop, textLeft);
                }
            }
        }

        private static void cleanOldFiles(String dir)
        {
            var oldFiles = Directory.GetFiles(dir, "*.png");

            foreach (var file in oldFiles)
            {
                var info = new FileInfo(file);
                var lastHour = DateTime.Now.AddHours(-1);

                var fileOld =
                    info.CreationTime < lastHour
                    || info.LastWriteTime < lastHour;

                if (fileOld)
                {
                    File.Delete(file);
                }
            }
        }

        #region Config

        private readonly String[] possibleCharacters =
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
            "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };

        private const String fontType = "Times New Roman";
        private const Int32 fontSize = 17;

        private readonly Color imageColor = Color.FromArgb(33, 140, 0);
        private const Int32 imageWidth = 100;
        private const Int32 imageHeight = 30;

        private const Int32 textTop = 3;
        private const Int32 textLeft = 3;

        #endregion


    }
}
