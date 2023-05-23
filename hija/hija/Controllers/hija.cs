using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using System.IO;



namespace madre.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class hija : ControllerBase
    {
        private readonly HttpClient httpClient;

        public hija()
        {
            httpClient = new HttpClient();
        }

        [HttpGet]
        public ActionResult<string> Saludar()
        {
            return Ok("Bienvenido al servicio de la hija");
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerDato(int id)
        {
            // Obtener y retornar el dato con el ID especificado para la hija
            // ...
                
            // Ejemplo: Crear un objeto JSON
            var dato = new { Id = id, Nombre = "Ejemplo para la hija" };

            return Ok(dato);
        }


        [HttpPost]
        public IActionResult CrearDato([FromBody] DatoModelo dato)
        {
            // Crear el nuevo dato utilizando la información recibida en el cuerpo de la solicitud para la hija
            // ...

            return Ok("Dato creado exitosamente para la hija");
        }

        [HttpPut("{id}")]
        public IActionResult ActualizarDato(int id, [FromBody] DatoModelo dato)
        {
            // Actualizar el dato con el ID especificado utilizando la información recibida en el cuerpo de la solicitud para la hija
            // ...

            return Ok("Dato actualizado exitosamente para la hija");
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarDato(int id)
        {
            // Eliminar el dato con el ID especificado para la hija
            // ...

            return Ok("Dato eliminado exitosamente para la hija");
        }

        // Agrega métodos adicionales específicos para la hija si es necesario

        // ...
        [HttpPost("ejecutar-programa")]
        public IActionResult EjecutarPrograma()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    process.WaitForInputIdle();

                    var processId = process.Id;

                    return Ok(new
                    {
                        ProcessId = processId,
                        Message = $"El programa Notepad se ha iniciado correctamente con el PID {processId}."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = $"Error al ejecutar el programa: {ex.Message}"
                });
            }
        }

        [HttpDelete("cerrar-programa/{pid}")]
        public IActionResult CerrarPrograma(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                var processName = process.ProcessName;
                var uptime = DateTime.Now - process.StartTime;
                var alive = DateTime.Now;

                process.Kill();

                var response = new
                {
                    Message = $"Programa '{processName}' cerrado exitosamente",
                    Uptime = uptime,
                    Alive = alive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error al cerrar el programa: {ex.Message}" });
            }
        }


        [HttpPost("apagar")]
        public IActionResult Apagar()
        {
            try
            {
                // Ejecutar el comando de apagado en la máquina madre
                using (var process = new Process())
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "Stop-Computer -Force",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }

                return Ok("Se ha enviado la solicitud de apagado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al enviar la solicitud de apagado: {ex.Message}");
            }
        }

        [HttpGet("getpadre")]
        public IActionResult GetPadre()
        {
            try
            {
                string filePath = Path.Combine("data", "jsonPadre.json");
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

                if (System.IO.File.Exists(fullPath))
                {
                    string json = System.IO.File.ReadAllText(fullPath);
                    return Ok(json);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error al obtener el archivo: {ex.Message}" });
            }
        }

        // ...

        [HttpGet("specific-son")]
        public IActionResult SpecificSon(string clave)
        {
            try
            {
                var jsonPadrePath = "./data/jsonPadre.json";
                var jsonPadreContent = System.IO.File.ReadAllText(jsonPadrePath);
                var jsonPadre = JObject.Parse(jsonPadreContent);

                var juegos = jsonPadre["juegos"];
                var juego = juegos.FirstOrDefault(j => j["clave"].ToString() == clave);

                if (juego != null)
                {
                    var path = juego["path"].ToString();
                    var jsonSonPath = "./data" + path;
                    var jsonSonContent = System.IO.File.ReadAllText(jsonSonPath);

                    return Content(jsonSonContent, "application/json");
                }

                return NotFound("No se encontró el juego correspondiente a la clave especificada.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }


        [HttpGet("getpadre/{etiqueta}")]
        public IActionResult GetPadrePorEtiqueta(string etiqueta)
        {
            try
            {
                string filePath = Path.Combine("data", "jsonPadre.json");
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

                if (System.IO.File.Exists(fullPath))
                {
                    string json = System.IO.File.ReadAllText(fullPath);
                    var jsonPadre = JObject.Parse(json);

                    var juegos = jsonPadre["juegos"];
                    var juegosFiltrados = juegos.Where(j => j["tags"].Any(t => t.ToString() == etiqueta)).ToList();

                    if (juegosFiltrados.Count > 0)
                    {
                        jsonPadre["juegos"] = new JArray(juegosFiltrados);
                        return Ok(jsonPadre.ToString());
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error al obtener el archivo: {ex.Message}" });
            }
        }




        [HttpPost("agregarprograma/{clave}")]
        public async Task<IActionResult> AgregarPrograma(string clave, [FromBody] ProgramaData data)
        {
            try
            {
                // Obtener la IP de la madre del archivo computadoras.json
                string computadorasPath = Path.Combine("data", "computadoras.json");
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, computadorasPath);

                if (System.IO.File.Exists(fullPath))
                {
                    string jsonComputadoras = System.IO.File.ReadAllText(fullPath);
                    var computadoras = JObject.Parse(jsonComputadoras);
                    var madre = computadoras["madre"];

                    if (madre != null)
                    {
                        string ipMadre = madre["Ip"].ToString();

                        // Obtener información del juego utilizando la clave
                        var jsonPadrePath = Path.Combine("data", "jsonPadre.json");
                        var jsonPadreContent = System.IO.File.ReadAllText(jsonPadrePath);
                        var jsonPadre = JObject.Parse(jsonPadreContent);

                        var juegos = jsonPadre["juegos"];
                        var juego = juegos.FirstOrDefault(j => j["clave"].ToString() == clave);

                        if (juego != null)
                        {
                            var nombreJuego = juego["nombre"].ToString();
                            var pathJuego = juego["path"].ToString();

                            // Crear el contenido para el bloc de notas con la información del juego
                            string contenidoBloc = $"{nombreJuego}{Environment.NewLine}{Environment.NewLine}" +
                                $"Descripción: {data.Descripcion}{Environment.NewLine}" +
                                $"Tiempo de juego: {data.TiempoDeJuego}{Environment.NewLine}";

                            // Iniciar el bloc de notas con el contenido generado
                            var startInfo = new ProcessStartInfo
                            {
                                FileName = "notepad.exe",
                                Arguments = contenidoBloc,
                                CreateNoWindow = true,
                                UseShellExecute = false
                            };

                            using (var process = new Process { StartInfo = startInfo })
                            {
                                process.Start();
                                process.WaitForInputIdle();

                                var processId = process.Id;

                                // Crear el objeto con los datos para enviar en la solicitud POST
                                var postData = new
                                {
                                    PID = processId,
                                    Timestamp = DateTime.UtcNow,
                                    IP = ipMadre,
                                    TiempoDeJuego = data.TiempoDeJuego,
                                    NombreJuego = nombreJuego
                                };

                                // Convertir el objeto de datos a JSON
                                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);

                                // Crear la solicitud POST y configurar la URL y el contenido JSON
                                var request = new HttpRequestMessage
                                {
                                    Method = HttpMethod.Post,
                                    RequestUri = new Uri($"http://{ipMadre}/api/madre/agregarprograma"),
                                    Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json")
                                };

                                // Enviar la solicitud y obtener la respuesta
                                var response = await httpClient.SendAsync(request);

                                if (response.IsSuccessStatusCode)
                                {
                                    // La solicitud fue exitosa
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    return Ok(responseContent);
                                }
                                else
                                {
                                    // La solicitud falló
                                    return StatusCode((int)response.StatusCode, "Error al enviar la solicitud: " + response.StatusCode);
                                }
                            }
                        }
                        else
                        {
                            return NotFound("No se encontró el juego correspondiente a la clave especificada.");
                        }
                    }
                    else
                    {
                        return NotFound("No se encontró la IP de la madre en el archivo computadoras.json.");
                    }
                }
                else
                {
                    return NotFound("No se encontró el archivo computadoras.json.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }





    }
    public class DatoModelo
    {
        // Propiedades del modelo de datos
        // ...
    }
    public class ProgramaData
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int TiempoDeJuego { get; set; }
    }

}
