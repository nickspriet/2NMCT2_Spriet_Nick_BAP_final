using nmct.ba.cashlessproject.model.IT;
using nmct.ssa.cashlessproject.webapp.it.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nmct.ssa.cashlessproject.webapp.it.Controllers
{
    [Authorize]
    public class OrganisatieController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            List<Organisatie> organisaties = new List<Organisatie>();
            organisaties = OrganisatieDA.GetOrganisaties();

            return View("Index", organisaties);
        }


        [HttpGet]
        public ActionResult New()
        {
            return PartialView("_New");
        }

        [HttpPost]
        public ActionResult New(Organisatie organisatie)
        {
            OrganisatieDA.AddOrganisatie(organisatie);
            return RedirectToAction("Index");

            //if (ModelState.IsValid) return RedirectToAction("Index");
            //else return PartialView("_New", organisatie);
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            Organisatie organisatie = new Organisatie();
            organisatie = OrganisatieDA.GetOrganisatieById(id.Value);

            return PartialView("_Edit", organisatie);
        }

        [HttpPost]
        public ActionResult Edit(Organisatie organisatie)
        {
            OrganisatieDA.UpdateOrganisatie(organisatie);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            Organisatie organisatie = new Organisatie();
            organisatie = OrganisatieDA.GetOrganisatieById(id.Value);

            return PartialView("_Details", organisatie);
        }


        [HttpGet]
        public ActionResult BekijkKassas(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            List<OrganisatieKassa> organisatiekassas = new List<OrganisatieKassa>();
            organisatiekassas = OrganisatieKassaDA.GetOrganisatieKassas().Where(ok => ok.Organisatie.ID == id.Value).ToList();

            ViewBag.OrganisatieNaam = OrganisatieDA.GetOrganisatieById(id.Value).OrganisatieNaam;


            List<KassaIT> kassas = new List<KassaIT>();
            foreach(OrganisatieKassa ok in organisatiekassas)
            {
                kassas.Add(KassaDA.GetKassaById(ok.KassaIT.ID));
            }
            

            return PartialView("_BekijkKassas", kassas);
        }


        [HttpGet]
        public ActionResult DeleteOrganisatieKassa(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            KassaIT kassa = KassaDA.GetKassaById(id.Value);
            ViewBag.KassaNaam = kassa.KassaNaam;

            List<OrganisatieKassa> organisatiekassas = OrganisatieKassaDA.GetOrganisatieKassas();
            OrganisatieKassa orgkassa = organisatiekassas.FirstOrDefault(ok => ok.KassaIT.ID == kassa.ID);

            if (orgkassa != null) ViewBag.SelectedOrganisatie = orgkassa.Organisatie.OrganisatieNaam;
            else ViewBag.SelectedOrganisatie = "geen organisatie";

            return View("DeleteOrganisatieKassa", kassa);
        }


        [HttpPost]
        public ActionResult DeleteOrganisatieKassa(KassaIT kassa)
        {
            OrganisatieKassaDA.DeleteOrganisatieKassa(kassa);

            return RedirectToAction("Index");
        }
    }
}