using FoodieFinderPasswordRecoveryServer.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodieFinderPasswordRecoveryServer.Pages
{

    public class IndexModel : PageModel
    {
        public AppDbContext DbContext { get; }
        public IConfiguration Configuration { get; }

        public IndexModel(AppDbContext dbContext, IConfiguration configuration)
        {
            DbContext = dbContext;
            Configuration = configuration;
        }

        public void OnGet()
        {

        }
    }
}