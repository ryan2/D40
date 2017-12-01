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
using ClosedXML.Excel;
using ClosedXML.Extensions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace D40.Controllers
{
    public class D40Controller : Controller
    {
        private D40DBContext db = new D40DBContext();

        // GET: D40
        public ActionResult Index(string catList, string searchString)
        {
            ViewBag.search = searchString;
            var categoryList = new List<string>(3) { "Active","Inactive","All"};
            ViewBag.catList = new SelectList(categoryList, catList);
            var viewModels = new NameIndexData();
            viewModels.Users = db.Names
                .Include(i => i.Assets)
                .Include(i => i.Software.Select(c => c.Software))
                .Include(i => i.Tickets.Select(c=>c.Asset));
            viewModels.Users = viewModels.Users.OrderBy(x => x.Last_Name);
            if (!String.IsNullOrEmpty(searchString))
            {
                viewModels.Users = viewModels.Users.Where(s => s.First_Name.Contains(searchString) || s.Last_Name.Contains(searchString));
            }
            if (String.IsNullOrEmpty(catList))
            {
                catList = "Active";
            }
            if (catList == "Active")
            {
                viewModels.Users = viewModels.Users.Where(s => s.Active);
            }
            else if (catList == "Inactive")
            {
                viewModels.Users = viewModels.Users.Where(s => !s.Active);
            }
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
                entries = entries.Where(s => s.Category.Equals(catList));
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
            ViewBag.all = new List<Models.D40>();
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
                    //
                    upload.SaveAs(Path.GetTempPath() + upload.FileName);

                    //

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
                        ViewBag.all.Add(entry);
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
        public ActionResult Update(List<D40.Models.D40> mod,List<bool> modc, List<D40.Models.D40> cat, List<bool> catc, List<D40.Models.D40> del,List<bool> delc, List<D40.Models.D40> all)
        {
            ViewBag.mod = new List<D40.Models.D40>();
            ViewBag.cat = new List<D40.Models.D40>();
            ViewBag.del = new List<D40.Models.D40>();
            ViewBag.all = all;
            if (mod != null)
            {
                int i = 0;
                foreach (D40.Models.D40 entry in mod)
                {
                    if (modc[i] == true)
                    {
                        if (ModelState.IsValid)
                        {
                            db.Entry(entry).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.mod.Add(entry);
                    }
                    i++;
                }
            }
            if (cat != null)
            {
                int i = 0;
                foreach (D40.Models.D40 entry in cat)
                {
                    
                    if (catc[i] == true)
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
                    else
                    {
                        ViewBag.cat.Add(entry);
                    }
                    i++;
                }
            }
            if (del != null)
            {
                int i = 0;
                foreach (D40.Models.D40 entry in del)
                {
                    if (delc[i] == true)
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
                    else
                    {
                        ViewBag.del.Add(entry);
                    }
                    i++;
                }
            }
            db.SaveChanges();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Excel(List<D40.Models.D40> mod, List<D40.Models.D40> cat, List<D40.Models.D40> del, List<D40.Models.Disputes> dis, List<D40.Models.D40> all)
        {
            if (mod == null && cat == null && del == null)
            {
                ModelState.AddModelError("File", "There are no changes to the D40");
                return View("Update");
            }
            int i = 1;
            int z = 0;
            int len;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ASPE D40 Modifications");
            System.Reflection.PropertyInfo[] properties = typeof(Models.D40).GetProperties();
            string[] prop = new string[25];
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                if (property.Name == "NameID")
                {
                    break;
                }
                prop[z] =property.Name;
                z++;
            }
            var array = new List<string[]>();
            array.Add(prop);
            ws.Range(1, 2,1,36).AsRange().AddToNamed("Properties");
            ws.Cell(1, 1).InsertData(array);
            ws.Cell(1, 28).Value = "Change Description";
            ws.Cell(1, 29).Value = "Change Reason";
            ws.Cell(1, 30).Value = "Change Field To";
            ws.Cell(1, 31).Value = "Ticket Number (for removes)";
            ws.Cell(1, 32).Value = "Date of Call to Change";
            ws.Cell(1, 33).Value = "Comments";
            ws.Cell(1, 34).Value = "LM Response";
            ws.Cell(1, 35).Value = "LM Rationale";
            ws.Cell(1, 36).Value = "Remove Asset";
            //
            i += 1;
            var list = new List<D40.Models.D40>();
            var list2 = new List<D40.Models.Disputes>();
            foreach (D40.Models.D40 item in all)
            {
                list.Add(item);
                ws.Cell(i, 1).InsertData(list);
                D40.Models.Disputes temp = dis.Find(t => t.Asset_Tag == item.Asset_Tag);
                if (temp!= null)
                {
                    list2.Add(temp);
                    ws.Cell(i, 27).InsertData(list2);
                    list2.Remove(temp);
                }
                list.Remove(item);
                i++;
            }

            //
            /*
            if (mod != null)
            {
                len = mod.Count;
                ws.Cell(i + 1, 1).InsertData(mod);
                i += len+1;
            }
            if (cat != null)
            {
                len = cat.Count;
                ws.Cell(i, 1).InsertData(cat);
                i += len;
            }
            if (del != null)
            {
                ws.Cell(i, 1).InsertData(del);
            }
            if (dis != null)
            {
                ws.Cell(2, 27).InsertData(dis);
            }
            */
            ws.Column(26).Delete();
            ws.Column(26).Delete();
            ws.Column(35).Delete();
            ws.Column(35).Delete();
            ws.Column(24).Delete();
            ws.Column(24).Delete();
            ws.Column(1).Delete();
            // Prepare the style for the Properties
            var propStyle = wb.Style;
            propStyle.Font.Bold = true;
            propStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            propStyle.Fill.BackgroundColor = XLColor.LightBlue;
            wb.NamedRanges.NamedRange("Properties").Ranges.Style = propStyle;
            ws.Columns().AdjustToContents();
            foreach (D40.Models.Disputes dispute in dis)
            {
                if (ModelState.IsValid)
                {
                    db.Disputes.Add(dispute);
                }
            }
            db.SaveChanges();
            return wb.Deliver("ASPED40Review.xlsx");
        }
        public ActionResult disputes_view(DateTime? Date, string Tag,bool? sort)
        {
            if (!sort.HasValue)
            {
                sort = false;
            }
            var disputes = from t in db.Disputes
                           select t;
            if (Date.HasValue)
            {
                disputes = disputes.Where(x => x.Date.Month == Date.Value.Month && x.Date.Year == Date.Value.Year);
                ViewBag.Date = Date.ToString();
            }
            if (!String.IsNullOrEmpty(Tag))
            {
                disputes = disputes.Where(x => x.Asset_Tag == Tag);
                ViewBag.Tag = Tag;
            }
            if (sort.Value)
            {
                disputes = disputes.OrderBy(t => t.Asset_Tag);
            }
            ViewBag.sort = sort;

            return View(disputes.ToList());
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
            var categoryList = new List<string>(4) { "Computer", "Phone", "Printer", "Phone Services" };
            ViewBag.catList = new SelectList(categoryList);
            var sl = new List<string>(3) { "Silver", "Gold", "Platinum" };
            ViewBag.Service = new SelectList(sl);
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
            var officeList = new List<string>(4) { "IO", "HP", "HSP", "SDP","DALTCP","OPPS" };
            ViewBag.office = new SelectList(officeList);       
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
        public ActionResult editName(int? id)
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
            var officeList = new List<string>(4) { "IO", "HP", "HSP", "SDP", "DALTCP", "OPPS" };
            if (!String.IsNullOrEmpty(user.Office))
            {
                ViewBag.office = new SelectList(officeList, user.Office);
            }
            else
            {
                ViewBag.office = new SelectList(officeList);
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editName([Bind(Include = "ID,Last_Name,First_Name,Active,Office")] Name user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public ActionResult deleteName(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Name user = db.Names.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            db.Names.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult removeName(int? id)
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
            user.Active = false;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
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
            var assets = db.D40.Where(x => x.NameID == id);
            ViewBag.assets = new List<SelectListItem>();
            foreach (D40.Models.D40 s in assets)
            {
                ViewBag.assets.Add(new SelectListItem { Text = s.Category + " "+s.Asset_Tag, Value = s.ID.ToString() });
            }
            ViewBag.user = user;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult openTicket([Bind(Include = "ID,Ticket_Num,Open_Date,Closed_Date,Description,NameID,D40ID")]Ticket ticket)
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
            ViewBag.user = user;
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
                    ViewBag.user = s.User;
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
        public ActionResult SoftwareIndex(string catList, string searchString)
        {
            ViewBag.search = searchString;
            var categoryList = new List<string>();
            var software = db.Software
                .Include(i => i.Users.Select(c => c.User));
            foreach (Software s in software)
            {
                categoryList.Add(s.title);
            }
            ViewBag.catList = new SelectList(categoryList, catList);
            if (!String.IsNullOrEmpty(searchString))
            {
                software = software.Where(s => s.Users.FirstOrDefault(x => x.User.First_Name.Contains(searchString)) != null || s.Users.FirstOrDefault(x => x.User.Last_Name.Contains(searchString)) != null);
            }
            if (!String.IsNullOrEmpty(catList))
            {
                software = software.Where(s => s.title == catList);
            }
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
        public ActionResult updateSoftware(int? id)
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
        public ActionResult updateSoftware([Bind(Include = "ID,title,license,num")] Software software)
        {
            if (ModelState.IsValid)
            {
                db.Entry(software).State = EntityState.Modified;
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
        public ActionResult editPrices(DateTime? FY)
        {
            if (FY == null)
            {
                var priceList = from t in db.Prices
                                select t;
                priceList = priceList.OrderBy(t => t.FY);
                ViewBag.prices = new List<SelectListItem>();
                foreach (Prices s in priceList)
                {
                    ViewBag.prices.Add(new SelectListItem { Text = s.FY.ToString("\\'yy"), Value = s.FY.ToString() });
                }
            return View();
            }
            Prices price = db.Prices.First(x => x.FY == FY);
            return View(price);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editPrices([Bind(Include = "ID,C_G,C_S,C_P,Ph_G,Ph_S,Ph_P,Pr_S,Pr_G,Pr_P,Ps_S,Ps_P,Ps_G,FY,D_S,D_G,D_P")] Prices price)
        {
            if (ModelState.IsValid)
            {
                db.Entry(price).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Prices_Index");
            }
            return View(price);
        }
        public ActionResult newPrice(int? FY)
        {
            if (FY != null)
            {
                DateTime Fiscal = new DateTime((int)FY,1,1);
                Prices price = db.Prices.FirstOrDefault(x => x.FY == Fiscal);
                if (price == null)
                {
                    ViewBag.FY = Fiscal;
                    return View();
                }
                return RedirectToAction("editPrices", new { @FY = Fiscal });
            }
            ViewBag.FYList = new List<SelectListItem>();
            DateTime now = new DateTime(DateTime.Now.Year, 1, 1);
            now = now.AddYears(-3);
            for (int i = 0; i < 7; i++)
            {
                ViewBag.FYList.Add(new SelectListItem { Text = now.ToString("\\'yy"), Value = now.Year.ToString() });
                now = now.AddYears(1);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newPrice([Bind(Include = "ID,C_G,C_S,C_P,Ph_G,Ph_S,Ph_P,Pr_S,Pr_G,Pr_P,Ps_S,Ps_P,Ps_G,FY,D_S.D_G,D_P")] Prices price)
        {
            if (ModelState.IsValid)
            {
                db.Prices.Add(price);
                db.SaveChanges();
                return RedirectToAction("Prices_Index");
            }
            return View(price);
        }
        public ActionResult Prices_Index(DateTime? d, DateTime? d1)
        {
            if (d1 != null)
            {
                if (d == null)
                {
                    d = d1;
                }
                //Range Search for Prices_Index
                else
                {
                    List<decimal> cc = new List<decimal>();
                    List<decimal> pc = new List<decimal>();
                    List<decimal> prc = new List<decimal>();
                    List<decimal> psc = new List<decimal>();
                    List<decimal> total = new List<decimal>();
                    int i = numMonths((DateTime) d, (DateTime) d1);
                    for (int j = 0; j < i; j++)
                    {
                        DateTime month = (DateTime)d;
                        month = month.AddMonths(j);
                        DateTime fy = fiscalYear(month);
                        IQueryable<D40.Models.D40> Assets = monthAssets(month);
                        List<decimal> t = assetsPrice(Assets, fy);
                        total.Add(t[0]);
                        cc.Add(t[1]);
                        pc.Add(t[2]);
                        prc.Add(t[3]);
                        psc.Add(t[4]);
                    }
                    ViewBag.total = total;
                    ViewBag.cc = cc;
                    ViewBag.pc = pc;
                    ViewBag.prc = prc;
                    ViewBag.psc = psc;
                    ViewBag.Month = d;
                    ViewBag.Range = true;
                    return View();
                }
            }
            ViewBag.Range = false;
            //Standard Current or single month search
            IQueryable<D40.Models.D40> curr;
            if (d != null)
            {
                curr = monthAssets((DateTime) d);
            }
            else {
                curr = from t in db.D40
                           select t;
                curr = curr.Where(x => x.Returned_Date == null);
            }
            var priceList = from t in db.Prices
                        select t;
            if (d == null)
            {
                d = DateTime.Now;
            }
            d = fiscalYear((DateTime)d);
            ViewBag.price = priceList.First(x=>x.FY == d);
            ViewBag.Pss = 0;
            ViewBag.Psg = 0;
            ViewBag.Psp = 0;
            ViewBag.Prs = 0;
            ViewBag.Prg = 0;
            ViewBag.Prp = 0;
            ViewBag.Php = 0;
            ViewBag.Phs = 0;
            ViewBag.Phg = 0;
            ViewBag.Cs = 0;
            ViewBag.Cg = 0;
            ViewBag.Cp = 0;
            foreach (Models.D40 asset in curr)
            {
                string cat = asset.Category;
                switch(cat)
                {
                    case "Computer":
                        if (asset.Service_Level == "Silver")
                        {
                            ViewBag.Cs += 1;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            ViewBag.Cg += 1;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            ViewBag.Cp += 1;
                        }
                        break;
                    case "Printer":
                        if (asset.Service_Level == "Silver")
                        {
                            ViewBag.Prs += 1;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            ViewBag.Prg += 1;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            ViewBag.Prp += 1;
                        }
                        break;
                    case "Phone":
                        if (asset.Service_Level == "Silver")
                        {
                            ViewBag.Phs += 1;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            ViewBag.Phg += 1;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            ViewBag.Php += 1;
                        }
                        break;
                    case "Phone Services":
                        if (asset.Service_Level == "Silver")
                        {
                            ViewBag.Pss += 1;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            ViewBag.Psg += 1;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            ViewBag.Psp += 1;
                        }
                        break;
                }
            }
            return View();
        }

        public int numMonths(DateTime d1, DateTime d2)
        {
            int num;
            int y1 = d1.Year;
            int y2 = d2.Year;
            int m1 = d1.Month;
            int m2 = d2.Month;
            if (y1 == y2)
            {
                num = m2 - m1 + 1;
            }
            else
            {
                num = ((y2 - y1) * 12) + m2 - m1 + 1;
            }
            return num;
        }
        public DateTime fiscalYear(DateTime d)
        {
            DateTime fy = new DateTime(d.Year, 1, 1);
            if (d.Month > 9)
            {
                fy = fy.AddYears(1);
            }
            return fy;
        }
        public IQueryable<D40.Models.D40> monthAssets(DateTime d)
        {
            int m = d.Month;
            int y = d.Year;
            DateTime start = new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month));
            DateTime end = new DateTime(d.Year,d.Month,1);

            var d40 = from t in db.D40
                      select t;
            d40 = d40.Where(x => (x.Received_Date <= start && x.Returned_Date >= end) || (x.Received_Date <= start && x.Returned_Date == null));
            return d40;
        }
        public List<decimal> assetsPrice(IQueryable<D40.Models.D40> assets, DateTime FY)
        {
            var priceList = from t in db.Prices
                            select t;
            Prices price = priceList.FirstOrDefault(x => x.FY == FY);
            decimal cc = 0;
            decimal pc = 0;
            decimal prc = 0;
            decimal psc = 0;
            foreach (D40.Models.D40 asset in assets)
            {
                string cat = asset.Category;
                switch (cat)
                {
                    case "Computer":
                        if (asset.Service_Level == "Silver")
                        {
                            cc += price.C_S;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            cc += price.C_G;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            cc += price.C_P;
                        }
                        break;
                    case "Printer":
                        if (asset.Service_Level == "Silver")
                        {
                            prc += price.Pr_S;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            prc += price.Pr_G;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            prc += price.Pr_P;
                        }
                        break;
                    case "Phone":
                        if (asset.Service_Level == "Silver")
                        {
                            pc += price.Ph_S;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            pc += price.Ph_G;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            pc += price.Ph_P;
                        }
                        break;
                    case "Phone Services":
                        if (asset.Service_Level == "Silver")
                        {
                            psc += price.Ps_S;
                        }
                        else if (asset.Service_Level == "Gold")
                        {
                            psc += price.Ps_G;
                        }
                        else if (asset.Service_Level == "Platinum")
                        {
                            psc += price.Ps_P;
                        }
                        break;
                }
            }
            decimal total = cc + pc + prc + psc;
            List<decimal> prices = new List<decimal>();
            prices.Add(total);
            prices.Add(cc);
            prices.Add(pc);
            prices.Add(prc);
            prices.Add(psc);
            return prices;
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
