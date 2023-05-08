using MvcCubosExamenDDB.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MvcCubosExamenDDB.Services {
    public class ServiceCubos {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;


        public ServiceCubos(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiMusica");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            _httpContextAccessor = httpContextAccessor;
        }

        #region GENERAL
        private async Task<T> CallApiAsync<T>(string request) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                HttpResponseMessage response = await client.GetAsync(request);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : default(T);
            }
        }

        private async Task<HttpStatusCode> InsertApiAsync<T>(string request, T objeto) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                return response.StatusCode;
            }
        }
        #endregion

        #region GENERAL TOKEN
        private async Task<T> CallApiAsync<T>(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : default(T);
            }
        }

        private async Task<HttpStatusCode> InsertApiAsync<T>(string request, T objeto, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                return response.StatusCode;
            }
        }


        #endregion

        #region TOKEN
        public async Task<string?> GetToken(string user, string pass) {
            LogIn model = new LogIn { Name = user, Password = pass };
            string token = "";

            using (HttpClient client = new HttpClient()) {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi + request);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode) {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    token = jsonObject.GetValue("response").ToString();
                }
                return (token != "") ? token : null;
            }
        }
        #endregion

        #region USUARIO
        public async Task RegisterUser(string name, string password, string email, string image) {
            Usuario newUser = new Usuario {
                IdUsuario = 0,
                Nombre = name,
                Pass = password,
                Email = email,
                Imagen = image
            };
            string request = "/api/auth/Register";
            await this.InsertApiAsync<Usuario>(request, newUser);
        }

        public async Task<Usuario> GetPerfil() {
            string request = "/api/usuarios/getperfil";
            string token = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Usuario>(request, token);
        }
        #endregion

        #region CUBOS
        public async Task<List<Cubo>> GetCubos() {
            string request = "/api/cubos/getcubos";
            return await this.CallApiAsync<List<Cubo>>(request);
        }

        public async Task<List<Cubo>> GetCubosMarca(string marca) {
            string request = "/api/cubos/getcubosmarca/" + marca;
            return await this.CallApiAsync<List<Cubo>>(request);
        }

        public async Task<HttpStatusCode> InsertCubo(Cubo cubo) {
            string request = "/api/cubos/insertcubo";
            return await this.InsertApiAsync<Cubo>(request, cubo);
        }
        #endregion


        #region PEDIDOS
        public async Task<HttpStatusCode> RealizarPedido(int idcubo) {
            string request = "/api/pedidos/realizarpedido?idCubo=" + idcubo;
            string token = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.InsertApiAsync<Pedido>(request, null, token);
        }

        public async Task<List<Pedido>> PedidosUsuario() {
            string request = "/api/pedidos/pedidosusuario";
            string token = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Pedido>>(request, token);
        }
        #endregion

    }
}
