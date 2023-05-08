using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcCubosExamenDDB.Filters;
using MvcCubosExamenDDB.Models;
using MvcCubosExamenDDB.Services;

namespace MvcCubosExamenDDB.Controllers {
    public class CubosController : Controller {

        private ServiceCubos service;
        private ServiceStorageBlob serviceBlob;
        private string containerName = "cubos";

        public CubosController(ServiceCubos service, ServiceStorageBlob serviceBlob) {
            this.service = service;
            this.serviceBlob = serviceBlob;
        }

        public async Task<IActionResult> Index() {
            List<Cubo> list = await this.service.GetCubos();
            foreach (Cubo c in list) {
                string blobName = c.Imagen;
                if (blobName != null) {
                    BlobContainerClient blobContainerClient = await this.serviceBlob.GetContainerAsync(this.containerName);
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                    BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                        BlobContainerName = this.containerName,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1),
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blobClient.GenerateSasUri(sasBuilder);
                    c.Imagen = uri.ToString();
                }
            }
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string marca) {
            List<Cubo> list = await this.service.GetCubosMarca(marca);
            foreach (Cubo c in list) {
                string blobName = c.Imagen;
                if (blobName != null) {
                    BlobContainerClient blobContainerClient = await this.serviceBlob.GetContainerAsync(this.containerName);
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                    BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                        BlobContainerName = this.containerName,
                        BlobName = blobName,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1),
                    };

                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blobClient.GenerateSasUri(sasBuilder);
                    c.Imagen = uri.ToString();
                }
            }
            return View(list);
        }

        [AuthorizeUsers]
        public IActionResult Insert() {
            return View();
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> Insert(Cubo cubo, IFormFile file) {
            string blobName = file.FileName;
            cubo.Imagen = blobName;

            using (Stream stream = file.OpenReadStream()) {
                await this.serviceBlob.UploadBlobAsync(this.containerName, blobName, stream);
            }
            await this.service.InsertCubo(cubo);
            return RedirectToAction("Index");
        }
    }
}
