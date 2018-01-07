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
    /// <summary>
    /// Generate captach for form validations
    /// </summary>
    public class Captcha
    {
        /// <param name="newText">Which the text shoud be renegerated</param>
        public Captcha(Boolean newText)
        {
            if (newText || String.IsNullOrEmpty(text))
            {
                generateString();
            }
        }



        /// <summary>
        /// Captcha image generated
        /// </summary>
        public String Image => generateImage(text);


	    /// <summary>
	    /// Validate and change captach
	    /// </summary>
	    public bool ValidateAndRenew(String typed)
        {
            var valid = typed == text;

            generateString();

            return valid;
        }



        private static HttpSessionState session => HttpContext.Current.Session;
		private static string text => session["Captcha"]?.ToString();


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
            using (var bitmap = new Bitmap(image_width, image_height))
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

                using (var font = new Font(font_type, font_size, FontStyle.Bold))
                {
                    graphics.DrawString(captcha, font, Brushes.White, text_top, text_left);
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

        private const String font_type = "Times New Roman";
        private const Int32 font_size = 17;

        private readonly Color imageColor = Color.FromArgb(33, 140, 0);
        private const Int32 image_width = 100;
        private const Int32 image_height = 30;

        private const Int32 text_top = 3;
        private const Int32 text_left = 3;

        #endregion


    }
}
