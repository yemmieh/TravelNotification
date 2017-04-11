using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MainTravelClass;

namespace TravelNotification.Controllers
{
    public class HomeController : Controller
    {
         [Authorize]
        public ActionResult Index()
        {
            StaffADProfile staffADProfile = new StaffADProfile();
            staffADProfile.user_logon_name = User.Identity.Name;

            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            ViewBag.StaffNumber = staffADProfile.employee_number;
            bool checkApproverUser = new AppClass().ValidateCheckApproverUser(staffADProfile.employee_number);
            ViewData["checkApproverUser"] = checkApproverUser;         
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}