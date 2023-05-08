using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCubosExamenDDB.Models {

    public class Cubo {

        public int IdCubo { get; set; }


        public string Nombre { get; set; }


        public string Marca { get; set; }


        public string Imagen { get; set; }


        public int Precio { get; set; }

    }
}
