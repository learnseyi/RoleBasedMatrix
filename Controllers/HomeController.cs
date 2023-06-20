using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoleBasedMatrix.Database;
using RoleBasedMatrix.Models;
using System.Diagnostics;


namespace RoleBasedMatrix.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _configuration;


        /// <summary>
        /// Initializes the HomeController with the global configuration settings
        /// and sets the initial values for the diviions select option
        /// </summary>
        /// <param name="configuration"></param>
        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult Index()
        {

            RBMDBContext dbContext = new(_configuration);
            dbContext.ConnectToDataBase();
            ViewBag.Divisions = new SelectList(dbContext.GetDivisions(), "DivisionId", "DivisionName");
            dbContext?.con?.Close();

            return View();
        }

        /// <summary>
        /// Gets department lists by division and roles by departments
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("home/getResult/{name}/{id:int}")]
        public JsonResult GetResult(string name, int id)
        {
            RBMDBContext dbContext = new(_configuration);
            dbContext.ConnectToDataBase();
            return name switch
            {
                "DivisionName" => Json(dbContext.GetDepartmentList(name, id)),//ViewBag.Roles = new SelectList(dbContext.GetDepartmentList(name, id), "DepartmentId", "DepartmentName", "DepartmentDescription");
                                                                              //dbContext?.con?.Close();
                                                                              //break;
                "DepartmentName" => Json(dbContext.GetRoles(id)),//ViewBag.Roles = new SelectList((IEnumerable<RolesModel>)dbContext.GetRoles(id), "Roleid", "DepartmentId", "RoleName");
                                                                 //dbContext?.con?.Close();
                                                                 //break;
                _ => Json("No result to be displayed"),//ViewBag.message = new SelectList("");
                                                       //break;
            };
        }

        /// <summary>
        /// Gets applications and permissions search results
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("home/getAssignments/{id:int?}")]
        public string GetAssignments(int id)
        {
            RBMDBContext dbContext = new(_configuration);
            string response = dbContext.GetAssignedAppsAndPermission(id);
            dbContext?.con?.Close();
            return response;
        }

        /// <summary>
        /// Returns application permissions
        /// </summary>
        /// <returns></returns>

        [Route("home/editpermissions/{role?}/{app?}")]
        public IActionResult EditPermissions(string role, string app)
        {
            //EditAssignmentModel model = new();
            RBMDBContext dbContext = new(_configuration);
            ViewData["Role"] = role;
            ViewData["app"] = app;
            //ViewData["Permissions"] = new SelectList(dbContext.GetAppAuthorizations(app), "AuthId", "AuthLabel");
            ViewData["currentPermissions"] = dbContext.GetCurrentPermissions(app, role);
            ViewData["Permissions"] = dbContext.GetAppAuthorizations(app);
            //TempData["Permissions"] = dbContext.GetAppAuthorizations(app);
            return View("Modal");
        }



    }
}