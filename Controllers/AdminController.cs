using MainTravelClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TravelNotification.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        [Authorize]
        public ActionResult DeniedRequest()
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();            
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                currentuser.UserName = staffADProfile.in_StaffName;
                currentuser.logonName = staffADProfile.user_logon_name;
                currentuser.Email = staffADProfile.email;
                List<TravelRequest> requestList = new List<TravelRequest>();
               // bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);


                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;


                if (checkApproverUser != false)
                {
                    requestList = new AdminClass().GetDeniedRequest();

                    return View(requestList);
                }
                else
                {

                    return View(requestList);
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
           [Authorize]
        public ActionResult ApprovedRequest()
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                currentuser.UserName = staffADProfile.in_StaffName;
                currentuser.logonName = staffADProfile.user_logon_name;
                currentuser.Email = staffADProfile.email;
                List<TravelRequest> requestList = new List<TravelRequest>();
              //  bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);


                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;


                if (checkApproverUser != false)
                {
                    requestList = new AdminClass().GetApprovedRequest();

                    return View(requestList);
                }
                else
                {

                    return View(requestList);
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
           [Authorize]
        public ActionResult ApproverList()
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                currentuser.UserName = staffADProfile.in_StaffName;
                currentuser.logonName = staffADProfile.user_logon_name;
                currentuser.Email = staffADProfile.email;
                List<ApproverInfo> approverList = new List<ApproverInfo>();
               // bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);

                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;

                if (checkApproverUser != false)
                {
                    approverList = new AdminClass().GetApproverList();
                    ViewBag.coInit = (String.IsNullOrEmpty(TempData["ErrorMessage"] as string)) ?
               "" : "<script type='text/javascript'>alert('" + TempData["ErrorMessage"] + "');</script>";
                    return View(approverList);
                }
                else
                {
                    ViewBag.coInit = (String.IsNullOrEmpty(TempData["ErrorMessage"] as string)) ?
               "" : "<script type='text/javascript'>alert('" + TempData["ErrorMessage"] + "');</script>";
                    return View(approverList);
                }
            }
            catch (Exception ex)
            {
                ViewBag.coInit = (String.IsNullOrEmpty(TempData["ErrorMessage"] as string)) ?
               "" : "<script type='text/javascript'>alert('" + TempData["ErrorMessage"] + "');</script>";
                return View(ex.Message);
            }

           
            return View();
        }
           [Authorize]
        public ActionResult Manage_Approval_List()
        {

            StaffADProfile staffADProfile = new StaffADProfile();
            CurrentUser currentuser = new CurrentUser();
            staffADProfile.user_logon_name = User.Identity.Name;

            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            currentuser.UserNo = staffADProfile.employee_number;
            currentuser.UserName = staffADProfile.in_StaffName;
            currentuser.logonName = staffADProfile.user_logon_name;
            currentuser.Email = staffADProfile.email;
    
            bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
            ViewData["checkApproverUser"] = checkApproverUser;

            if (!checkApproverUser)
            {
                TempData["ErrorMessage"] = "";
                return RedirectToAction("AwaitingApproval");
            }
            else
            {                
                ApproverInfo approvers = new ApproverInfo();
                //approvers = new AdminClass().GetApproverList();
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                return View(approvers);
            }
            //List<ApproverInfo> approvers = new List<ApproverInfo>();
            
        }

           [Authorize]
        public ActionResult getProfileDetails(string StaffNumber)
        {
            string errorResult = "{{\"employee_number\":\"{0}\",\"name\":\"{1}\"}}";
                ApproverInfo approver = new ApproverInfo();
                approver.StaffNumber = StaffNumber;
                approver = new AppClass().getApproverProfile(StaffNumber);

            if (approver.name == null)
            {
                errorResult = string.Format(errorResult, "Error", "No records found for the staff number");
                return Content(errorResult, "application/json");
            }
            else
            {
                return Json(approver, JsonRequestBehavior.AllowGet);
            }
        }


           [Authorize]
           public ActionResult getActDetails(string AccNo)
           {
               StaffADProfile staffADProfile = new StaffADProfile();
               CurrentUser currentuser = new CurrentUser();
               staffADProfile.user_logon_name = User.Identity.Name;

               ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

               staffADProfile = activeDirectoryQuery.GetStaffProfile();
               currentuser.UserNo = staffADProfile.employee_number;
                ApproverInfo approver = new ApproverInfo();
               // approver.StaffNumber = StaffNumber;
                approver = new AppClass().getApproverProfile(currentuser.UserNo);
               string errorResult = null;
               List<Account> details = new List<Account>();
               for (int i = 1; i < 3; i++)
               {
                   var Accdetails = new AppClass().getAllCards(i, AccNo);
                   foreach (Account newAccdetails in Accdetails)
                   {
                       Account Acc = new Account();
                       Acc.Account_Name = newAccdetails.Account_Name;
                       Acc.description = newAccdetails.description;
                       Acc.AccountNo = newAccdetails.AccountNo;
                       Acc.DomicileBranch = newAccdetails.DomicileBranch;
                       Acc.DomicileBranchCode = newAccdetails.DomicileBranchCode;
                       details.Add(Acc);
                   }

               }

               if (details == null)
               {
                   errorResult = string.Format(errorResult, "Error", "No records found for the Account number");
                   return Content(errorResult, "application/json");
               }
               else
               {
                   return Json(details, JsonRequestBehavior.AllowGet);
               }
           }

        [Authorize]
        public ActionResult InsertApprover(ApproverInfo approver)
        {
            try{
                StaffADProfile staffADProfile = new StaffADProfile();
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;
                bool checkAdmin = new AppClass().ValidateAdminUser(currentuser.UserNo);
               // ViewData["checkAdmin"] = checkAdmin;

                if (!checkAdmin)
                {
                    TempData["ErrorMessage"] = "You are not authorized to Perform these operation";
                    TempData["TravelRequest"] = approver;
                    return RedirectToAction("Manage_Approval_List");
                }
                else
                {

                    var insert = approver.name = new AdminClass().AddApprover(approver);
                    string[] result = insert.ToString().Split('|');

                    if (result[0] != "0")
                    {
                        if (result[0] == "2627")
                        {
                            TempData["ErrorMessage"] = "User Already Existed on Approver list";
                            // TempData["TravelRequest"] = approver;
                            return RedirectToAction("Manage_Approval_List");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = result[1];
                            TempData["TravelRequest"] = approver;
                            return RedirectToAction("Manage_Approval_List");
                        }

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "You have successfully added a new Approver";
                        //  TempData["Approvernames"] = string.Join("\\n", approverNames);             
                        return RedirectToAction("ApproverList");
                    }                                       

                }
                
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


           [Authorize]
        public ActionResult Edit_Approver(string StaffNumber)
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;

                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;
               
                ApproverInfo approvers = new ApproverInfo();
                approvers = new AdminClass().GetApproverDetails(StaffNumber).First();
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
                return View(approvers);
            }catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Edit_Approver");
            }
            
        }

           [Authorize]
        public ActionResult UpdateApprover(ApproverInfo approver)
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;

                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;

                bool checkAdmin = new AppClass().ValidateAdminUser(currentuser.UserNo);
                //ViewData["checkAdmin"] = checkAdmin;

                if (!checkAdmin)
                {
                    TempData["ErrorMessage"] = "You are not authorized to Perform these operation";
                    TempData["TravelRequest"] = approver;
                    return RedirectToAction("Manage_Approval_List");
                }
                else
                {
                    var Update = new AdminClass().UpdateApprover(approver);
                    string[] result = Update.ToString().Split('|');

                    if (result[0] != "0")
                    {
                        if (result[0] == "2627")
                        {
                            TempData["ErrorMessage"] = "User Already Existed on Approver list";
                            // TempData["TravelRequest"] = approver;
                            return RedirectToAction("EditApprover");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = result[1];
                            //TempData["TravelRequest"] = approver;
                            return RedirectToAction("EditApprover");
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "You have successfully Updated ApproverName";
                        //  TempData["Approvernames"] = string.Join("\\n", approverNames);             
                        return RedirectToAction("ApproverList");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
           [Authorize]
        public ActionResult DeleteApprover(string Approver_ID)
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                bool checkAdmin = new AppClass().ValidateAdminUser(currentuser.UserNo);
                //ViewData["checkAdmin"] = checkAdmin;

                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser;

                if (!checkAdmin)
                {
                    TempData["ErrorMessage"] = "You are not authorized to Perform these operation";
                    //TempData["TravelRequest"] = approver;
                    return RedirectToAction("Manage_Approval_List");
                }
                else
                {

                    var delete = new AdminClass().DeleteApprover(Approver_ID);
                    string[] result = delete.ToString().Split('|');

                    if (result[0] != "0")
                    {
                        if (result[0] == "2627")
                        {
                            TempData["ErrorMessage"] = result[1];
                            // TempData["TravelRequest"] = approver;
                            return RedirectToAction("ApproverList");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = result[1];
                            // TempData["TravelRequest"] = approver;
                            return RedirectToAction("ApproverList");
                        }

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "You have successfully Updated ApproverName";
                        //  TempData["Approvernames"] = string.Join("\\n", approverNames);             
                        return RedirectToAction("ApproverList");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}