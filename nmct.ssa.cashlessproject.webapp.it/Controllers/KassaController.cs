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
    public class KassaController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            List<KassaIT> kassas = KassaDA.GetKassas();

            return View(kassas);
        }

        [HttpGet]
        public ActionResult New()
        {
            List<Organisatie> organisaties = OrganisatieDA.GetOrganisaties();

            Organisatie geenOrg = new Organisatie() { ID = 0, OrganisatieNaam = "geen organisatie" };
            organisaties.Insert(0, geenOrg);
            ViewBag.Organisaties = new SelectList(organisaties, "ID", "OrganisatieNaam");

            return PartialView("_New");
        }

        [HttpPost]
        public ActionResult New(KassaIT kassa)
        {
            kassa.ID = KassaDA.AddKassa(kassa);
            if (kassa.SelectedOrganisatieID != 0) OrganisatieKassaDA.AddOrganisatieKassa(kassa);
           
            return RedirectToAction("Index");

            //if (ModelState.IsValid) return RedirectToAction("Index");
            //else return PartialView("_New", organisatie);
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            KassaIT kassa = KassaDA.GetKassaById(id.Value);

            List<Organisatie> organisaties = OrganisatieDA.GetOrganisaties();
            List<OrganisatieKassa> organisatiekassas = OrganisatieKassaDA.GetOrganisatieKassas();

            Organisatie geenOrg = new Organisatie() { ID = 0, OrganisatieNaam = "geen organisatie" };
            organisaties.Insert(0, geenOrg);

            OrganisatieKassa orgkassa = organisatiekassas.FirstOrDefault(ok => ok.KassaIT.ID == id.Value);
            if (orgkassa != null) kassa.SelectedOrganisatieID = orgkassa.Organisatie.ID;
            else kassa.SelectedOrganisatieID = geenOrg.ID;
            
            ViewBag.Organisaties = new SelectList(organisaties, "ID", "OrganisatieNaam", kassa.SelectedOrganisatieID);

            return PartialView("_Edit", kassa);
        }

        [HttpPost]
        public ActionResult Edit(KassaIT kassa)
        {
            KassaDA.UpdateKassa(kassa);

            List<OrganisatieKassa> organisatiekassas = OrganisatieKassaDA.GetOrganisatieKassas();
            OrganisatieKassa selectedorganisatiekassa = organisatiekassas.FirstOrDefault(ok => ok.KassaIT.ID == kassa.ID);

            if (selectedorganisatiekassa != null)
            {
                if (kassa.SelectedOrganisatieID == 0) OrganisatieKassaDA.DeleteOrganisatieKassa(kassa);
                else OrganisatieKassaDA.UpdateOrganisatieKassa(kassa);
            }
            else OrganisatieKassaDA.AddOrganisatieKassa(kassa);
            
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            KassaIT kassa = KassaDA.GetKassaById(id.Value);

            List<OrganisatieKassa> organisatiekassas = OrganisatieKassaDA.GetOrganisatieKassas();  
            OrganisatieKassa orgkassa = organisatiekassas.FirstOrDefault(ok => ok.KassaIT.ID == kassa.ID);

            if (orgkassa != null) ViewBag.SelectedOrganisatie = orgkassa.Organisatie.OrganisatieNaam;
            else ViewBag.SelectedOrganisatie = "geen organisatie";
           
            return PartialView("_Details", kassa);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            KassaIT kassa = KassaDA.GetKassaById(id.Value);
            ViewBag.KassaNaam = kassa.KassaNaam;

            List<OrganisatieKassa> organisatiekassas = OrganisatieKassaDA.GetOrganisatieKassas();
            OrganisatieKassa orgkassa = organisatiekassas.FirstOrDefault(ok => ok.KassaIT.ID == kassa.ID);

            if (orgkassa != null) ViewBag.SelectedOrganisatie = orgkassa.Organisatie.OrganisatieNaam;
            else ViewBag.SelectedOrganisatie = "geen organisatie";

            return PartialView("_Delete", kassa);
        }


        [HttpPost]
        public ActionResult Delete(KassaIT kassa)
        {
            OrganisatieKassaDA.DeleteOrganisatieKassa(kassa);
            KassaDA.DeleteKassa(kassa);

            return RedirectToAction("Index");
        }
    }
}
