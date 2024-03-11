﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BITCollege_JL.Data;
using BITCollege_JL.Models;

namespace BITCollege_JL.Controllers
{
    public class AcademicProgramsController : Controller
    {
        private BITCollege_JLContext db = new BITCollege_JLContext();

        // GET: AcademicPrograms
        public ActionResult Index()
        {
            return View(db.AcademicPrograms.ToList());
        }

        // GET: AcademicPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            if (academicProgram == null)
            {
                return HttpNotFound();
            }
            return View(academicProgram);
        }

        // GET: AcademicPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademicPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AcademicProgramId,ProgramAcronym,Description")] AcademicProgram academicProgram)
        {
            if (ModelState.IsValid)
            {
                db.AcademicPrograms.Add(academicProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(academicProgram);
        }

        // GET: AcademicPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            if (academicProgram == null)
            {
                return HttpNotFound();
            }
            return View(academicProgram);
        }

        // POST: AcademicPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AcademicProgramId,ProgramAcronym,Description")] AcademicProgram academicProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(academicProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(academicProgram);
        }

        // GET: AcademicPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            if (academicProgram == null)
            {
                return HttpNotFound();
            }
            return View(academicProgram);
        }

        // POST: AcademicPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            academicProgram.Students.Clear();
            academicProgram.Courses.Clear();
            db.AcademicPrograms.Remove(academicProgram);
            db.SaveChanges();
            return RedirectToAction("Index");

            //Clears the students and courses associated with the Academic Program, but doesn't remove students or courses.
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
