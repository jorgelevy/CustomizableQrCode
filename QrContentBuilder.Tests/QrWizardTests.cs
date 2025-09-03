using Bunit;
using CustomizableQrCode.DemoBlazor.Components.Blazor;
using Xunit;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.Tests
{
    public class QrWizardTests : TestContext
    {
        [Fact]
        public void Renderiza_QrWizard_ConLink()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            var comp = ctx.RenderComponent<QrWizard>(parameters => parameters
                .Add(p => p.SelectedType, QrContentType.Link)
                .Add(p => p.LinkUrl, "https://midominio.com")
            );

            Assert.Contains("https://midominio.com", comp.Markup);
        }

        [Fact]
        public void Renderiza_QrWizard_ConTexto()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            var comp = ctx.RenderComponent<QrWizard>(parameters => parameters
                .Add(p => p.SelectedType, QrContentType.Text)
                .Add(p => p.TextContent, "Hola mundo!")
            );

            Assert.Contains("Hola mundo!", comp.Markup);
        }

        [Fact]
        public void Renderiza_QrWizard_ConEmail()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            var comp = ctx.RenderComponent<QrWizard>(parameters => parameters
                .Add(p => p.SelectedType, QrContentType.Email)
                .Add(p => p.EmailTo, "correo@dominio.com")
                .Add(p => p.EmailSubject, "Asunto")
                .Add(p => p.EmailBody, "Cuerpo del correo")
            );

            Assert.Contains("correo@dominio.com", comp.Markup);
            Assert.Contains("Asunto", comp.Markup);
        }
    }
}
