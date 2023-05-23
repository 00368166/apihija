using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

using System.IO;



namespace madre.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class hija : ControllerBase
    {
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
                    var juegosFiltrados = juegos.Where(j => j["tags"].Contains(etiqueta)).ToList();
                    jsonPadre["juegos"] = new JArray(juegosFiltrados);

                    return Ok(jsonPadre.ToString());
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



    }
    public class DatoModelo
    {
        // Propiedades del modelo de datos
        // ...
    }
}
