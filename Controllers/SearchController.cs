using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MainTravelClass;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace TravelNotification.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [Authorize]
        public ActionResult AdminPage()
        {
            StaffADProfile staffADProfile = new StaffADProfile();
            CurrentUser currentuser = new CurrentUser();
            staffADProfile.user_logon_name = User.Identity.Name;

            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            currentuser.UserNo = staffADProfile.employee_number;
            bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
            ViewData["checkApproverUser"] = checkApproverUser;
            return View();
        }

        [Authorize]
        public ActionResult SearchSubmit(SearchModel Search)
        {
            StaffADProfile staffADProfile = new StaffADProfile();
            CurrentUser currentuser = new CurrentUser();
            staffADProfile.user_logon_name = User.Identity.Name;

            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            currentuser.UserNo = staffADProfile.employee_number;
            bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
            ViewData["checkApproverUser"] = checkApproverUser;
            if (Search.branchName != null)
            {
                string[] BranchArray = Search.branchName.Split(':');
                Search.branchName = BranchArray[0];
                Search.BranchCode = int.Parse(BranchArray[1]);
            }
          

            if (Search.Dept != null)
            {
                string[] DeptArray = Search.Dept.Split(':');
                Search.Dept = DeptArray[0];
                Search.Dept_id = int.Parse(DeptArray[1]);
            }
            else
            {
                Search.Dept_id = null;
            }
            if (Search.DomicileBranch != null)
            {
                string[] DomicileBranchArray = Search.DomicileBranch.Split(':');
                Search.DomicileBranch = DomicileBranchArray[0];
                Search.DomicileBranchCode = int.Parse(DomicileBranchArray[1]);
            }
            else
            {
                Search.DomicileBranchCode = null;
            }

            if (Search.ExportToExport != true)
            {
                Search.Requests = new SearchAppClass().SearchTravelRequest(Search);              
            }
            else
            {
                List<ExcelResult> excelresult = new List<ExcelResult>();
                excelresult = new SearchAppClass().SearchTravelRequestExcel(Search);            

                GridView gv = new GridView();
                gv.DataSource = excelresult;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=TravelNotification_Excel.xls");
                Response.AddHeader("Pragma", "public");
                Response.AddHeader("Cache-Control", "max-age=0");
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                hw.AddAttribute("xmlns:x", "urn:schemas-microsoft-com:office:excel");
                hw.RenderBeginTag(HtmlTextWriterTag.Html);
                hw.RenderBeginTag(HtmlTextWriterTag.Head);
                hw.RenderBeginTag(HtmlTextWriterTag.Style);
                //hw.Write("br {mso-data-placement:same-cell;}");
                //hw.RenderEndTag() ;
                //hw.RenderEndTag();
                hw.RenderBeginTag(HtmlTextWriterTag.Body);                
                gv.RenderControl(hw);
                //hw.RenderEndTag();
                //hw.RenderEndTag();
                Response.Write(HttpUtility.HtmlDecode(sw.ToString()));
                Response.Flush();
                Response.End();
                return RedirectToAction("AdminPage");
            }
            //Search.Requests = new SearchAppClass().SearchTravelRequest(Search);
            return View("AdminPage", Search);
        }
    }
}