using MainTravelClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace TravelNotification.Controllers
{
    public class PartialController : Controller
    {
        string errorResult = "";
        // GET: Partial
        public ActionResult getCards(string AccNo)
        {

            List<Card> details = new List<Card>();

         //   TravelRequest newrequest = new TravelRequest();


            for (int i = 1; i < 3; i++)
            {

                var Accdetails = new AppClass().getAllCards(i, AccNo);
                foreach (Account newAccdetails in Accdetails)
                {
                    Card Acc = new Card();
                  //  Acc.Account_Name = newAccdetails.Account_Name;
                    Acc.Id = newAccdetails.description;
                    Acc.Name = newAccdetails.description;
                    
                    details.Add(Acc);
                }
            }
          //  var model = new CardViewModel();
            var model = new CardViewModel();
            var selectedCards = new List<Card>();

            //setup a view model
            model.AvailableCards = details;
            model.SelectedCards = selectedCards;
           // model.descriptionList = details;

            //for (int b = 0; b < details.Count; b++)
            //{

            //    ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("Cards[{0}]", b);
            //}

            if (model == null)
            {
                errorResult = string.Format(errorResult, "Error", "No Card Found");
                return Content(errorResult, "application/json");
            }
            else
            {
                
                return PartialView("getCards", model);
            }
        }


       


        public PartialViewResult AddCountry(string CountryName, int index)
        {
            var newCountry = new Country() { CountryName = CountryName, Index = index};
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("Countries[{0}]", index);
            return PartialView("AddCountry", newCountry);
        }

        public ActionResult HistoryIndex(string id)
        {
           // List<ApprovalDetails> History = new List<ApprovalDetails>();
            //History = new AppClass().getHistory(id);


            XElement ApprovalHistory = new AppClass().getApprovalHistory(id);
            XDocument xDocument = DataHandlers.ToXDocument(ApprovalHistory);

            List<ApprovalDetails> approvalHistory = xDocument.Descendants("Approvals")
                .Select(det => new ApprovalDetails
                {
                    ApproverNames = det.Element("ApproverName").Value,
                    ApproverStaffNumbers = det.Element("ApproverStaffNumber").Value,
                    ApprovedStages = det.Element("ApprovedStage").Value,
                    ApproverAction = det.Element("ApproverAction").Value,
                    ApprovalDateTime = det.Element("ApprovalDateTime").Value,
                    ApproverComment = det.Element("ApproverComment").Value.Equals("") ? "None" : det.Element("ApproverComment").Value,
                })
                .ToList();


            return PartialView("HistoryIndex", approvalHistory);
        }


    }
}