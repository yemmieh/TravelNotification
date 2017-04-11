using MainTravelClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;


namespace TravelNotification.Controllers
{
    public class TravelController : Controller
    {
        string errorResult = null;
        // GET: Travel
        [Authorize]
        public ActionResult NewRequest()
        {
            StaffADProfile staffADProfile = new StaffADProfile();
           // staffADProfile.user_logon_name = Environment.UserName;
            staffADProfile.user_logon_name = User.Identity.Name;

            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            //AD
            ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
            staffADProfile = activeDirectoryQuery.GetStaffProfile();
            TravelRequest newrequest = new TravelRequest();
            Profile profile = new Profile();
            Country countries = new Country();
            Account acc = new Account();
            acc.description = "no card";
            
            countries.countryList = new AppClass().getCountry();

            profile.StaffNo = staffADProfile.employee_number;
            bool checkApproverUser = new AppClass().ValidateCheckApproverUser(profile.StaffNo);
            ViewData["checkApproverUser"] = checkApproverUser; 

           // bool checkAdmin = new AppClass().ValidateAdminUser(profile.StaffNo);
         

            profile = new AppClass().getProfile(profile.StaffNo);
            profile.Email = staffADProfile.email;
            newrequest.StaffProfile = profile;
            newrequest.id = "TRA" + profile.StaffNo + id;
            newrequest.country = countries;
            newrequest.Accountdetails = acc;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            //ViewBag.coInit = "<script type='text/javascript'>alert('"+ TempData["ErrorMessage"] +"');</script>";
            return View(newrequest);
        }

        [Authorize]     
        public ActionResult getActDetails(string AccNo)
        {
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
           //Account newAccount = new Account();
           //details.Add(newAccount);
            
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
        public ActionResult AddRequest(TravelRequest newTravelRequest, Postedcards postedcards, Card card)
        {
            try
            {
                CardViewModel newCardList = new CardViewModel();

                newCardList = new AppClass().GetCardsModel(newTravelRequest.Accountdetails.AccountNo, postedcards);
                newTravelRequest.Action = "New Request";

                newTravelRequest.ApprovalStage = "Card Services Approval";

                var InsertTravelRequest = new AppClass().postDBRequest(newTravelRequest, newCardList, "TravelConnection");

                string[] result = InsertTravelRequest.ToString().Split('|');

                if (result[0] != "0")
                {
                    if (result[0] == "2627")
                    {
                        return Content("Unable to Submit \n Duplicate record");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result[1];
                        TempData["TravelRequest"] = newTravelRequest;
                        return RedirectToAction("NewRequest");
                    }
                }
                else
                {                                        
                    List<ApproverInfo> approversnames = new List<ApproverInfo>();
                    List<ApproverInfo> approvers = new List<ApproverInfo>();
                    approvers = new AppClass().getApproverInfo();
                    var approverNames = new AppClass().getApprovernames();
                    foreach (var approver in approvers)
                    {                  
                    SendEmail newmail = new SendEmail();
                    newmail.sendEmail(newTravelRequest.Accountdetails.Account_Name,
                                        newTravelRequest.Accountdetails.AccountNo,
                                        newTravelRequest.StaffProfile.StaffNo,
                                        newTravelRequest.StaffProfile.StaffName,
                                        newTravelRequest.StaffProfile.jobtitle,
                                        newTravelRequest.Action,
                                        newTravelRequest.Comment,
                                        newTravelRequest.ApprovalStage,
                                        newTravelRequest.Date_Submitted.ToString(),
                                        approver.Email,
                                        newTravelRequest.StaffProfile.StaffNo,
                                        newTravelRequest.StaffProfile.StaffName);
                    }
                    TempData["Key"] = "Insert"; 
                    TempData["ErrorMessage"] = "You have successfully submitted your request for possible approval by:";
                    TempData["Approvernames"] = string.Join("\\n", approverNames);
                    TempData["TravelRequest"] = newTravelRequest;
                    return RedirectToAction("AwaitingApproval");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Update(TravelCards updatetravel)
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();

                // staffADProfile.user_logon_name = User.Identity.Name;
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                currentuser.UserName = new AppClass().getProfile(currentuser.UserNo).StaffName;
                currentuser.logonName = staffADProfile.user_logon_name;
                currentuser.Email = staffADProfile.email;
                bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);

                if (!checkApprover)
                {
                    TempData["ErrorMessage"] = "You are not authorize to Perform these operation";
                    TempData["TravelRequest"] = updatetravel;
                    return RedirectToAction("ExplodeTravelrequest");
                }
                else
                {
                    var MyTravelApproval = new AppClass().updateDBRequest(updatetravel, currentuser, "TravelConnection");
                    string[] result = MyTravelApproval.ToString().Split('|');

                    if (result[0] != "0")
                    {
                        if (result[0] == "2627")
                        {
                            // return Content("Unable to Submit \n Duplicate record");
                            TempData["ErrorMessage"] = "Unable to Submit \n Duplicate record";
                            TempData["TravelRequest"] = updatetravel;
                            return RedirectToAction("ExplodeTravelrequest");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = result[1];
                            TempData["TravelRequest"] = updatetravel;
                            return RedirectToAction("ExplodeTravelrequest");
                            // return Content(result[1]);
                        }

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Your Request as been Submitted Successfully ";
                        TempData["TravelRequest"] = updatetravel;

                        SendEmail newmail = new SendEmail();
                        newmail.sendEmail(updatetravel.travel_request.Accountdetails.Account_Name,
                                              updatetravel.travel_request.Accountdetails.AccountNo,
                                              updatetravel.travel_request.StaffProfile.StaffNo,
                                              updatetravel.travel_request.StaffProfile.StaffName,
                                              updatetravel.travel_request.StaffProfile.jobtitle,
                                              updatetravel.travel_request.Action,
                                              updatetravel.travel_request.Comment,
                                            updatetravel.travel_request.ApprovalStage,
                                            updatetravel.travel_request.Date_Submitted.ToString(),
                                            updatetravel.travel_request.StaffProfile.Email,
                                            updatetravel.travel_request.StaffProfile.StaffNo,
                                            updatetravel.travel_request.StaffProfile.StaffName);
                        //return RedirectToAction("ExplodeTravelrequest");
                        if (updatetravel.travel_request.Action != "Deny")
                        {
                            TempData["ErrorMessage"] = "You have Successfully Approved the request";
                           
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "You have Successfully Denied the request";
                        }

                        TempData["Key"] = "Approval";
                        return RedirectToAction("AwaitingApproval");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }




        [Authorize]
        public ActionResult MyTravelRequests()
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
               
                staffADProfile.user_logon_name = User.Identity.Name;  
           
                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(staffADProfile.employee_number);
                ViewData["checkApproverUser"] = checkApproverUser; 
                var requestList = new AppClass().GetMyTravelRequest(staffADProfile);

                return View(requestList);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }


        [Authorize]
        public ActionResult AwaitingApproval()
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();

               // staffADProfile.user_logon_name = User.Identity.Name;
                CurrentUser currentuser = new CurrentUser();
                staffADProfile.user_logon_name = User.Identity.Name;

                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);

                staffADProfile          = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo      = staffADProfile.employee_number;
                currentuser.UserName    = staffADProfile.in_StaffName;
                currentuser.logonName   = staffADProfile.user_logon_name;
                currentuser.Email       = staffADProfile.email;
                List<TravelRequest> requestList = new List<TravelRequest>();
                bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);
                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser; 
               // bool checkAdmin = new AppClass().ValidateAdminUser(staffADProfile.employee_number);
                
                string Key = TempData["key"] as string;
                
                if (Key != "Approval")
                {
                    ViewBag.coInit = (String.IsNullOrEmpty(TempData["ErrorMessage"] as string)) ?
                       "" : "<script type='text/javascript'>alert(\"" + TempData["ErrorMessage"] + "\\n\\n" + TempData["Approvernames"] + "\");</script>";

                }
                else
                {
                    ViewBag.coInit = (String.IsNullOrEmpty(TempData["ErrorMessage"] as string)) ?
                   "" : "<script type='text/javascript'>alert('" + TempData["ErrorMessage"] + "');</script>";

                }

                //ViewBag.coInit = (String.IsNullOrEmpty(TempData["ErrorMessage"] as string)) ?
                //    "" : "<script type='text/javascript'>alert(\"" + TempData["ErrorMessage"] + "\\n\\n" + TempData["Approvernames"] + "\");</script>";

               // ViewBag.PostBackMessage = "<script type='text/javascript'>alert(\"" + PostBackMessage + "\\n\\n" + Approvers + "\");</script>";

                
                if (checkApprover != false)
                {
                     requestList = new AppClass().AwaitingApprovals();

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
                // staffADProfile.user_logon_name = User.Identity.Name;
                CurrentUser currentuser = new CurrentUser();
                currentuser.logonName = User.Identity.Name;
                staffADProfile.user_logon_name = User.Identity.Name;
                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(staffADProfile.employee_number);
                ViewData["checkApproverUser"] = checkApproverUser; 
                List<TravelRequest> requestList = new List<TravelRequest>();
                requestList = new AppClass().ApprovedApprovals(staffADProfile.employee_number);
                return View(requestList);
            }
            catch(Exception ex)
            {
                return View(ex.Message);
            }
        }
        [Authorize]
        public ActionResult ExplodeTravelrequest(string id)
        {
            try
            {
                StaffADProfile staffADProfile = new StaffADProfile();
                // staffADProfile.user_logon_name = User.Identity.Name;
                CurrentUser currentuser = new CurrentUser();
                currentuser.logonName = User.Identity.Name;
                staffADProfile.user_logon_name = User.Identity.Name;
                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                currentuser.UserName = staffADProfile.in_StaffName;
                currentuser.logonName = staffADProfile.user_logon_name;
                currentuser.Email = staffADProfile.email;

                bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);
                currentuser.IsApprover = checkApprover;
                // check for AdminUsers
                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(currentuser.UserNo);
                ViewData["checkApproverUser"] = checkApproverUser; 

                TravelCards request = new TravelCards();
              //  TravelCards AllInput = new TravelCards();
  
                request = new AppClass().ExplodeRequest(id).First();
                if (checkApprover == false) 
                {
                    request.travel_request.Status = "Approved";
                }
                @ViewBag.requestStage = request.travel_request.ApprovalStage;
                return View(request);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //[AcceptParameter(Name = "Checkout")]
        //public ActionResult MultipleApproval(string[] selectedRows, string button)
        //{
        //    return View();
        //}
        // [HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        //[AcceptParameter(Name = "Delete")]
        //public ActionResult MultipleDenial(string[] selectedRows, string UserID, string partnerid, string productid)
        //{
        //    return View();
        //}


        public ActionResult MultipleApproval(FormCollection form)
        {
            try
            {

                StaffADProfile staffADProfile = new StaffADProfile();
                // staffADProfile.user_logon_name = User.Identity.Name;
                CurrentUser currentuser = new CurrentUser();
                currentuser.logonName = User.Identity.Name;
                
                staffADProfile.user_logon_name = User.Identity.Name;
                ActiveDirectoryQuery activeDirectoryQuery = new ActiveDirectoryQuery(staffADProfile);
                staffADProfile = activeDirectoryQuery.GetStaffProfile();
                currentuser.UserNo = staffADProfile.employee_number;
                currentuser.UserName = new AppClass().getProfile(currentuser.UserNo).StaffName;
                bool checkApproverUser = new AppClass().ValidateCheckApproverUser(staffADProfile.employee_number);
                ViewData["checkApproverUser"] = checkApproverUser; 
                var ch = form.GetValues("assignChkBx");
                var comment = form.GetValue("Comment").AttemptedValue;
                var Action = form.GetValue("Action").AttemptedValue;
                bool checkApprover = new AppClass().ValidateCurrentUser(currentuser);
                currentuser.IsApprover = checkApprover;
                if (checkApprover == false)
                {

                }
                else
                {

                    foreach (var id in ch)
                    {
                        TravelCards updatetravel = new TravelCards();
                        updatetravel = new AppClass().ExplodeRequest(id).First();
                        updatetravel.travel_request.Action = Action.ToString();
                        updatetravel.travel_request.Comment = comment.ToString();
                        var MyTravelApproval = new AppClass().updateDBRequest(updatetravel, currentuser, "TravelConnection");
                        string[] result = MyTravelApproval.ToString().Split('|');

                        if (result[0] != "0")
                        {
                            if (result[0] == "2627")
                            {
                                // return Content("Unable to Submit \n Duplicate record");
                                TempData["ErrorMessage"] = "Unable to Submit \n Duplicate record";
                                TempData["TravelRequest"] = updatetravel;
                               // return RedirectToAction("AwaitingApproval");
                            }
                            else
                            {
                                TempData["ErrorMessage"] = result[1];
                                TempData["TravelRequest"] = updatetravel;
                               // return RedirectToAction("AwaitingApproval");
                                // return Content(result[1]);
                            }

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Your Request as been Submitted Successfully ";
                            TempData["TravelRequest"] = updatetravel;

                            SendEmail newmail = new SendEmail();
                            newmail.sendEmail(updatetravel.travel_request.Accountdetails.Account_Name,
                                                updatetravel.travel_request.Accountdetails.AccountNo,
                                                updatetravel.travel_request.StaffProfile.StaffNo,
                                                updatetravel.travel_request.StaffProfile.StaffName,
                                                updatetravel.travel_request.StaffProfile.jobtitle,
                                                updatetravel.travel_request.Action,
                                                updatetravel.travel_request.Comment,
                                                updatetravel.travel_request.ApprovalStage,
                                                updatetravel.travel_request.Date_Submitted.ToString(),
                                                updatetravel.travel_request.StaffProfile.Email,
                                                updatetravel.travel_request.StaffProfile.StaffNo,
                                                updatetravel.travel_request.StaffProfile.StaffName);
                            //return RedirectToAction("ExplodeTravelrequest");
                            if (updatetravel.travel_request.Action != "Deny")
                            {
                                TempData["ErrorMessage"] = "You have Successfully Approved the request";
                                
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "You have Successfully Denied the request";
                            }

                            TempData["Key"] = "Approval";
                            //return RedirectToAction("AwaitingApproval");
                        }

                    }
                    //return View();
                    //TempData["Key"] = "Approval";
                    //return RedirectToAction("AwaitingApproval");


                }
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData["Key"] = "Approval";
                return RedirectToAction("AwaitingApproval");
            }

            return RedirectToAction("AwaitingApproval");
            
        }


        [Authorize]
        public ActionResult GetApprovers(string WorkflowID)
        {
             TravelCards request = new TravelCards();           
             request = new AppClass().ExplodeRequest(WorkflowID).First();
             string errorResult = "{{\"employee_number\":\"{0}\",\"name\":\"{1}\"}}";
             if (request.travel_request.ApprovalStage != "Card Services Approval")
             {                 
                errorResult = string.Format(errorResult, null, "No approvers found for the entry");
                return Content(errorResult, "application/json");
             }
             else
             { 
            var profile = new AppClass().getApprovernames();
            if (profile == null || profile.Count() <= 0)
            {
                errorResult = string.Format(errorResult, null, "No approvers found for the entry");
                return Content(errorResult, "application/json");
            }
            else
            {
                return Json(profile, JsonRequestBehavior.AllowGet);
            }
           }
        }
       
   
    }
}