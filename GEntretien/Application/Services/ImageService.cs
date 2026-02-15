using Microsoft.AspNetCore.Components.Forms;

namespace GEntretien.Application.Services
{
    public interface IImageService
    {
        Task<(byte[]? data, string? fileName, string? contentType)> ProcessImageFileAsync(
            IBrowserFile file,
            long maxFileSize = 5242880); // 5MB par défaut
    }

    public class ImageService : IImageService
    {
        private static readonly string[] AllowedMimeTypes =
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        public async Task<(byte[]? data, string? fileName, string? contentType)> ProcessImageFileAsync(
            IBrowserFile file,
            long maxFileSize = 5242880)
        {
            try
            {
                // Vérifier l'extension du fichier
                var ext = Path.GetExtension(file.Name).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    throw new InvalidOperationException(
                        $"Type de fichier non autorisé: {ext}. Extensions autorisées: {string.Join(", ", AllowedExtensions)}");
                }

                // Vérifier le type MIME
                if (!AllowedMimeTypes.Contains(file.ContentType))
                {
                    throw new InvalidOperationException(
                        $"Type MIME non autorisé: {file.ContentType}");
                }

                // Vérifier la taille du fichier
                if (file.Size > maxFileSize)
                {
                    throw new InvalidOperationException(
                        $"La taille du fichier dépasse la limite: {file.Size} > {maxFileSize} bytes");
                }

                // Lire le fichier en mémoire
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(memoryStream);
                
                var imageData = memoryStream.ToArray();

                return (imageData, file.Name, file.ContentType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Erreur lors du traitement du fichier image: {ex.Message}", ex);
            }
        }
    }
}
