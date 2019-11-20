using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tprofile.Classes.Payeezy;
using tprofile.Repositories;

namespace PayeezyTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AuthorizeSessionResponse data = new AuthorizeSessionResponse();

            //var message = new AuthorizeSession()
            //{
            //    Gateway = "PAYEEZY",
            //    ApiKey = "G26mPwHd0zkPLdRjZXwHzbvbZGARSYOv",
            //    ApiSecret = "69d81c12e3a5b5b6f08f06bf14478bf5fc124018e24833138e3d751217dff62c",
            //    AuthToken = "fdoa-58b315cee9dfdf98ab62efa24904a8a2aef8e02f5e0e259e",
            //    TransarmorToken = "NOIW",
            //    ZeroDollarAuth = false
            //};

            var message = new AuthorizeSession()
            {
                Gateway = "PAYEEZY",
                ApiKey = "aRRMPmVZ1YcYg1TYPDTShbF7MTHyINy1",
                ApiSecret = "fcaccf88badd97b8e436035f074c5a1139dfb0528722dad7ad7c651fc5fb852",
                AuthToken = "fdoa-b5074351a146da5885d6648325b09e16b5074351a146da56",
                TransarmorToken = "NOIW",
                ZeroDollarAuth = true
            };



            var payeezyV2Repo = new PayeezyRepositoryV2();
            var payeezyResponse = payeezyV2Repo.AuthorizeSession(message);
            if (payeezyResponse != null)
            {
                data = payeezyResponse;
            }
            
            return View(data);
        }

      
    }
}