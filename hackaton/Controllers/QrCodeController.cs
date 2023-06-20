using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hackaton.Models;
using hackaton.Models.DAO;
using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;
using hackaton.Models.Caches;
using Microsoft.VisualStudio.Web.CodeGeneration;
using SixLabors.ImageSharp.Formats.Png;

namespace hackaton.Controllers
{
    public class QrCodeController : Controller
    {
        private readonly Context _context;
        private readonly QRCodeCacheService _qrCodeCacheService;

        public QrCodeController(Context context, QRCodeCacheService cache)
        {
            _context = context;
            _qrCodeCacheService = cache;
        }
        public IActionResult GenerateQrCode([FromBody]User user) {
            //  string text = "Testando QrCOde";
            // Define o texto para o QR Code

        
            string qrCodeText = Guid.NewGuid().ToString();
            
            if (string.IsNullOrEmpty(qrCodeText))
            {
                var retorno = Content("QR Code sem parametros");
                retorno.StatusCode = 500;
                return retorno;
               
            }

            if (string.IsNullOrEmpty(user.CPF))
            {
                return BadRequest();
            }
            Console.WriteLine("Texto: "+qrCodeText);
            Console.WriteLine("Tamanho: " + qrCodeText.Length);
           if (_qrCodeCacheService.GetQRCodeByContentAsync(qrCodeText) == null) {

                QrCode qr = new QrCode
                {
                    Content = qrCodeText,
                    UserId = user.Id,

                    TimeExpiration = DateTime.Now.AddSeconds(30).ToUniversalTime()//.AddMinutes(5)
                };


                _context.QrCodes.Add(qr);
                 _context.SaveChanges();
            }
           

            // Define o formato da imagem como PNG em um string base 64
            var imgType = Base64QRCode.ImageType.Png;

            //Instancia o Gerador de QrCode
            QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();

            //Gera o qrcode baseado no qrcodeText de tamanho médio
            QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);

            //Cria uma instancia de um gerador para gerar um qrCode Base64
            var qrCode = new Base64QRCode(qrCodeData);
          
            //Gera um qrcode em uma string base 64
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, SixLabors.ImageSharp.Color.Black, SixLabors.ImageSharp.Color.White, true, imgType);

            //Converte a string em formate base 64 para um array de bytes
            byte[] imageBytes = Convert.FromBase64String(qrCodeImageAsBase64);

            //retorna a imagem do QR Code no formato PNG
             return File(imageBytes, "image/png");

        }

        private bool QrCodeExists(int id)
        {
          return (_context.QrCodes?.Any(e => e.QRCodeId == id)).GetValueOrDefault();
        }
    }
}
