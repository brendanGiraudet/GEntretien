using GEntretien.Application.Services;
using Microsoft.AspNetCore.Components;

namespace GEntretien.Web.Features.Equipment.Components;

public partial class QRCodeComponent
{
    [Parameter]
    public int EquipmentId { get; set; }

    [Parameter]
    public string? EquipmentName { get; set; }

    [Inject]
    private IQRCodeService QRCodeService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private string QRCodeSvg { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        GenerateQRCode();
    }

    private void GenerateQRCode()
    {
        try
        {
            // Générer l'URL complète pour le QR code
            var baseUri = NavigationManager.BaseUri;
            var qrCodeUrl = $"{baseUri}equipment/view/{EquipmentId}";
            
            QRCodeSvg = QRCodeService.GenerateQRCodeSvg(qrCodeUrl);
        }
        catch (Exception ex)
        {
            QRCodeSvg = $"<text x=\"50%\" y=\"50%\" text-anchor=\"middle\">Erreur: {ex.Message}</text>";
        }
    }

    private async Task PrintQRCode()
    {
        // L'impression est gérée via JavaScript (à ajouter dans un fichier JS si nécessaire)
        // Pour maintenant, utiliser la fonction print du navigateur
        await Task.CompletedTask;
    }
}
