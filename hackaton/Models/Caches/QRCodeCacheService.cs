using hackaton.Models.DAO;
using Microsoft.Extensions.Caching.Memory;

namespace hackaton.Models.Caches
{
    public class QRCodeCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly Context _context;

        public QRCodeCacheService(IMemoryCache cache, Context context)
        {
            _cache = cache;
            _context = context;

        }

        public void AddQRCodeToCache(QrCode qrCode)
        {
            _cache.Set(qrCode.Content, qrCode);
        }


        public QrCode GetQRCodeByContentAsync(string content)
        {
            // Verifica se o usuário está no cache
            if (_cache.TryGetValue(content, out QrCode qrCode))
            {
                return qrCode;
            }

            // Se o usuário não está no cache, busca no banco de dados
            qrCode = _context.QrCodes.FirstOrDefault(qr => qr.Content == content);

            // Adiciona o usuário ao cache por 5 minutos
            if (qrCode != null)
            {
                _cache.Set(qrCode.Content, qrCode, TimeSpan.FromMinutes(5));
            }

            return qrCode;
        }
    }
}

