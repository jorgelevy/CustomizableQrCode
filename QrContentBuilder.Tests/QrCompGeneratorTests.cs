using Bunit;
using CustomizableQrCode.DemoBlazor.Components.Blazor;
using Xunit;

namespace CustomizableQrCode.Tests
{
    public class QrCompGeneratorTests : TestContext
    {
        [Fact]
        public void Renderiza_Componente_SinError()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            var comp = ctx.RenderComponent<QrCompGenerator>();
            Assert.NotNull(comp.Markup);
        }

        [Fact]
        public void GeneraQr_Link_FlujoCompleto()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            var comp = ctx.RenderComponent<QrCompGenerator>();
            var input = comp.Find("input[value='https://midominio.com']");
            input.Change("https://google.com");
            comp.Find("button:contains('Generar')").Click();

            Assert.Contains("google.com", comp.Markup);
        }

        [Fact]
        public void ExportQr_AsPdf_Dispara_JSInterop()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            ctx.JSInterop.SetupVoid("exportQrAsPdf", _ => true);

            var comp = ctx.RenderComponent<QrCompGenerator>();
            comp.Find("button:contains('Descargar PDF')").Click();

            ctx.JSInterop.VerifyInvoke("exportQrAsPdf");
        }
    }
}
