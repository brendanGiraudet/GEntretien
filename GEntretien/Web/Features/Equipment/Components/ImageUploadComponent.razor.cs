using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using GEntretien.Application.Services;

namespace GEntretien.Web.Features.Equipment.Components;

public partial class ImageUploadComponent : IAsyncDisposable
{
    [Parameter]
    public byte[]? ImageData { get; set; }

    [Parameter]
    public string? ImageFileName { get; set; }

    [Parameter]
    public string? ImageContentType { get; set; }

    [Parameter]
    public EventCallback<(byte[]? data, string? fileName, string? contentType)> OnImageChanged { get; set; }

    [Inject]
    private IImageService ImageService { get; set; } = default!;

    private string? StatusMessage { get; set; }
    private string StatusClass { get; set; } = "alert-danger";
    private IBrowserFile? _selectedFile;

    public bool HasImage => ImageData?.Length > 0;

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        StatusMessage = null;
        
        try
        {
            _selectedFile = e.File;
            
            var (data, fileName, contentType) = await ImageService.ProcessImageFileAsync(_selectedFile);
            
            ImageData = data;
            ImageFileName = fileName;
            ImageContentType = contentType;
            
            await OnImageChanged.InvokeAsync((data, fileName, contentType));
            
            StatusMessage = "Image téléchargée avec succès.";
            StatusClass = "alert-success";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
            StatusClass = "alert-danger";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erreur: {ex.Message}";
            StatusClass = "alert-danger";
        }
    }

    private async Task RemoveImage()
    {
        ImageData = null;
        ImageFileName = null;
        ImageContentType = null;
        _selectedFile = null;
        
        await OnImageChanged.InvokeAsync((null, null, null));
        
        StatusMessage = "Image supprimée.";
        StatusClass = "alert-info";
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_selectedFile is not null)
        {
            await _selectedFile.OpenReadStream().DisposeAsync();
        }
    }
}
