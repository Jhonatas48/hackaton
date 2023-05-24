using hackaton.Models.DAO;
using hackaton.Models.Validations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace hackaton.Models.Injectors
{
    public class RequireLoginAttributeFactory : IFilterFactory
    {
        private readonly Context _context;

        public RequireLoginAttributeFactory(Context context)
        {
            _context = context;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new RequireLoginAttribute(_context);
        }

        public bool IsReusable => false;
    }

}
