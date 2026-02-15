using QRCoder;

namespace GEntretien.Application.Services
{
    public interface IQRCodeService
    {
        string GenerateQRCodeSvg(string data, int pixelsPerModule = 10);
    }

    public class QRCodeService : IQRCodeService
    {
        public string GenerateQRCodeSvg(string data, int pixelsPerModule = 10)
        {
            try
            {
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new SvgQRCode(qrCodeData))
                    {
                        var svgString = qrCode.GetGraphic(pixelsPerModule);
                        return svgString;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erreur lors de la génération du QR code: {ex.Message}", ex);
            }
        }
    }
}
