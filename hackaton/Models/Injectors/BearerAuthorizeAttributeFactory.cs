using hackaton.Models.DAO;
using hackaton.Models.Security;
using hackaton.Models.Validations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hackaton.Models.Injectors
{
    public class BearerAuthorizeAttributeFactory: IFilterFactory
    {
        private readonly Context _context;

        public BearerAuthorizeAttributeFactory(Context context)
        {
            _context = context;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new BearerAuthorizeAttribute(_context);
        }

        public bool IsReusable => false;
    }
}
