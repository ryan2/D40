using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using D40.Models;
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
            return View(db.D40.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase upload)
        {
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
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    for (int i = 0;i < dt.Rows.Count; i++){
                        Models.D40 entry = new Models.D40();
                        entry.Category = (string) dt.Rows[i][0];
                        entry.Record_ID = System.DBNull.Value.Equals(dt.Rows[i][1]) ? 0 : (int)((Double)dt.Rows[i][1]);
                        entry.Asset_Tag = (string) dt.Rows[i][2];
                        entry.Asset_status = (string)dt.Rows[i][3];
                        entry.Serial_Number = (string)dt.Rows[i][4];
                        entry.BB_Phone = System.DBNull.Value.Equals(dt.Rows[i][5]) ? "" : (string)dt.Rows[i][5];
                        entry.Refresh_Date = (DateTime)dt.Rows[i][6];
                        entry.Model = (string)dt.Rows[i][7];
                        entry.Seat_Type = (string)dt.Rows[i][8];
                        entry.Service_Level = (string)dt.Rows[i][9];
                        entry.HHS_Billing = (string)dt.Rows[i][10];
                        entry.OpDiv = (string)dt.Rows[i][11];
                        entry.StaffDiv = (string)dt.Rows[i][12];
                        entry.Office = (string)dt.Rows[i][13];
                        entry.Last_Name = (string)dt.Rows[i][14];
                        entry.First_Name = (string)dt.Rows[i][15];
                        entry.Site_Address = (string)dt.Rows[i][16];
                        entry.Floor = System.DBNull.Value.Equals(dt.Rows[i][17]) ? "" : (string)dt.Rows[i][17];
                        entry.Room = (string)dt.Rows[i][18];
                        entry.Lumension_Report_Date = System.DBNull.Value.Equals(dt.Rows[i][19]) ? null : (DateTime?)Convert.ToDateTime(dt.Rows[i][19]);
                        entry.Lumension_Computer_Name = System.DBNull.Value.Equals(dt.Rows[i][20]) ? "" : (String)dt.Rows[i][20];
                        entry.Lumension_Login_User = System.DBNull.Value.Equals(dt.Rows[i][21]) ? "" : (String)dt.Rows[i][21];
                        db.D40.Add(entry);
                    }
                    db.SaveChanges();
                    reader.Close();
                  
                    return View(db.D40.ToList());
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: D40/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Category,Record_ID,Asset_Tag,Asset_status,Serial_Number,BB_Phone,Refresh_Date,Model,Seat_Type,Service_Level,HHS_Billing,OpDiv,StaffDiv,Office,Last_Name,First_Name,Site_Address,Floor,Room,Lumension_Report_Date,Lumension_Computer_Name,Lumension_Login_User")] Models.D40 d40)
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
        public ActionResult Edit([Bind(Include = "ID,Category,Record_ID,Asset_Tag,Asset_status,Serial_Number,BB_Phone,Refresh_Date,Model,Seat_Type,Service_Level,HHS_Billing,OpDiv,StaffDiv,Office,Last_Name,First_Name,Site_Address,Floor,Room,Lumension_Report_Date,Lumension_Computer_Name,Lumension_Login_User")] Models.D40 d40)
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
        public ActionResult DeleteConfirmed(int id)
        {
            Models.D40 d40 = db.D40.Find(id);
            db.D40.Remove(d40);
            db.SaveChanges();
            return RedirectToAction("Index");
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
