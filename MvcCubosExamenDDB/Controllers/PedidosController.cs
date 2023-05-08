using Microsoft.AspNetCore.Mvc;
using MvcCubosExamenDDB.Filters;
using MvcCubosExamenDDB.Models;
using MvcCubosExamenDDB.Services;

namespace MvcCubosExamenDDB.Controllers {
    public class PedidosController : Controller {

        private ServiceCubos service;

        public PedidosController(ServiceCubos service) {
            this.service = service;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Pedidos() {
            List<Pedido> list = await this.service.PedidosUsuario();
            return View(list);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Comprar(int idcubo) {
            await this.service.RealizarPedido(idcubo);
            return RedirectToAction("Perfil", "User");
        }
    }
}
