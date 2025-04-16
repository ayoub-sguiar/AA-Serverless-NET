using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ResizeFunction
{
    public static class ResizeHttpTrigger
    {
        [Function("ResizeHttpTrigger")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("ResizeHttpTrigger");

            try
            {
                var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

                // Récupérer les dimensions depuis l'URL
                if (!int.TryParse(query["w"], out int width) || !int.TryParse(query["h"], out int height))
                {
                    var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    badResponse.WriteString("Paramètres 'w' et 'h' requis (entiers).");
                    return badResponse;
                }

                byte[] resizedImageBytes;
                using (var msInput = new MemoryStream())
                {
                    await req.Body.CopyToAsync(msInput);
                    msInput.Position = 0;

                    using (var image = Image.Load(msInput))
                    {
                        image.Mutate(x => x.Resize(width, height));

                        using (var msOutput = new MemoryStream())
                        {
                            image.Save(msOutput, new JpegEncoder());
                            resizedImageBytes = msOutput.ToArray();
                        }
                    }
                }

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "image/jpeg");
                await response.Body.WriteAsync(resizedImageBytes, 0, resizedImageBytes.Length);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError($"Erreur : {ex.Message}");
                var error = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                error.WriteString("Erreur interne lors du traitement de l'image.");
                return error;
            }
        }
    }
}
