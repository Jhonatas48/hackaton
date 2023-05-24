using hackaton.Models.DAO;
using hackaton.Models.Validations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hackaton.Models.Injectors
{
    public class RequireLoginAdminAttributeFactory : IFilterFactory
    {
        private readonly Context _context;

        public RequireLoginAdminAttributeFactory(Context context)
        {
            _context = context;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new RequireLoginAdminAttribute(_context);
        }

        public bool IsReusable => false;
    }

}
