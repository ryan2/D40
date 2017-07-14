using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using D40.Models;
using D40.ViewModels;
using Excel;
using System.IO;

namespace D40.Controllers
{
    public class D40Controller : Controller
    {
        private D40DBContext db = new D40DBContext();

        // GET: D40
        public ActionResult Index()
        {
            var viewModels = new NameIndexData();
            viewModels.Users = db.Names
                .Include(i => i.Assets)
                .Include(i => i.Software.Select(c => c.Software))
                .Include(i => i.Tickets);
            viewModels.Users = viewModels.Users.OrderBy(x => x.Last_Name);
            return View(viewModels);
        }
        public ActionResult Index_Old(string catList, string searchString)
        {
            ViewBag.search = searchString;
            var categoryList = new List<string>(4) { "Computer", "Phone", "Printer", "Phone Services" };
            ViewBag.catList = new SelectList(categoryList, catList);
            ViewBag.Show = false;
            var entries = from t in db.D40 select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                entries = entries.Where(s => s.First_Name.Contains(searchString) || s.Last_Name.Contains(searchString) || s.Asset_Tag.Contains(searchString) || (s.First_Name + " " + s.Last_Name).Contains(searchString));
            }
            if (!String.IsNullOrEmpty(catList))
            {
                entries = entries.Where(s => s.Category.Contains(catList));
            }
            return View(entries.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index_Old(HttpPostedFileBase upload)
        {
            ViewBag.Show = true;
            ViewBag.newEntry = new List<Models.D40>();
            ViewBag.modEntry = new Dictionary<Models.D40,Models.D40>();
            var categoryList = new List<string>(4) { "Computer", "Phone", "Printer", "Phone Services" };
            ViewBag.catList = new SelectList(categoryList);
            int diff = 0;

            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        ViewBag.Show = false;
                        return View(db.D40.ToList());
                    }

                    reader.IsFirstRowAsColumnNames = true;
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    List<D40.Models.D40> del = db.D40.Where(x=>x.Returned_Date==null).ToList();
                    ViewBag.Show = true;
                    for (int i = 0;i < dt.Rows.Count; i++){
                        Models.D40 entry = new Models.D40();
                        entry.Category = (string) dt.Rows[i][0];
                        entry.Record_ID = System.DBNull.Value.Equals(dt.Rows[i][1]) ? 0 : (int)((Double)dt.Rows[i][1]);
                        entry.Asset_Tag = (string)dt.Rows[i][2];
                        entry.Asset_status = (string)dt.Rows[i][3];
                        entry.Serial_Number = (string)dt.Rows[i][4];
                        entry.BB_Phone = System.DBNull.Value.Equals(dt.Rows[i][5]) ? null : (string)dt.Rows[i][5];
                        bool two = System.DBNull.Value.Equals(dt.Rows[i][6]);
                        entry.Refresh_Date = two ? new DateTime(2000,1,1) : (DateTime)dt.Rows[i][6];
                        entry.Model = System.DBNull.Value.Equals(dt.Rows[i][7]) ? null : (string)dt.Rows[i][7];
                        entry.Seat_Type = (string)dt.Rows[i][8];
                        entry.Service_Level = (string)dt.Rows[i][9];
                        entry.HHS_Billing = (string)dt.Rows[i][10];
                        entry.OpDiv = (string)dt.Rows[i][11];
                        entry.StaffDiv = (string)dt.Rows[i][12];
                        entry.Office = (string)dt.Rows[i][13];
                        entry.Last_Name = (string)dt.Rows[i][14];
                        entry.First_Name = (string)dt.Rows[i][15];
                        entry.Site_Address = (string)dt.Rows[i][16];
                        entry.Floor = System.DBNull.Value.Equals(dt.Rows[i][17]) ? null : (string)dt.Rows[i][17];
                        entry.Room = (string)dt.Rows[i][18];
                        entry.Lumension_Report_Date = System.DBNull.Value.Equals(dt.Rows[i][19]) ? null : (DateTime?)Convert.ToDateTime(dt.Rows[i][19]);
                        entry.Lumension_Computer_Name = System.DBNull.Value.Equals(dt.Rows[i][20]) ? null : (String)dt.Rows[i][20];
                        entry.Lumension_Login_User = System.DBNull.Value.Equals(dt.Rows[i][21]) ? null : (String)dt.Rows[i][21];
                        entry.Received_Date = DateTime.Now;
                        entry.Returned_Date = null;
                        Models.D40 d40 = db.D40.SingleOrDefault(x => x.Asset_Tag == entry.Asset_Tag && x.Category == entry.Category);
                        if (d40 != null)
                        {
                            if (d40.Equals(entry))
                            {
                                del.Remove(d40);
                                continue;
                            }
                            ViewBag.modEntry.Add(d40, entry);
                            del.Remove(d40);
                            diff++;
                            continue;
                        }
                        //db.D40.Add(entry);
                        ViewBag.newEntry.Add(entry);
                        diff++;
                    }
                    ViewBag.del = del;
                    reader.Close();
                    if (diff == 0)
                    {
                        ViewBag.show = false;
                    }
                  
                    return View(db.D40.ToList());
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                    ViewBag.Show = false;
                }
            }
            return View(db.D40.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(List<D40.Models.D40> mod, List<D40.Models.D40> cat, List<D40.Models.D40> del)
        {
            if (mod!= null)
            {
                foreach (D40.Models.D40 entry in mod)
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(entry).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            if (cat != null) { 
                foreach (D40.Models.D40 entry in cat)
                {
                    Name user = db.Names.SingleOrDefault(x => x.Last_Name == entry.Last_Name && x.First_Name == entry.First_Name);
                    if (user == null)
                    {
                        user = new Name();
                        user.First_Name = entry.First_Name;
                        user.Last_Name = entry.Last_Name;
                        user.Active = true;
                        db.Names.Add(user);
                        db.SaveChanges();
                    }
                    entry.NameID = user.ID;
                    if (ModelState.IsValid)
                    {
                        db.D40.Add(entry);
                    }
            }
        }
            if (del != null)
            {
                foreach (D40.Models.D40 entry in del)
                {
                    int key = entry.NameID;
                    int key2 = entry.ID;
                    //Name user = db.Names.Find(key);
                    entry.Returned_Date = DateTime.Now;
                    db.Entry(entry).State = EntityState.Modified;
                    //if (user.Assets.Count == 1)
                    //{
                    //    user.Active = false;
                    //    db.Entry(user).State = EntityState.Modified;
                    //}
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Clear()
        {
            var context = from c in db.D40 select c;
            db.D40.RemoveRange(context);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: D40/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.D40 d40 = db.D40.Find(id);
            if (d40 == null)
            {
                return HttpNotFound();
            }
            return View(d40);
        }

        // GET: D40/Create
        //new Asset, ID is ID of name.
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Name user = db.Names.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            ViewBag.FN = user.First_Name;
            ViewBag.LN = user.Last_Name;
            return View();
        }

        // POST: D40/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Category,Record_ID,Asset_Tag,Asset_status,Serial_Number,BB_Phone,Refresh_Date,Model,Seat_Type,Service_Level,HHS_Billing,OpDiv,StaffDiv,Office,Last_Name,First_Name,Site_Address,Floor,Room,Lumension_Report_Date,Lumension_Computer_Name,Lumension_Login_User,Received_Date,NameID,Returned_Date")] Models.D40 d40)
        {
            if (ModelState.IsValid)
            {
                db.D40.Add(d40);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(d40);
        }

        // GET: D40/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.D40 d40 = db.D40.Find(id);
            if (d40 == null)
            {
                return HttpNotFound();
            }
            return View(d40);
        }

        // POST: D40/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Category,Record_ID,Asset_Tag,Asset_status,Serial_Number,BB_Phone,Refresh_Date,Model,Seat_Type,Service_Level,HHS_Billing,OpDiv,StaffDiv,Office,Last_Name,First_Name,Site_Address,Floor,Room,Lumension_Report_Date,Lumension_Computer_Name,Lumension_Login_User,Received_Date,NameID,Returned_Date")] Models.D40 d40)
        {
            if (ModelState.IsValid)
            {
                db.Entry(d40).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(d40);
        }

        // GET: D40/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.D40 d40 = db.D40.Find(id);
            if (d40 == null)
            {
                return HttpNotFound();
            }
            return View(d40);
        }

        // POST: D40/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include = "ID,Category,Record_ID,Asset_Tag,Asset_status,Serial_Number,BB_Phone,Refresh_Date,Model,Seat_Type,Service_Level,HHS_Billing,OpDiv,StaffDiv,Office,Last_Name,First_Name,Site_Address,Floor,Room,Lumension_Report_Date,Lumension_Computer_Name,Lumension_Login_User,Received_Date,NameID,Returned_Date")] Models.D40 d40)
        {
            db.Entry(d40).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult newName()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newName([Bind(Include = "ID,Last_Name,First_Name,Active,Office")] Name user)
        {
            if (ModelState.IsValid)
            {
                db.Names.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
        public ActionResult closeTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ticket.Closed_Date = DateTime.Now;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult openTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Name user = db.Names.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult openTicket([Bind(Include = "ID,Ticket_Num,Open_Date,Closed_Date,Description,NameID")]Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticket);
        }
        public ActionResult addSoftware(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Name user = db.Names.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            var software = from t in db.Software
                               select t;
            ViewBag.Software = new List<SelectListItem>();
            foreach (Software s in software)
            {
                ViewBag.Software.Add(new SelectListItem { Text = s.title, Value = s.ID.ToString() });
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addSoftware([Bind(Include = "ID,NameID,SoftwareID")] SoftwareName sn)
        {
            if (ModelState.IsValid)
            {
                SoftwareName s = db.SoftwareNames.FirstOrDefault(x => x.NameID == sn.NameID && x.SoftwareID == sn.SoftwareID);
                if (s != null)
                {
                    ModelState.AddModelError("Software", "This user already has "+s.Software.title);
                    ViewBag.ID = sn.ID;
                    var software = from t in db.Software
                                   select t;
                    ViewBag.Software = new List<SelectListItem>();
                    foreach (Software a in software)
                    {
                        ViewBag.Software.Add(new SelectListItem { Text = a.title, Value = a.ID.ToString() });
                    }
                    return View(sn);
                }
                db.SoftwareNames.Add(sn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }
        public ActionResult removeSoftware(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Name user = db.Names.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.user = user;
            var software = db.SoftwareNames.Where(x => x.NameID == id).Include(i=>i.Software);
            return View(software);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removeSoftware(int id)
        {
            SoftwareName software = db.SoftwareNames.Find(id);
            db.SoftwareNames.Remove(software);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult SoftwareIndex()
        {
            var software = db.Software
                .Include(i => i.Users.Select(c => c.User));
            return View(software.ToList());
        }
        public ActionResult newSoftware()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newSoftware([Bind(Include = "ID,title,license,num")] Software software)
        {
            if (ModelState.IsValid)
            {
                db.Software.Add(software);
                db.SaveChanges();
                return RedirectToAction("SoftwareIndex");
            }

            return View(software);
        }
        public ActionResult deleteSoftware(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Software software = db.Software.Find(id);
            if (software == null)
            {
                return HttpNotFound();
            }
            return View(software);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteSoftware(int id)
        {
            Software software = db.Software.Find(id);
            db.Software.Remove(software);
            db.SaveChanges();
            return RedirectToAction("SoftwareIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
