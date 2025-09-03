using System.Drawing;
namespace CustomizableQrCode.Utils
{
    public static class ColorUtils
    {
        // Método ya existente
        public static bool IsContrastAccessible(string? fg, string? bg)
        {
            if (string.IsNullOrWhiteSpace(fg) || string.IsNullOrWhiteSpace(bg))
                return true;

            var fgColor = System.Drawing.ColorTranslator.FromHtml(fg);
            var bgColor = System.Drawing.ColorTranslator.FromHtml(bg);

            double contrast = GetContrastRatio(fgColor, bgColor);
            return contrast >= 4.5; // WCAG AA mínimo para texto normal
        }

        // Calcula el contraste
        private static double GetContrastRatio(System.Drawing.Color c1, System.Drawing.Color c2)
        {
            double l1 = GetRelativeLuminance(c1);
            double l2 = GetRelativeLuminance(c2);
            double brightest = Math.Max(l1, l2);
            double darkest = Math.Min(l1, l2);
            return (brightest + 0.05) / (darkest + 0.05);
        }

        private static double GetRelativeLuminance(System.Drawing.Color c)
        {
            double R = c.R / 255.0;
            double G = c.G / 255.0;
            double B = c.B / 255.0;

            R = (R <= 0.03928) ? R / 12.92 : Math.Pow((R + 0.055) / 1.055, 2.4);
            G = (G <= 0.03928) ? G / 12.92 : Math.Pow((G + 0.055) / 1.055, 2.4);
            B = (B <= 0.03928) ? B / 12.92 : Math.Pow((B + 0.055) / 1.055, 2.4);

            return 0.2126 * R + 0.7152 * G + 0.0722 * B;
        }

        // 🔹 Nuevo: sugerir un color accesible alternativo
        public static string SuggestAccessibleColor(string fg, string bg)
        {
            var fgColor = System.Drawing.ColorTranslator.FromHtml(fg);
            var bgColor = System.Drawing.ColorTranslator.FromHtml(bg);

            // Si ya es válido, regresa el mismo
            if (IsContrastAccessible(fg, bg))
                return fg;

            // Estrategia: aclarar u oscurecer el color hasta lograr contraste válido
            for (int i = 0; i < 20; i++)
            {
                var adjusted = AdjustBrightness(fgColor, i * 0.1);
                if (IsContrastAccessible(ColorTranslator.ToHtml(adjusted), bg))
                    return ColorTranslator.ToHtml(adjusted);
            }

            // fallback: negro o blanco
            return "#000000";
        }

        private static System.Drawing.Color AdjustBrightness(System.Drawing.Color color, double factor)
        {
            int r = Math.Min(255, (int)(color.R * (1 - factor)));
            int g = Math.Min(255, (int)(color.G * (1 - factor)));
            int b = Math.Min(255, (int)(color.B * (1 - factor)));
            return System.Drawing.Color.FromArgb(r, g, b);
        }
    }
}