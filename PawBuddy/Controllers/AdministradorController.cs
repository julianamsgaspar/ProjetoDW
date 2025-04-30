using Microsoft.AspNetCore.Mvc;
using PawBuddy.Data;

namespace PawBuddy.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        
        
        public AdministradorController(ApplicationDbContext context)
        {
            _context = context;
            
        }
        

        // GET: AdministradorController
        public ActionResult Index()
        {
            return View();
        }
        
        
        
        
        
        
        
        
        
        
        

    }
}
