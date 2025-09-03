using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomizableQrCode
{
    public static class QrContentBuilder
    {
        // 🔹 URL / Texto
        public static string BuildLink(string url) =>
            string.IsNullOrWhiteSpace(url) ? "" : url.Trim();

        public static string BuildText(string text) =>
            string.IsNullOrWhiteSpace(text) ? "" : text.Trim();

        // 🔹 Email
        public static string BuildEmail(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to)) return "";

            subject ??= "";
            body ??= "";

            var subjectParam = string.IsNullOrWhiteSpace(subject) ? "" : $"subject={Uri.EscapeDataString(subject)}";
            var bodyParam = string.IsNullOrWhiteSpace(body) ? "" : $"body={Uri.EscapeDataString(body)}";
            var sep = (!string.IsNullOrEmpty(subjectParam) && !string.IsNullOrEmpty(bodyParam)) ? "&" : "";
            var query = (subjectParam + sep + bodyParam);

            return $"mailto:{to}" + (string.IsNullOrEmpty(query) ? "" : $"?{query}");
        }

        // 🔹 Llamada
        public static string BuildCall(string phone) =>
            string.IsNullOrWhiteSpace(phone) ? "" : $"tel:{phone}";

        // 🔹 SMS
        public static string BuildSMS(string phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            message ??= "";
            return $"sms:{phone}{(string.IsNullOrWhiteSpace(message) ? "" : $"?body={Uri.EscapeDataString(message)}")}";
        }

        // 🔹 VCard
        public static string BuildVCard(string firstName, string lastName, string phone, string email, string company, string job, string address)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:3.0");
            sb.AppendLine($"N:{lastName ?? ""};{firstName ?? ""}");
            sb.AppendLine($"FN:{(firstName ?? "")} {(lastName ?? "")}".Trim());
            if (!string.IsNullOrWhiteSpace(company)) sb.AppendLine($"ORG:{company}");
            if (!string.IsNullOrWhiteSpace(job)) sb.AppendLine($"TITLE:{job}");
            if (!string.IsNullOrWhiteSpace(phone)) sb.AppendLine($"TEL:{phone}");
            if (!string.IsNullOrWhiteSpace(email)) sb.AppendLine($"EMAIL:{email}");
            if (!string.IsNullOrWhiteSpace(address)) sb.AppendLine($"ADR:{address}");
            sb.AppendLine("END:VCARD");
            return sb.ToString();
        }

        // 🔹 WhatsApp
        public static string BuildWhatsApp(string phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            message ??= "";
            return string.IsNullOrWhiteSpace(message)
                ? $"https://wa.me/{phone}"
                : $"https://wa.me/{phone}?text={Uri.EscapeDataString(message)}";
        }

        // 🔹 WiFi
        public static string BuildWiFi(string ssid, string password, string encryption)
        {
            if (string.IsNullOrWhiteSpace(ssid)) return "";
            encryption ??= "nopass";
            password ??= "";
            return $"WIFI:T:{encryption};S:{ssid};P:{password};;";
        }

        // 🔹 PDF / App / Imagen / Video / Social
        public static string BuildPDF(string url) =>
            string.IsNullOrWhiteSpace(url) ? "" : url.Trim();

        public static string BuildApp(string url) =>
            string.IsNullOrWhiteSpace(url) ? "" : url.Trim();

        public static string BuildImage(string url) =>
            string.IsNullOrWhiteSpace(url) ? "" : url.Trim();

        public static string BuildVideo(string url) =>
            string.IsNullOrWhiteSpace(url) ? "" : url.Trim();

        public static string BuildSocial(string url) =>
            string.IsNullOrWhiteSpace(url) ? "" : url.Trim();

        // 🔹 Evento
        public static string BuildEvent(string title, string location, string start, string end)
        {
            if (string.IsNullOrWhiteSpace(title)) return "";
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"SUMMARY:{title}");
            if (!string.IsNullOrWhiteSpace(location)) sb.AppendLine($"LOCATION:{location}");
            if (!string.IsNullOrWhiteSpace(start)) sb.AppendLine($"DTSTART:{start}");
            if (!string.IsNullOrWhiteSpace(end)) sb.AppendLine($"DTEND:{end}");
            sb.AppendLine("END:VEVENT");
            return sb.ToString();
        }

        // 🔹 Barcode2D
        public static string BuildBarcode2D(string content) =>
            string.IsNullOrWhiteSpace(content) ? "" : content.Trim();

        // 🔹 Validadores
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^\+?\d{7,15}$");
        }

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out var temp)
                   && (temp.Scheme == Uri.UriSchemeHttp || temp.Scheme == Uri.UriSchemeHttps);
        }
    }
}
