using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using MvcCubosExamenDDB.Filters;
using MvcCubosExamenDDB.Models;
using MvcCubosExamenDDB.Services;

namespace MvcCubosExamenDDB.Controllers {
    public class UserController : Controller {

        private ServiceCubos service;
        private ServiceStorageBlob serviceBlob;

        public UserController(ServiceCubos service, ServiceStorageBlob serviceBlob) {
            this.service = service;
            this.serviceBlob = serviceBlob;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Perfil() {
            string containerName = "usuarios";
            string blobName = HttpContext.User.FindFirst("IMAGEN").Value;

            if (blobName != null) {
                BlobContainerClient blobContainerClient = await this.serviceBlob.GetContainerAsync(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddHours(1),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                var uri = blobClient.GenerateSasUri(sasBuilder);
                ViewData["URI"] = uri;
            }

            return View();
        }

        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario, IFormFile file) {
            string blobName = usuario.Email + file.FileName;
            using (Stream stream = file.OpenReadStream()) {
                await this.serviceBlob.UploadBlobAsync("usuarios", blobName, stream);
            }

            await this.service.RegisterUser(usuario.Nombre, usuario.Pass, usuario.Email, blobName);
            return RedirectToAction("Perfil");
        }
    }
}
