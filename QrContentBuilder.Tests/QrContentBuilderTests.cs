using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CustomizableQrCode.Test
{
    public class QrContentBuilderTests
    {
        [Fact]
        public void BuildLink_Returns_Url()
        {
            var url = "https://ejemplo.com";
            Assert.Equal(url, QrContentBuilder.BuildLink(url));
        }

        [Fact]
        public void BuildText_Returns_Text()
        {
            var txt = "Texto QR";
            Assert.Equal(txt, QrContentBuilder.BuildText(txt));
        }

        [Fact]
        public void BuildEmail_Builds_Mailto()
        {
            var result = QrContentBuilder.BuildEmail("to@mail.com", "Asunto", "Cuerpo");
            Assert.StartsWith("mailto:to@mail.com", result);
            Assert.Contains("subject=", result);
            Assert.Contains("body=", result);
        }

        [Fact]
        public void BuildCall_Builds_Tel()
        {
            Assert.Equal("tel:5551234567", QrContentBuilder.BuildCall("5551234567"));
        }

        [Fact]
        public void BuildSMS_Builds_SmsWithBody()
        {
            var result = QrContentBuilder.BuildSMS("555", "Hola mundo!");
            Assert.Equal("sms:555?body=Hola%20mundo%21", result);
        }

        [Fact]
        public void BuildVCard_Generates_Valid_Format()
        {
            var result = QrContentBuilder.BuildVCard("Juan", "Pérez", "555123", "jp@mail.com", "ACME", "CTO", "Calle X");
            Assert.Contains("BEGIN:VCARD", result);
            Assert.Contains("FN:Juan Pérez", result);
            Assert.Contains("TEL:555123", result);
            Assert.Contains("END:VCARD", result);
        }

        [Fact]
        public void BuildWhatsApp_Generates_Valid_Url()
        {
            var url = QrContentBuilder.BuildWhatsApp("123456", "Hola!");
            Assert.Equal("https://wa.me/123456?text=Hola%21", url);
            Assert.Equal("https://wa.me/123456", QrContentBuilder.BuildWhatsApp("123456", ""));
        }

        [Fact]
        public void BuildWiFi_Generates_Valid_Format()
        {
            var wifi = QrContentBuilder.BuildWiFi("Red", "clave123", "WPA");
            Assert.Equal("WIFI:T:WPA;S:Red;P:clave123;;", wifi);
        }

        [Fact]
        public void BuildPDF_Returns_Url()
        {
            var pdf = "https://dominio.com/archivo.pdf";
            Assert.Equal(pdf, QrContentBuilder.BuildPDF(pdf));
        }

        [Fact]
        public void BuildApp_Returns_Url()
        {
            var app = "https://app.com";
            Assert.Equal(app, QrContentBuilder.BuildApp(app));
        }

        [Fact]
        public void BuildImage_Returns_Url()
        {
            var img = "https://img.com/pic.png";
            Assert.Equal(img, QrContentBuilder.BuildImage(img));
        }

        [Fact]
        public void BuildVideo_Returns_Url()
        {
            var vid = "https://video.com/v";
            Assert.Equal(vid, QrContentBuilder.BuildVideo(vid));
        }

        [Fact]
        public void BuildSocial_Returns_Url()
        {
            var social = "https://twitter.com/user";
            Assert.Equal(social, QrContentBuilder.BuildSocial(social));
        }

        [Fact]
        public void BuildEvent_Generates_Valid_Format()
        {
            var ev = QrContentBuilder.BuildEvent("Evento", "CDMX", "20250701T100000", "20250701T120000");
            Assert.Contains("BEGIN:VEVENT", ev);
            Assert.Contains("SUMMARY:Evento", ev);
            Assert.Contains("DTSTART:20250701T100000", ev);
            Assert.Contains("END:VEVENT", ev);
        }

        [Fact]
        public void BuildBarcode2D_Returns_Content()
        {
            Assert.Equal("ABC123", QrContentBuilder.BuildBarcode2D("ABC123"));
        }

        [Fact]
        public void BuildLink_Returns_Empty_For_Null_Or_Empty()
        {
            Assert.Equal("", QrContentBuilder.BuildLink(null));
            Assert.Equal("", QrContentBuilder.BuildLink(""));
        }

        [Fact]
        public void BuildEmail_Returns_Empty_If_To_Is_Null_Or_Empty()
        {
            Assert.Equal("", QrContentBuilder.BuildEmail(null, "asunto", "msg"));
            Assert.Equal("", QrContentBuilder.BuildEmail("", "asunto", "msg"));
        }

        [Fact]
        public void BuildWiFi_Returns_Empty_If_SSID_Is_Null_Or_Empty()
        {
            Assert.Equal("", QrContentBuilder.BuildWiFi(null, "pass", "WPA"));
            Assert.Equal("", QrContentBuilder.BuildWiFi("", "pass", "WPA"));
        }

        [Fact]
        public void BuildEvent_Returns_Empty_If_Title_Is_Null_Or_Empty()
        {
            Assert.Equal("", QrContentBuilder.BuildEvent(null, "loc", "20250101", "20250102"));
            Assert.Equal("", QrContentBuilder.BuildEvent("", "loc", "20250101", "20250102"));
        }

        [Fact]
        public void IsValidEmail_Works()
        {
            Assert.True(QrContentBuilder.IsValidEmail("correo@dominio.com"));
            Assert.False(QrContentBuilder.IsValidEmail("no-email"));
        }

        [Fact]
        public void IsValidPhone_Works()
        {
            Assert.True(QrContentBuilder.IsValidPhone("+521234567890"));
            Assert.False(QrContentBuilder.IsValidPhone("abc-1234"));
        }

        [Fact]
        public void IsValidUrl_Works()
        {
            Assert.True(QrContentBuilder.IsValidUrl("https://dominio.com"));
            Assert.False(QrContentBuilder.IsValidUrl("ftp://dominio.com"));
            Assert.False(QrContentBuilder.IsValidUrl("not a url"));
        }
    }

}
