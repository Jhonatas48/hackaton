using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hackaton.Models;
using hackaton.Models.DAO;
using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;

namespace hackaton.Controllers
{
    public class QrCodeController : Controller
    {
        private readonly Context _context;

        public QrCodeController(Context context)
        {
            _context = context;
        }

        public IActionResult ver() { 
         
          return View();
        }

        public IActionResult GenerateQrCode() {
          //  string text = "Testando QrCOde";
            // Define o texto para o QR Code
            string qrCodeText = "https://www.google.com.br";
           

            if (string.IsNullOrEmpty(qrCodeText))
            {
                var retorno = Content("QR Code sem parametros");
                retorno.StatusCode = 500;
                return retorno;
               
            }


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
              return _context.QrCode != null ? 
                          View(await _context.QrCode.ToListAsync()) :
                          Problem("Entity set 'Context.QrCode'  is null.");
        }

        // GET: QrCode/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QrCode == null)
            {
                return NotFound();
            }

            var qrCode = await _context.QrCode
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
            if (id == null || _context.QrCode == null)
            {
                return NotFound();
            }

            var qrCode = await _context.QrCode.FindAsync(id);
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
            if (id == null || _context.QrCode == null)
            {
                return NotFound();
            }

            var qrCode = await _context.QrCode
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
            if (_context.QrCode == null)
            {
                return Problem("Entity set 'Context.QrCode'  is null.");
            }
            var qrCode = await _context.QrCode.FindAsync(id);
            if (qrCode != null)
            {
                _context.QrCode.Remove(qrCode);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QrCodeExists(int id)
        {
          return (_context.QrCode?.Any(e => e.QRCodeId == id)).GetValueOrDefault();
        }
    }
}
