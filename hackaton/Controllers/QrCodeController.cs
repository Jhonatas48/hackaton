using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hackaton.Models;
using hackaton.Models.DAO;
using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;
using hackaton.Models.Caches;

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

        public IActionResult ver() { 
         
          return View();
        }

        public async Task<IActionResult> GenerateQrCode() {
          //  string text = "Testando QrCOde";
            // Define o texto para o QR Code
            string qrCodeText = Guid.NewGuid().ToString();
            
            if (string.IsNullOrEmpty(qrCodeText))
            {
                var retorno = Content("QR Code sem parametros");
                retorno.StatusCode = 500;
                return retorno;
               
            }
            Console.WriteLine("Texto: "+qrCodeText);
            Console.WriteLine("Tamanho: " + qrCodeText.Length);
           /* if (_qrCodeCacheService.GetQRCodeByContentAsync(qrCodeText) == null) {

                _context.QrCodes.Add(new QrCode{ Content = qrCodeText });
                await _context.SaveChangesAsync();
            }
           */
            // Configurações do QR Code
            byte[] imageBytes;

            using (MemoryStream stream = new MemoryStream()) { 

                QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(qrCodeText,QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qRCodeData);
                
                //Cria um bitmap baseada no tamnho de 20 pixels
                using (Bitmap qrCodeBitmap = qrCode.GetGraphic(20)) {

                    //Salva a imagem no bitmat no formatp PNG
                    qrCodeBitmap.Save(stream,ImageFormat.Png);
                  
                    //Converte a imagem em um array de bytes
                    imageBytes = stream.ToArray();
                    
                }
            }

            //retorna a imagem do QR Code
            return File(imageBytes, "image/png");

        }

        // GET: QrCode
        public async Task<IActionResult> Index()
        {
              return _context.QrCodes != null ? 
                          View(await _context.QrCodes.ToListAsync()) :
                          Problem("Entity set 'Context.QrCode'  is null.");
        }

        // GET: QrCode/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QrCodes == null)
            {
                return NotFound();
            }

            var qrCode = await _context.QrCodes
                .FirstOrDefaultAsync(m => m.QRCodeId == id);
            if (qrCode == null)
            {
                return NotFound();
            }

            return View(qrCode);
        }

        // GET: QrCode/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QrCode/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QrCode qrCode)
        {
           
             _context.Add(qrCode);
             _context.SaveChanges();
            
            return View(qrCode);
        }

        // GET: QrCode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.QrCodes == null)
            {
                return NotFound();
            }

            var qrCode = await _context.QrCodes.FindAsync(id);
            if (qrCode == null)
            {
                return NotFound();
            }
            return View(qrCode);
        }

        // POST: QrCode/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QrCode qrCode)
        {
            if (id != qrCode.QRCodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qrCode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QrCodeExists(qrCode.QRCodeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(qrCode);
        }

        // GET: QrCode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.QrCodes == null)
            {
                return NotFound();
            }

            var qrCode = await _context.QrCodes
                .FirstOrDefaultAsync(m => m.QRCodeId == id);
            if (qrCode == null)
            {
                return NotFound();
            }

            return View(qrCode);
        }

        // POST: QrCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.QrCodes == null)
            {
                return Problem("Entity set 'Context.QrCode'  is null.");
            }
            var qrCode = await _context.QrCodes.FindAsync(id);
            if (qrCode != null)
            {
                _context.QrCodes.Remove(qrCode);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QrCodeExists(int id)
        {
          return (_context.QrCodes?.Any(e => e.QRCodeId == id)).GetValueOrDefault();
        }
    }
}
