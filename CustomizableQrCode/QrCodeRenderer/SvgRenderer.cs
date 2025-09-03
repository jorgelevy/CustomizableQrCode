using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.QrCodeRenderer
{
    /// <summary>
    /// Clase responsable de generar SVGs personalizados de códigos QR,
    /// incluyendo la personalización de módulos, marcos y centros de los ojos, colores y logos.
    /// </summary>
    public static class SvgRenderer
    {
        /// <summary>
        /// Genera el SVG completo de un código QR personalizado, permitiendo múltiples estilos para módulos y marcos de los ojos.
        /// </summary>
        /// <param name="matrix">Matriz booleana que representa los módulos del QR.</param>
        /// <param name="moduleShape">Forma de los módulos del QR.</param>
        /// <param name="moduleColor">Color de los módulos.</param>
        /// <param name="eyeFrameShape">Forma del marco de los ojos del QR.</param>
        /// <param name="eyeFrameColor">Color del marco de los ojos.</param>
        /// <param name="eyeCenterShape">Forma del centro de los ojos del QR.</param>
        /// <param name="eyeCenterColor">Color del centro de los ojos.</param>
        /// <param name="backgroundGradient">Color de fondo o gradiente (opcional).</param>
        /// <param name="logoBase64">Imagen del logo en base64 (opcional).</param>
        /// <param name="size">Tamaño en píxeles del SVG generado.</param>
        /// <returns>SVG como cadena de texto.</returns>
        public static string Render(
            bool[,] matrix,
            ModuleShape moduleShape,
            string moduleColor,
            EyeFrameShape eyeFrameShape,
            string eyeFrameColor,
            EyeCenterShape eyeCenterShape,
            string eyeCenterColor,
            string? backgroundGradient,
            string? logoBase64,
            int size
        )
        {
            int quietZone = 4;
            int modules = matrix.GetLength(0);
            double moduleSize = (double)size / modules;
            double eyeSize = moduleSize * 7;
            double innerSize = eyeSize - 2 * moduleSize;
            double centerSize = eyeSize - 4 * moduleSize;

            // Coordenadas de los tres ojos del QR (esquinas)
            var eyePositions = new (int x, int y)[]
            {
                (quietZone, quietZone),
                (modules - 7 - quietZone, quietZone),
                (quietZone, modules - 7 - quietZone)
            };

            //var sb = new StringBuilder();
            //sb.AppendLine($"<svg xmlns='http://www.w3.org/2000/svg' width='{size}' height='{size}' viewBox='0 0 {size} {size}'>");
            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns='http://www.w3.org/2000/svg' width='{size}' height='{size}' viewBox='0 0 {size} {size}'>");


            //// Fondo del SVG
            //if (!string.IsNullOrEmpty(backgroundGradient))
            //{
            //    var colors = backgroundGradient.Split(',');
            //    if (colors.Length == 2)
            //    {
            //        sb.AppendLine($"<defs><linearGradient id='bg' x1='0%' y1='0%' x2='100%' y2='100%'>" +
            //            $"<stop offset='0%' stop-color='{colors[0]}'/>" +
            //            $"<stop offset='100%' stop-color='{colors[1]}'/>" +
            //            $"</linearGradient></defs>");
            //        sb.AppendLine($"<rect width='{size}' height='{size}' fill='url(#bg)'/>");
            //    }
            //    else
            //    {
            //        sb.AppendLine($"<rect width='{size}' height='{size}' fill='{colors[0]}'/>");
            //    }
            //}
            //else
            //{
            //    sb.AppendLine($"<rect width='{size}' height='{size}' fill='#fff'/>");
            //}

            //// Función auxiliar: Determina si un módulo está en el área de algún ojo
            //bool IsInEyeArea(int x, int y)
            //{
            //    foreach (var (ex, ey) in eyePositions)
            //    {
            //        if (x >= ex && x < ex + 7 && y >= ey && y < ey + 7)
            //            return true;
            //    }
            //    return false;
            //}

            // 🔹 Definiciones globales (gradientes y filtros)
            sb.AppendLine("<defs>");
                    sb.AppendLine(@"
                <filter id='eyeShadow' x='-20%' y='-20%' width='140%' height='140%'>
                    <feDropShadow dx='0' dy='2' stdDeviation='2' flood-color='rgba(0,0,0,0.25)'/>
                </filter>
            ");
            if (!string.IsNullOrEmpty(backgroundGradient))
            {
                var colors = backgroundGradient.Split(',');
                if (colors.Length == 2)
                {
                    sb.AppendLine(@$"
                <radialGradient id='bg' cx='50%' cy='50%' r='75%'>
                    <stop offset='0%' stop-color='{colors[0]}'/>
                    <stop offset='100%' stop-color='{colors[1]}'/>
                </radialGradient>
            ");
                }
            }
            sb.AppendLine("</defs>");

            // 🔹 Fondo
            if (!string.IsNullOrEmpty(backgroundGradient))
            {
                var colors = backgroundGradient.Split(',');
                if (colors.Length == 2)
                    sb.AppendLine($"<rect width='{size}' height='{size}' fill='url(#bg)'/>");
                else
                    sb.AppendLine($"<rect width='{size}' height='{size}' fill='{colors[0]}'/>");
            }
            else
            {
                sb.AppendLine($"<rect width='{size}' height='{size}' fill='#fff'/>");
            }

            // 🔹 Función auxiliar: detectar si está en área de ojo
            bool IsInEyeArea(int x, int y)
            {
                foreach (var (ex, ey) in eyePositions)
                {
                    if (x >= ex && x < ex + 7 && y >= ey && y < ey + 7)
                        return true;
                }
                return false;
            }

            // Renderizado de los módulos (sin superponer los ojos)
            for (int x = 0; x < modules; x++)
            {
                for (int y = 0; y < modules; y++)
                {
                    if (IsInEyeArea(x, y)) continue;
                    if (!matrix[x, y]) continue;
                    double px = x * moduleSize;
                    double py = y * moduleSize;

                    switch (moduleShape)
                    {
                        case ModuleShape.Square:
                            sb.AppendLine($"<rect x='{px}' y='{py}' width='{moduleSize}' height='{moduleSize}' fill='{moduleColor}'/>");
                            break;
                        case ModuleShape.Circle:
                            sb.AppendLine($"<circle cx='{px + moduleSize / 2}' cy='{py + moduleSize / 2}' r='{moduleSize / 2}' fill='{moduleColor}'/>");
                            break;
                        case ModuleShape.Hexagon:
                            var hex = GetHexagonPoints(px, py, moduleSize);
                            sb.AppendLine($"<polygon points='{hex}' fill='{moduleColor}'/>");
                            break;
                        //case ModuleShape.Rounded:
                        //    sb.AppendLine($"<rect x='{px}' y='{py}' width='{moduleSize}' height='{moduleSize}' rx='{moduleSize * 0.4}' fill='{moduleColor}'/>");
                        //    break;
                        //case ModuleShape.Outline:
                        //    sb.AppendLine($"<rect x='{px}' y='{py}' width='{moduleSize}' height='{moduleSize}' fill='none' stroke='{moduleColor}' stroke-width='{moduleSize * 0.2}'/>");
                        //    break;
                        case ModuleShape.Heart:
                            sb.AppendLine($"<path d='M{px + moduleSize / 2},{py + moduleSize / 4} " +
                                $"C{px},{py - moduleSize / 4} {px - moduleSize / 2},{py + moduleSize / 2} {px + moduleSize / 2},{py + moduleSize} " +
                                $"C{px + moduleSize * 1.5},{py + moduleSize / 2} {px + moduleSize},{py - moduleSize / 4} {px + moduleSize / 2},{py + moduleSize / 4} Z' " +
                                $"fill='{moduleColor}'/>");
                            break;

                        case ModuleShape.Star:
                            sb.AppendLine(DrawStar(px + moduleSize / 2, py + moduleSize / 2, moduleSize / 2, moduleColor));
                            break;

                        case ModuleShape.Diamond:
                            sb.AppendLine($"<polygon points='{px + moduleSize / 2},{py} {px + moduleSize},{py + moduleSize / 2} {px + moduleSize / 2},{py + moduleSize} {px},{py + moduleSize / 2}' fill='{moduleColor}'/>");
                            break;

                        case ModuleShape.VerticalBar:
                            sb.AppendLine($"<rect x='{px + moduleSize * 0.35}' y='{py}' width='{moduleSize * 0.3}' height='{moduleSize}' rx='{moduleSize * 0.15}' fill='{moduleColor}'/>");
                            break;

                        case ModuleShape.HorizontalBar:
                            sb.AppendLine($"<rect x='{px}' y='{py + moduleSize * 0.35}' width='{moduleSize}' height='{moduleSize * 0.3}' rx='{moduleSize * 0.15}' fill='{moduleColor}'/>");
                            break;

                        case ModuleShape.Block:
                            sb.AppendLine($"<rect x='{px + moduleSize * 0.1}' y='{py + moduleSize * 0.1}' width='{moduleSize * 0.8}' height='{moduleSize * 0.8}' fill='{moduleColor}'/>");
                            break;

                        //case ModuleShape.RoundedBlock:
                        //    sb.AppendLine($"<rect x='{px + moduleSize * 0.1}' y='{py + moduleSize * 0.1}' width='{moduleSize * 0.8}' height='{moduleSize * 0.8}' rx='{moduleSize * 0.3}' fill='{moduleColor}'/>");
                        //    break;
                        case ModuleShape.Triangle:
                            sb.AppendLine(DrawTriangle(px, py, moduleSize, moduleColor));
                            break;
                        case ModuleShape.Cross:
                            sb.AppendLine(DrawCross(px, py, moduleSize, moduleColor));
                            break;
                        case ModuleShape.Teardrop:
                            sb.AppendLine(DrawTeardrop(px, py, moduleSize, moduleColor));
                            break;
                        case ModuleShape.Wave:
                            sb.AppendLine(DrawWave(px, py, moduleSize, moduleColor));
                            break;
                        case ModuleShape.Flower:
                            sb.AppendLine(DrawFlower(px, py, moduleSize, moduleColor));
                            break;
                        case ModuleShape.PixelBurst:
                            sb.AppendLine(DrawPixelBurst(px, py, moduleSize, moduleColor));
                            break;
                        //case ModuleShape.Infinity:
                        //    sb.AppendLine(DrawInfinity(px, py, moduleSize, moduleColor));
                        //    break;
                        case ModuleShape.Snowflake:
                            sb.AppendLine(DrawSnowflake(px, py, moduleSize, moduleColor));
                            break;
                        //case ModuleShape.Arrow:
                        //    //sb.AppendLine(DrawArrow(px, py, moduleSize, moduleColor));
                        //    sb.AppendLine($"<g transform='rotate(90,{px},{py})'>{DrawArrow(px, py, moduleSize, moduleColor)}</g>");
                        //    break;
                        case ModuleShape.DotGrid:
                            sb.AppendLine(DrawDotGrid(px, py, moduleSize, moduleColor));
                            break;
                    }

                }
            }

            // Renderizado de los ojos del QR y sus centros
            for (int eyeIndex = 0; eyeIndex < eyePositions.Length; eyeIndex++)
            {
                var (ex, ey) = eyePositions[eyeIndex];
                double ox = ex * moduleSize;
                double oy = ey * moduleSize;
                double centerCx = ox + eyeSize / 2;
                double centerCy = oy + eyeSize / 2;

                // Determina el ángulo de rotación según el ojo (para marcos asimétricos)
                double angle = 0;
                switch (eyeIndex)
                {
                    case 0: angle = 270; break; // Superior izquierda
                    case 1: angle = 360; break; // Superior derecha
                    case 2: angle = -180; break; // Inferior izquierda
                }

                // Renderizado del marco del ojo según el estilo seleccionado
                switch (eyeFrameShape)
                {
                    case EyeFrameShape.Square:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Rounded:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' rx='{moduleSize * 1.5}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' rx='{moduleSize * 0.8}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Circle:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{eyeSize / 2}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Diamond:
                        sb.AppendLine($"<path d='{GetDiamondPath(centerCx, centerCy, eyeSize / 2)}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<path d='{GetDiamondPath(centerCx, centerCy, innerSize / 2)}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Leaf:
                        sb.AppendLine(DrawLeafEye(ox, oy, eyeSize, eyeFrameColor));
                        sb.AppendLine(DrawLeafEye(ox + moduleSize, oy + moduleSize, innerSize, "#fff"));
                        break;
                    case EyeFrameShape.Point:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{eyeSize / 2}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Dotted:
                        sb.AppendLine(DrawDottedEye(ox, oy, eyeSize, eyeFrameColor, moduleSize));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Double:
                        sb.AppendLine(DrawDoubleEye(ox, oy, eyeSize, eyeFrameColor, moduleSize));
                        sb.AppendLine($"<rect x='{ox + moduleSize * 2}' y='{oy + moduleSize * 2}' width='{eyeSize - 4 * moduleSize}' height='{eyeSize - 4 * moduleSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.CircleInSquare:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' rx='{moduleSize * 1.5}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularLeft:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "left"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularRight:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "right"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularTop:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "top"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularBottom:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "bottom"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Wavy:
                        sb.AppendLine(DrawWavyEye(ox, oy, eyeSize, eyeFrameColor));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Pixelated:
                        sb.AppendLine(DrawPixelatedEye(ox, oy, eyeSize, eyeFrameColor));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    // === Estilos personalizados (esquinas rectas, redondeadas, combinadas, etc.) ===
                    case EyeFrameShape.CornerRect:
                        string marco = DrawBottomLeftSquareEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marco, centerCx, centerCy, angle));
                        string centro = DrawBottomLeftSquareEyeCenter(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centro, centerCx, centerCy, angle));
                        break;
                    case EyeFrameShape.TwoCornerRect:
                        int[] angles = { 90, 180, 0 };
                        double cx = ox + eyeSize / 2;
                        double cy = oy + eyeSize / 2;
                        sb.AppendLine(WithRotation(DrawTwoCornerRectEye(ox, oy, eyeSize, eyeFrameColor), cx, cy, angles[eyeIndex]));
                        sb.AppendLine(WithRotation(DrawTwoCornerRectEyeCenter(ox, oy, eyeSize, "#fff"), cx, cy, angles[eyeIndex]));
                        break;
                    case EyeFrameShape.CornerRectRadio:
                        string marcoradio = DrawBottomLeftSquareRadioEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marcoradio, centerCx, centerCy, angle));
                        string centroradio = DrawBottomLeftSquareRadioEyeCenter(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centroradio, centerCx, centerCy, angle));
                        break;
                    case EyeFrameShape.TwoCornerRectIn:
                        int[] anglesIn = { 0, 270, 90 };
                        double cxIn = ox + eyeSize / 2;
                        double cyIn = oy + eyeSize / 2;
                        sb.AppendLine(WithRotation(DrawTwoCornerRectInEye(ox, oy, eyeSize, eyeFrameColor), cxIn, cyIn, anglesIn[eyeIndex]));
                        sb.AppendLine(WithRotation(DrawTwoCornerRectInEyeCenter(ox, oy, eyeSize, "#fff"), cxIn, cyIn, anglesIn[eyeIndex]));
                        break;
                    case EyeFrameShape.CornerRoundOut:
                        string marcoRCO = DrawBottomLeftSquareRCOEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marcoRCO, centerCx, centerCy, angle));
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.CornerRoundOutSP:
                        string marcoRCOSP = DrawBottomLeftSquareRCOSPEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marcoRCOSP, centerCx, centerCy, angle));
                        string centroRCOSP = DrawBottomLeftSquareRCOSPEyePupil(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centroRCOSP, centerCx, centerCy, angle));
                        break;
                    default:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                }

                // Renderizado del centro/pupila del ojo
                EyeCenterShape allowedShape = eyeCenterShape;
                if (eyeFrameShape == EyeFrameShape.Leaf && eyeCenterShape != EyeCenterShape.Circle && eyeCenterShape != EyeCenterShape.Leaf)
                    allowedShape = EyeCenterShape.Leaf;
                else if (eyeFrameShape == EyeFrameShape.Diamond && eyeCenterShape != EyeCenterShape.Circle && eyeCenterShape != EyeCenterShape.Diamond)
                    allowedShape = EyeCenterShape.Diamond;

                switch (allowedShape)
                {
                    case EyeCenterShape.Circle:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{centerSize / 2}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Square:
                        sb.AppendLine($"<rect x='{centerCx - centerSize / 2}' y='{centerCy - centerSize / 2}' width='{centerSize}' height='{centerSize}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Rounded:
                        sb.AppendLine($"<rect x='{centerCx - centerSize / 2}' y='{centerCy - centerSize / 2}' width='{centerSize}' height='{centerSize}' rx='{centerSize * 0.25}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Point:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{centerSize * 0.28}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Diamond:
                        sb.AppendLine($"<path d='{GetDiamondPath(centerCx, centerCy, centerSize / 2)}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Leaf:
                        sb.AppendLine(DrawLeafEye(centerCx - centerSize / 2, centerCy - centerSize / 2, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.CornerRect:
                        string pupil = DrawBottomLeftSquareEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupil, centerCx, centerCy, angle));
                        break;
                    case EyeCenterShape.TwoCornerRect:
                        int[] angles = { 90, 180, 0 };
                        double cx = ox + eyeSize / 2;
                        double cy = oy + eyeSize / 2;
                        string pupila = DrawTwoCornerRectEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupila, cx, cy, angles[eyeIndex]));
                        break;
                    case EyeCenterShape.CornerRectRadio:
                        string pupilRadio = DrawBottomLeftSquareRadioEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupilRadio, centerCx, centerCy, angle));
                        break;
                    case EyeCenterShape.TwoCornerRectIn:
                        int[] anglesIn = { 0, 270, 90 };
                        double cxIn = ox + eyeSize / 2;
                        double cyIn = oy + eyeSize / 2;
                        string pupilaIn = DrawTwoCornerRectInEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupilaIn, cxIn, cyIn, anglesIn[eyeIndex]));
                        break;
                    case EyeCenterShape.CornerRoundOut:
                        string pupilCRO = DrawBottomLeftSquareRCOEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupilCRO, centerCx, centerCy, angle));
                        break;
                    case EyeCenterShape.Plus:
                        sb.AppendLine(DrawPlusShape(centerCx, centerCy, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.Star:
                        sb.AppendLine(DrawStarShapeRounded(centerCx, centerCy, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.Starburst:
                        // Si eyeCenterShape == EyeCenterShape.StarburstCircle
                        sb.AppendLine(DrawStarburstEyeWithCircle(centerCx, centerCy, centerSize, eyeCenterColor, eyeCenterColor,15, 0.75));
                        break;
                    case EyeCenterShape.ConcaveSquircle:
                        sb.AppendLine(DrawConcaveSquircleEye(centerCx, centerCy, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.DiagonalCut:
                        string diagonalCut = DrawDiagonalCutEye(centerCx, centerCy, centerSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(diagonalCut, centerCx, centerCy, angle));
                        break;
                    case EyeCenterShape.TripleBar:
                        sb.AppendLine(DrawTripleBarEye(centerCx, centerCy, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.TripleBarV:
                        sb.AppendLine(DrawTripleBarVerticalEye(centerCx, centerCy, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.BubbleGrid:
                        sb.AppendLine(DrawBubbleGridEye(centerCx, centerCy, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.BubbleOver:
                        sb.AppendLine(DrawBubbleOverEye(centerCx - centerSize / 2, centerCy - centerSize / 2, centerSize, eyeCenterColor));
                        break;

                    case EyeCenterShape.BlockGrid:
                        sb.AppendLine(DrawBlockGridEye(centerCx - centerSize / 2, centerCy - centerSize / 2, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.IrregularSquare:
                        sb.AppendLine(DrawIrregularSquareEye(centerCx - centerSize / 2, centerCy - centerSize / 2, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.WavySquare:
                        sb.AppendLine(DrawWavySquareEye(centerCx - centerSize / 2, centerCy - centerSize / 2, centerSize, eyeCenterColor));
                        break;

                }
            }

            // Logo central (opcional)
            if (!string.IsNullOrWhiteSpace(logoBase64))
            {
                var (logoOriginalWidth, logoOriginalHeight) = GetImageDimensionsFromBase64(logoBase64);
                double aspectRatio = (double)logoOriginalWidth / logoOriginalHeight;

                double maxLogoArea = size * 0.22;
                double logoWidth, logoHeight;

                if (aspectRatio >= 1)
                {
                    logoWidth = maxLogoArea;
                    logoHeight = maxLogoArea / aspectRatio;
                }
                else
                {
                    logoHeight = maxLogoArea;
                    logoWidth = maxLogoArea * aspectRatio;
                }

                double logoX = (size - logoWidth) / 2.0;
                double logoY = (size - logoHeight) / 2.0;
                double margin = maxLogoArea * 0.07;
                double rectWidth = logoWidth + margin * 2;
                double rectHeight = logoHeight + margin * 2;
                double rectX = (size - rectWidth) / 2.0;
                double rectY = (size - rectHeight) / 2.0;

                sb.AppendLine($"<rect x='{rectX}' y='{rectY}' width='{rectWidth}' height='{rectHeight}' rx='{rectWidth * 0.12}' fill='#fff'/>");
                sb.AppendLine(
                    $"<image href='{logoBase64}' x='{logoX}' y='{logoY}' width='{logoWidth}' height='{logoHeight}' " +
                    $"style='border-radius:12px;pointer-events:none;' preserveAspectRatio='xMidYMid meet' />"
                );
            }

            sb.AppendLine("</svg>");
            return sb.ToString();
        }

        /// <summary>
        /// Calcula los puntos para dibujar un hexágono en SVG.
        /// </summary>
        /// <param name="x">Coordenada X de la esquina superior izquierda.</param>
        /// <param name="y">Coordenada Y de la esquina superior izquierda.</param>
        /// <param name="size">Tamaño del hexágono.</param>
        /// <returns>Cadena con los puntos SVG.</returns>
        private static string GetHexagonPoints(double x, double y, double size)
        {
            double dx = size / 2.0;
            double dy = size / 2.0;
            double r = size / 2.0;
            var points = new List<(double X, double Y)>();
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3.0 * i - Math.PI / 6.0;
                double px = x + dx + r * Math.Cos(angle);
                double py = y + dy + r * Math.Sin(angle);
                points.Add((px, py));
            }
            return string.Join(" ", points.Select(p => $"{p.X},{p.Y}"));
        }

        /// <summary>
        /// Genera el path SVG para un diamante (rombo).
        /// </summary>
        /// <param name="cx">Centro X.</param>
        /// <param name="cy">Centro Y.</param>
        /// <param name="r">Radio (distancia desde el centro a los vértices).</param>
        /// <returns>Cadena path SVG.</returns>
        private static string GetDiamondPath(double cx, double cy, double r)
        {
            return $"M{cx},{cy - r} L{cx + r},{cy} L{cx},{cy + r} L{cx - r},{cy} Z";
        }

        /// <summary>
        /// Dibuja un "ojo hoja" (Leaf eye) para QR personalizados.
        /// </summary>
        private static string DrawLeafEye(double x, double y, double size, string color)
        {
            var cx = x + size / 2;
            var cy = y + size / 2;
            var r = size / 2;
            return $"<path d='M{cx} {cy - r} Q {cx + r} {cy} {cx} {cy + r} Q {cx - r} {cy} {cx} {cy - r} Z' fill='{color}'/>";
        }

        /// <summary>
        /// Dibuja un marco punteado alrededor de un ojo.
        /// </summary>
        private static string DrawDottedEye(double x, double y, double size, string color, double moduleSize)
        {
            int dots = 14;
            double r = moduleSize * 0.38;
            var sb = new StringBuilder();
            for (int i = 0; i < dots; i++)
            {
                double px = x + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{px}' cy='{y}' r='{r}' fill='{color}'/>");
            }
            for (int i = 0; i < dots; i++)
            {
                double px = x + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{px}' cy='{y + size}' r='{r}' fill='{color}'/>");
            }
            for (int i = 1; i < dots - 1; i++)
            {
                double py = y + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{x}' cy='{py}' r='{r}' fill='{color}'/>");
            }
            for (int i = 1; i < dots - 1; i++)
            {
                double py = y + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{x + size}' cy='{py}' r='{r}' fill='{color}'/>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja un ojo doble (dos bordes) para QR personalizados.
        /// </summary>
        private static string DrawDoubleEye(double x, double y, double size, string color, double moduleSize)
        {
            double margin = moduleSize * 0.9;
            var sb = new StringBuilder();
            sb.AppendLine($"<rect x='{x}' y='{y}' width='{size}' height='{size}' fill='none' stroke='{color}' stroke-width='{margin}'/>");
            sb.AppendLine($"<rect x='{x + margin * 1.4}' y='{y + margin * 1.4}' width='{size - margin * 2.8}' height='{size - margin * 2.8}' fill='none' stroke='{color}' stroke-width='{margin / 1.7}'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja un marco con ondas senoidales.
        /// </summary>
        private static string DrawWavyEye(double x, double y, double size, string color)
        {
            int waves = 4;
            double amplitude = size * 0.07;
            var sb = new StringBuilder();
            sb.Append($"<path d='M{x},{y + amplitude}");
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size * (t - 0.25 / waves)},{y - amplitude} {x + size * t},{y + amplitude}");
            }
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size + amplitude},{y + size * (t - 0.25 / waves)} {x + size - amplitude},{y + size * t}");
            }
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size * (1 - t + 0.25 / waves)},{y + size + amplitude} {x + size * (1 - t)},{y + size - amplitude}");
            }
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x - amplitude},{y + size * (1 - t + 0.25 / waves)} {x + amplitude},{y + size * (1 - t)}");
            }
            sb.Append(" Z' fill='" + color + "'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja una esquina del marco del ojo redondeada, según el parámetro.
        /// </summary>
        /// <param name="corner">Puede ser 'left', 'right', 'top', 'bottom'.</param>
        private static string DrawCornerRoundedEye(double x, double y, double size, string color, string corner)
        {
            double r = size * 0.25;
            var sb = new StringBuilder();

            switch (corner)
            {
                case "left":
                    sb.Append($"<path d='M{x + r},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"L{x},{y + r} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
                    sb.Append("Z' fill='" + color + "'/>");
                    break;
                case "right":
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size - r},{y} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
                case "top":
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x + r},{y + size} ");
                    sb.Append($"A{r},{r} 0 0,1 {x},{y + size - r} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
                case "bottom":
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size - r} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja el marco pixelado de un ojo de QR.
        /// </summary>
        private static string DrawPixelatedEye(double x, double y, double size, string color)
        {
            int pixels = 8;
            double pixelSize = size / (double)pixels;
            var sb = new StringBuilder();
            for (int i = 0; i < pixels; i++)
                sb.Append($"<rect x='{x + i * pixelSize}' y='{y}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            for (int i = 1; i < pixels; i++)
                sb.Append($"<rect x='{x + size - pixelSize}' y='{y + i * pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            for (int i = pixels - 2; i >= 0; i--)
                sb.Append($"<rect x='{x + i * pixelSize}' y='{y + size - pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            for (int i = pixels - 2; i > 0; i--)
                sb.Append($"<rect x='{x}' y='{y + i * pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Aplica rotación a un fragmento SVG alrededor de un centro dado.
        /// </summary>
        private static string WithRotation(string path, double cx, double cy, double angle)
        {
            return $"<g transform='rotate({angle},{cx},{cy})'>{path}</g>";
        }

        /// <summary>
        /// Dibuja un marco de ojo con una esquina inferior izquierda cuadrada, resto redondeado.
        /// </summary>
        private static string DrawBottomLeftSquareEye(double x, double y, double size, string color)
        {
            double r = size * 0.30;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + size} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x + size - r},{y} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
            sb.Append($"L{x + size},{y + size - r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} ");
            sb.Append($"L{x},{y + size} Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Dibuja el centro (blanco) para el marco de ojo BottomLeftSquare.
        /// </summary>
        private static string DrawBottomLeftSquareEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawBottomLeftSquareEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Dibuja la pupila para el marco de ojo BottomLeftSquare.
        /// </summary>
        private static string DrawBottomLeftSquareEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja un marco de ojo con dos esquinas opuestas redondeadas.
        /// </summary>
        private static string DrawTwoCornerRectEye(double x, double y, double size, string color, bool outward = true)
        {
            double r = size * 0.28;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"L{x},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Centro blanco para marco TwoCornerRect.
        /// </summary>
        private static string DrawTwoCornerRectEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawTwoCornerRectEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Pupila para marco TwoCornerRect.
        /// </summary>
        private static string DrawTwoCornerRectEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawTwoCornerRectEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco exterior con esquinas redondeadas (radio mayor).
        /// </summary>
        private static string DrawBottomLeftSquareRadioEye(double x, double y, double size, string color)
        {
            double r = size * 0.50;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + size} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x + size - r},{y} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
            sb.Append($"L{x + size},{y + size - r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} ");
            sb.Append($"L{x},{y + size} Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Centro blanco para el marco BottomLeftSquareRadio.
        /// </summary>
        private static string DrawBottomLeftSquareRadioEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Pupila para el marco BottomLeftSquareRadio.
        /// </summary>
        private static string DrawBottomLeftSquareRadioEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco con esquinas internas redondeadas.
        /// </summary>
        private static string DrawTwoCornerRectInEye(double x, double y, double size, string color, bool outward = true)
        {
            double r = size * 0.23;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x + r},{y} ");
            sb.Append($"A{r},{r} 0 0,1 {x},{y + r} ");
            sb.Append($"L{x},{y + size - r} ");
            sb.Append($"A{r},{r} 0 0,0 {x + r},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y + r} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size - r},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Centro blanco para el marco TwoCornerRectIn.
        /// </summary>
        private static string DrawTwoCornerRectInEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawTwoCornerRectInEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Pupila para el marco TwoCornerRectIn.
        /// </summary>
        private static string DrawTwoCornerRectInEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawTwoCornerRectInEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco exterior "CornerRoundOut".
        /// </summary>
        private static string DrawBottomLeftSquareRCOEye(double x, double y, double size, string color)
        {
            double r = size * 0.50;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"L{x},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Pupila para el marco CornerRoundOut.
        /// </summary>
        private static string DrawBottomLeftSquareRCOEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco exterior "CornerRoundOutSP".
        /// </summary>
        private static string DrawBottomLeftSquareRCOSPEye(double x, double y, double size, string color)
        {
            double r = size * 0.50;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"L{x},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Pupila para el marco CornerRoundOutSP.
        /// </summary>
        private static string DrawBottomLeftSquareRCOSPEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Pupila especial para el marco TwoCornerRectIn (no utilizado en principal, pero disponible).
        /// </summary>
        private static string DrawTwoCornerRCOSPInEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawTwoCornerRectInEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Genera una figura SVG de cruz tipo “plus”
        /// centrada en (cx, cy) y de tamaño size x size.
        /// </summary>
        /// <param name="cx">Centro X</param>
        /// <param name="cy">Centro Y</param>
        /// <param name="size">Tamaño total de la figura (lado del cuadrado contenedor)</param>
        /// <param name="color">Color de relleno</param>
        /// <returns>SVG de la cruz</returns>
        private static string DrawPlusShape(double cx, double cy, double size, string color)
        {
            // La cruz es un cuadrado central con dos rectángulos cruzados
            // Tamaño del brazo como proporción (ajustable)
            double arm = size * 0.3; // ancho del brazo
            double half = size / 2;

            // Coordenadas de la cruz
            double x = cx - half;
            double y = cy - half;

            // Path SVG para una cruz
            //      ---
            //    |     |
            //   ---   ---
            //    |     |
            //      ---
            var sb = new StringBuilder();
            sb.Append($"<rect x='{x + arm}' y='{y}' width='{size - 2 * arm}' height='{size}' fill='{color}'/>"); // Vertical
            sb.Append($"<rect x='{x}' y='{y + arm}' width='{size}' height='{size - 2 * arm}' fill='{color}'/>"); // Horizontal
            return sb.ToString();
        }

        /// <summary>
        /// Genera una estrella de cinco puntas centrada en (cx, cy) y de tamaño size x size.
        /// </summary>
        /// <param name="cx">Centro X</param>
        /// <param name="cy">Centro Y</param>
        /// <param name="size">Tamaño de la figura</param>
        /// <param name="color">Color de relleno</param>
        /// <returns>SVG de la estrella</returns>
        private static string DrawStarShapeRounded(double cx, double cy, double size, string color)
        {
            int points = 5;
            double outerRadius = size / 2;
            double innerRadius = outerRadius * 0.5;
            double angle = -Math.PI / 2;
            var sb = new StringBuilder();
            sb.Append($"<polygon fill='{color}' stroke='{color}' stroke-width='{size * 0.13}' stroke-linejoin='round' points='");

            for (int i = 0; i < points * 2; i++)
            {
                double r = (i % 2 == 0) ? outerRadius : innerRadius;
                double a = angle + i * Math.PI / points;
                double x = cx + r * Math.Cos(a);
                double y = cy + r * Math.Sin(a);
                sb.Append($"{x},{y} ");
            }

            sb.Append("'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja una forma de estrella de picos (starburst) con un círculo central más grande.
        /// </summary>
        /// <param name="cx">Centro X.</param>
        /// <param name="cy">Centro Y.</param>
        /// <param name="size">Tamaño total (ancho/alto).</param>
        /// <param name="color">Color de la estrella.</param>
        /// <param name="circleColor">Color del círculo central.</param>
        /// <param name="spikes">Cantidad de picos (default 20).</param>
        /// <param name="circleRatio">Proporción del círculo respecto al tamaño (default 0.58).</param>
        /// <returns>SVG string.</returns>
        private static string DrawStarburstEyeWithCircle(double cx, double cy, double size, string color, string circleColor, int spikes = 20, double circleRatio = 0.58)
        {
            double outerRadius = size / 2;
            double innerRadius = outerRadius * 0.65;
            double angle = -Math.PI / 2;

            var sb = new StringBuilder();
            sb.Append($"<polygon fill='{color}' points='");
            for (int i = 0; i < spikes * 2; i++)
            {
                double r = (i % 2 == 0) ? outerRadius : innerRadius;
                double a = angle + i * Math.PI / spikes;
                double x = cx + r * Math.Cos(a);
                double y = cy + r * Math.Sin(a);
                sb.Append($"{x},{y} ");
            }
            sb.Append("'/>");

            // Agrega círculo central (encima, más grande que el "default")
            double circleRadius = outerRadius * circleRatio;
            sb.Append($"<circle cx='{cx}' cy='{cy}' r='{circleRadius}' fill='{circleColor}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Dibuja una forma cuadrada con lados cóncavos (concave squircle/inward square) para el ojo del QR.
        /// </summary>
        /// <param name="cx">Coordenada X del centro de la figura.</param>
        /// <param name="cy">Coordenada Y del centro de la figura.</param>
        /// <param name="size">Tamaño total de la figura (ancho/alto).</param>
        /// <param name="color">Color de relleno.</param>
        /// <param name="depth">Profundidad de la concavidad (0 a 0.5, valor sugerido: 0.2).</param>
        /// <returns>SVG path string.</returns>
        private static string DrawConcaveSquircleEye(double cx, double cy, double size, string color, double depth = -0.10)
        {
            // Definimos los 4 vértices y los puntos medios para el control de curvas cóncavas
            double half = size / 2;
            double left = cx - half;
            double right = cx + half;
            double top = cy - half;
            double bottom = cy + half;
            double offset = size * depth;

            var sb = new StringBuilder();
            sb.Append($"<path d='");
            sb.Append($"M {left},{top + offset} ");
            sb.Append($"Q {cx},{top - offset} {right},{top + offset} ");
            sb.Append($"Q {right + offset},{cy} {right},{bottom - offset} ");
            sb.Append($"Q {cx},{bottom + offset} {left},{bottom - offset} ");
            sb.Append($"Q {left - offset},{cy} {left},{top + offset} ");
            sb.Append("' fill='" + color + "'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja una figura cuadrada con dos esquinas opuestas recortadas en diagonal para el centro del ojo QR.
        /// </summary>
        /// <param name="cx">Coordenada X del centro.</param>
        /// <param name="cy">Coordenada Y del centro.</param>
        /// <param name="size">Tamaño del cuadrado base.</param>
        /// <param name="color">Color de relleno.</param>
        /// <param name="cut">Proporción del corte diagonal respecto al lado (por defecto 0.28).</param>
        /// <returns>SVG path string.</returns>
        private static string DrawDiagonalCutEye(double cx, double cy, double size, string color, double cut = 0.40)
        {
            double half = size / 2;
            double left = cx - half;
            double right = cx + half;
            double top = cy - half;
            double bottom = cy + half;
            double d = size * cut;

            var sb = new StringBuilder();
            sb.Append($"<path d='");
            sb.Append($"M {left},{top + d} ");            // Arriba izquierda después del corte
            sb.Append($"L {left + d},{top} ");            // Arriba después del corte
            sb.Append($"L {right},{top} ");               // Arriba derecha
            sb.Append($"L {right},{bottom - d} ");        // Derecha abajo antes del corte
            sb.Append($"L {right - d},{bottom} ");        // Abajo después del corte
            sb.Append($"L {left},{bottom} ");             // Abajo izquierda
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja tres barras horizontales redondeadas como centro de ojo QR.
        /// </summary>
        /// <param name="cx">Coordenada X del centro del ojo.</param>
        /// <param name="cy">Coordenada Y del centro del ojo.</param>
        /// <param name="size">Tamaño total del área.</param>
        /// <param name="color">Color de las barras.</param>
        /// <returns>SVG group con tres rectángulos redondeados.</returns>
        private static string DrawTripleBarEye(double cx, double cy, double size, string color)
        {
            double barWidth = size * 1;
            double barHeight = size * 0.35;
            double spacing = size * 0.04;

            double left = cx - barWidth / 2;
            double top = cy - (barHeight * 1.5 + spacing);

            var sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                double y = top + i * (barHeight + spacing);
                sb.Append($"<rect x='{left}' y='{y}' width='{barWidth}' height='{barHeight}' rx='{barHeight / 2}' fill='{color}'/>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja tres barras verticales redondeadas como centro de ojo QR.
        /// </summary>
        /// <param name="cx">Coordenada X del centro del ojo.</param>
        /// <param name="cy">Coordenada Y del centro del ojo.</param>
        /// <param name="size">Tamaño total del área.</param>
        /// <param name="color">Color de las barras.</param>
        /// <returns>SVG group con tres rectángulos verticales redondeados.</returns>
        private static string DrawTripleBarVerticalEye(double cx, double cy, double size, string color)
        {
            double barHeight = size * 1;
            double barWidth = size * 0.25;
            double spacing = size * 0.04;

            double top = cy - barHeight / 2;
            double left = cx - (barWidth * 1.5 + spacing);

            var sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                double x = left + i * (barWidth + spacing);
                sb.Append($"<rect x='{x}' y='{top}' width='{barWidth}' height='{barHeight}' rx='{barWidth / 2}' fill='{color}'/>");
            }
            return sb.ToString();
        }


        /// <summary>
        /// Dibuja un ojo de 3x3 círculos ("Bubble Grid") en el centro (como la imagen proporcionada).
        /// </summary>
        /// <param name="cx">Coordenada X del centro.</param>
        /// <param name="cy">Coordenada Y del centro.</param>
        /// <param name="size">Tamaño total del área.</param>
        /// <param name="color">Color de los círculos.</param>
        /// <returns>SVG de una cuadrícula 3x3 de círculos.</returns>
        private static string DrawBubbleGridEye(double cx, double cy, double size, string color)
        {
            double gridSize = size * 0.85;   // Ajusta el tamaño total del grid dentro del centro
            double r = gridSize / 6;         // Radio de cada círculo
            double step = gridSize / 3;      // Distancia entre centros de los círculos

            // Coordenada inicio para centrar la cuadrícula 3x3
            double startX = cx - gridSize / 2;
            double startY = cy - gridSize / 2;

            var sb = new StringBuilder();

            // 3x3 círculos
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    double x = startX + col * step + step / 2;
                    double y = startY + row * step + step / 2;
                    sb.Append($"<circle cx='{x}' cy='{y}' r='{r}' fill='{color}'/>");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Dibuja un centro de ojo en forma de cuadrícula de 3x3 burbujas/círculos grandes superpuestos, cubriendo todo el área y sin dejar huecos internos.
        /// </summary>
        /// <param name="x">Coordenada X inicial.</param>
        /// <param name="y">Coordenada Y inicial.</param>
        /// <param name="size">Tamaño del área cuadrada que ocupará el centro del ojo.</param>
        /// <param name="color">Color de los círculos.</param>
        /// <returns>SVG con la cuadrícula de burbujas superpuestas.</returns>
        private static string DrawBubbleOverEye(double x, double y, double size, string color)
        {
            var sb = new StringBuilder();
            int grid = 3;
            // Queremos que el círculo del centro esté exactamente al centro, y los demás alrededor
            double gap = size * 0.15; // Ajusta el gap para más o menos superposición (más pequeño = más juntos)
            double bubbleSize = (size + gap * (grid - 1)) / grid;
            double realGridSize = bubbleSize * grid - gap * (grid - 1);
            double offset = (size - realGridSize) / 2;

            for (int row = 0; row < grid; row++)
            {
                for (int col = 0; col < grid; col++)
                {
                    double cx = x + offset + col * (bubbleSize - gap) + bubbleSize / 2;
                    double cy = y + offset + row * (bubbleSize - gap) + bubbleSize / 2;
                    sb.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{bubbleSize / 2}' fill='{color}'/>");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja un centro de ojo tipo "BlockGrid": una cuadrícula 3x3 de cuadrados, cada uno con una ligera rotación aleatoria para un efecto dinámico.
        /// </summary>
        private static string DrawBlockGridEye(double x, double y, double size, string color)
        {
            var sb = new StringBuilder();
            int grid = 3;
            // Tamaño del cuadrado menor para permitir giro sin salir del área
            double blockSize = size / (grid + 0.2);
            // Centrado de la cuadrícula dentro del área disponible
            double gridSize = blockSize * grid;
            double offset = (size - gridSize) / 2;

            Random rand = new Random(31); // Semilla fija para que no cambie cada render

            for (int row = 0; row < grid; row++)
            {
                for (int col = 0; col < grid; col++)
                {
                    double cx = x + offset + col * blockSize + blockSize / 2;
                    double cy = y + offset + row * blockSize + blockSize / 2;
                    // Rango de rotación: -15° a +15°
                    double angle = (rand.NextDouble() - 0.5) * 30;
                    sb.AppendLine($"<g transform='rotate({angle},{cx},{cy})'><rect x='{cx - blockSize / 2}' y='{cy - blockSize / 2}' width='{blockSize}' height='{blockSize}' fill='{color}'/></g>");
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Dibuja un centro de ojo tipo "IrregularSquare": un cuadrado cuyos bordes están perturbados aleatoriamente para dar apariencia de trazo manual.
        /// </summary>
        private static string DrawIrregularSquareEye(double x, double y, double size, string color)
        {
            var sb = new StringBuilder();
            int segmentsPerSide = 14; // Entre más segmentos, más detalle
            double step = size / segmentsPerSide;
            double amplitude = size * 0.06; // Qué tanto "vibra" el borde

            Random rand = new Random(43); // Semilla fija para render reproducible

            sb.Append($"<path d='");

            // Lado superior: izq -> der
            for (int i = 0; i <= segmentsPerSide; i++)
            {
                double px = x + i * step;
                double py = y + Math.Sin(i * 0.7) * rand.NextDouble() * amplitude;
                if (i == 0)
                    sb.Append($"M{px},{py} ");
                else
                    sb.Append($"L{px},{py} ");
            }

            // Lado derecho: arr -> abajo
            for (int i = 1; i <= segmentsPerSide; i++)
            {
                double px = x + size + Math.Sin(i * 0.5) * rand.NextDouble() * amplitude;
                double py = y + i * step;
                sb.Append($"L{px},{py} ");
            }

            // Lado inferior: der -> izq
            for (int i = 1; i <= segmentsPerSide; i++)
            {
                double px = x + size - i * step;
                double py = y + size + Math.Sin(i * 0.6) * rand.NextDouble() * amplitude;
                sb.Append($"L{px},{py} ");
            }

            // Lado izquierdo: abajo -> arriba
            for (int i = 1; i < segmentsPerSide; i++)
            {
                double px = x + Math.Sin(i * 0.7) * rand.NextDouble() * amplitude;
                double py = y + size - i * step;
                sb.Append($"L{px},{py} ");
            }

            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");
            return sb.ToString();
        }


        /// <summary>
        /// Dibuja un centro de ojo cuadrado con bordes ondulados ("WavySquare") usando ondas senoidales.
        /// </summary>
        private static string DrawWavySquareEye(double x, double y, double size, string color)
        {
            int teethPerSide = 8; // Ajusta para más o menos dientes
            double step = size / teethPerSide;
            double amplitude = size * 0.03; // Ajusta la altura de los dientes
            var sb = new StringBuilder();

            // Comienza en esquina superior izquierda, hacia afuera
            sb.Append($"<path d='");

            // Top side
            for (int i = 0; i < teethPerSide; i++)
            {
                double px1 = x + i * step;
                double py1 = y;
                double px2 = px1 + step / 2;
                double py2 = (i % 2 == 0) ? y - amplitude : y + amplitude;
                double px3 = x + (i + 1) * step;
                double py3 = y;
                if (i == 0) sb.Append($"M{px1},{py1} ");
                sb.Append($"L{px2},{py2} L{px3},{py3} ");
            }
            // Right side
            for (int i = 0; i < teethPerSide; i++)
            {
                double px1 = x + size;
                double py1 = y + i * step;
                double px2 = px1 + ((i % 2 == 0) ? amplitude : -amplitude);
                double py2 = py1 + step / 2;
                double px3 = x + size;
                double py3 = y + (i + 1) * step;
                sb.Append($"L{px2},{py2} L{px3},{py3} ");
            }
            // Bottom side
            for (int i = 0; i < teethPerSide; i++)
            {
                double px1 = x + size - i * step;
                double py1 = y + size;
                double px2 = px1 - step / 2;
                double py2 = (i % 2 == 0) ? y + size + amplitude : y + size - amplitude;
                double px3 = x + size - (i + 1) * step;
                double py3 = y + size;
                sb.Append($"L{px2},{py2} L{px3},{py3} ");
            }
            // Left side
            for (int i = 0; i < teethPerSide; i++)
            {
                double px1 = x;
                double py1 = y + size - i * step;
                double px2 = px1 - ((i % 2 == 0) ? amplitude : -amplitude);
                double py2 = py1 - step / 2;
                double px3 = x;
                double py3 = y + size - (i + 1) * step;
                sb.Append($"L{px2},{py2} L{px3},{py3} ");
            }
            sb.Append("Z' fill='" + color + "'/>");
            return sb.ToString();
        }


        public static (int width, int height) GetImageDimensionsFromBase64(string base64)
        {
            var base64Data = base64.Contains(",") ? base64.Substring(base64.IndexOf(",") + 1) : base64;
            byte[] imageBytes = Convert.FromBase64String(base64Data);
            using (var ms = new MemoryStream(imageBytes))
            using (var img = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(ms))
            {
                return (img.Width, img.Height);
            }
        }

        private static string DrawStar(double cx, double cy, double r, string color, int points = 5)
        {
            var sb = new StringBuilder();
            sb.Append($"<polygon fill='{color}' points='");

            for (int i = 0; i < points * 2; i++)
            {
                double angle = Math.PI / points * i;
                double radius = (i % 2 == 0) ? r : r * 0.5;
                double x = cx + radius * Math.Cos(angle - Math.PI / 2);
                double y = cy + radius * Math.Sin(angle - Math.PI / 2);
                sb.Append($"{x},{y} ");
            }

            sb.Append("'/>");
            return sb.ToString();
        }

        private static string DrawTriangle(double x, double y, double size, string color)
        {
            double cx = x + size / 2;
            double cy = y + size / 2;
            double r = size / 2;
            return $"<polygon points='{cx},{cy - r} {cx - r},{cy + r} {cx + r},{cy + r}' fill='{color}'/>";
        }

        private static string DrawCross(double x, double y, double size, string color)
        {
            double third = size / 3;
            var sb = new StringBuilder();
            sb.AppendLine($"<rect x='{x + third}' y='{y}' width='{third}' height='{size}' fill='{color}'/>");
            sb.AppendLine($"<rect x='{x}' y='{y + third}' width='{size}' height='{third}' fill='{color}'/>");
            return sb.ToString();
        }

        private static string DrawTeardrop(double x, double y, double size, string color)
        {
            double cx = x + size / 2;
            double cy = y + size / 2;
            double r = size / 2;
            return $"<path d='M{cx},{cy - r} Q{cx + r},{cy} {cx},{cy + r} Q{cx - r},{cy} {cx},{cy - r}Z' fill='{color}'/>";
        }

        private static string DrawWave(double x, double y, double size, string color)
        {
            double r = size / 2;
            return $"<path d='M{x},{y + r} Q{x + r / 2},{y - r / 2} {x + r},{y + r} T{x + size},{y + r} V{y + size} H{x} Z' fill='{color}'/>";
        }

        private static string DrawFlower(double x, double y, double size, string color)
        {
            double cx = x + size / 2, cy = y + size / 2, r = size / 3;
            var sb = new StringBuilder();
            sb.AppendLine($"<circle cx='{cx - r / 1.5}' cy='{cy}' r='{r}' fill='{color}'/>");
            sb.AppendLine($"<circle cx='{cx + r / 1.5}' cy='{cy}' r='{r}' fill='{color}'/>");
            sb.AppendLine($"<circle cx='{cx}' cy='{cy - r / 1.5}' r='{r}' fill='{color}'/>");
            sb.AppendLine($"<circle cx='{cx}' cy='{cy + r / 1.5}' r='{r}' fill='{color}'/>");
            return sb.ToString();
        }

        private static string DrawPixelBurst(double x, double y, double size, string color)
        {
            double cx = x + size / 2, cy = y + size / 2, r = size / 2;
            return $"<rect x='{x}' y='{y}' width='{size}' height='{size}' fill='{color}' rx='{size * 0.2}'/>";
        }

        private static string DrawInfinity(double x, double y, double size, string color)
        {
            double cx = x + size / 2, cy = y + size / 2, r = size / 3;
            return $"<path d='M{cx - r},{cy} C{cx - r / 2},{cy - r} {cx + r / 2},{cy + r} {cx + r},{cy} " +
                   $"C{cx + r / 2},{cy - r} {cx - r / 2},{cy + r} {cx - r},{cy} Z' fill='{color}'/>";
        }

        private static string DrawSnowflake(double x, double y, double size, string color)
        {
            double cx = x + size / 2, cy = y + size / 2, r = size / 2;
            var sb = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                double angle = i * Math.PI / 3;
                double x2 = cx + r * Math.Cos(angle);
                double y2 = cy + r * Math.Sin(angle);
                sb.AppendLine($"<line x1='{cx}' y1='{cy}' x2='{x2}' y2='{y2}' stroke='{color}' stroke-width='{size * 0.15}'/>");
            }
            return sb.ToString();
        }

        private static string DrawArrow(double x, double y, double size, string color)
        {
            double cx = x + size / 2;
            double cy = y + size / 2;
            double w = size * 0.6;   // ancho de la flecha
            double h = size * 0.9;   // alto de la flecha
            double head = size * 0.4; // altura de la punta

            return $@"
                <path d='
                    M {cx - w / 2},{cy + h / 2} 
                    L {cx - w / 2},{cy - head / 2}
                    L {cx - w},{cy - head / 2}
                    L {cx},{cy - h / 2}
                    L {cx + w},{cy - head / 2}
                    L {cx + w / 2},{cy - head / 2}
                    L {cx + w / 2},{cy + h / 2}
                    Z
                ' fill='{color}'/>
            ";
        }


        private static string DrawDotGrid(double x, double y, double size, string color)
        {
            var sb = new StringBuilder();
            double step = size / 3;
            double r = step / 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double cx = x + step * (i + 0.5);
                    double cy = y + step * (j + 0.5);
                    sb.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{r}' fill='{color}'/>");
                }
            }
            return sb.ToString();
        }


    }
}
