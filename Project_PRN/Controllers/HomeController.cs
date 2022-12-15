using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Project_PRN.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Data.OleDb;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

namespace Project_PRN.Controllers
{
    public class HomeController : Controller
    {

        DataProvider dtp = new DataProvider();
        PrnProjectContext context = new PrnProjectContext();
        private IHostingEnvironment Environment;
        private IConfiguration Configuration;
        public HomeController(IHostingEnvironment _environment, IConfiguration _configuration)
        {
            Environment = _environment;
            Configuration = _configuration;
        }
        public IActionResult Index()
        {
            var subject = context.Subjects.ToList();
            ViewData["slist"] = subject;

            return View();
        }
        public IActionResult SubjectManager()
        {
            var subject = context.Subjects.ToList();
            ViewData["slist"] = subject;
            return View();
        }
        public IActionResult InsertSubject(string Id, string Name, int TeacherId, string Image)
        {
            Subject s = new Subject();
            s.Id = Id;
            s.Name = Name;
            s.TeacherId = TeacherId;
            s.Image = Image;
            context.Subjects.Add(s);
            context.SaveChanges();
            return RedirectToAction("SubjectManager");
        }
        public IActionResult TransmissionSubject(string id, string type)
        {
            using (PrnProjectContext context = new PrnProjectContext())
            {

                var subject = context.Subjects.FirstOrDefault(x => x.Id == id);
                if (type == null)
                {
                    return View("UpdateSubjectForm", subject);
                }
                else
                {
                    var question = context.Questions.Where(x => x.SubjectId == id).ToList();
                    string s = "";
                    foreach (var item in question)
                    {
                        string[] data = item.Content.Split("/");
                        foreach (var str in data)
                        {
                            s += str;
                            s += "\n";
                        }
                    }
                    ViewBag.S = s;
                    ViewBag.question = question;
                    return View("QuestionManager", subject);
                }

            }
        }
        public IActionResult UpdateSubject(string Id, string Name, int TeacherId, string Image)
        {
            using (PrnProjectContext context = new PrnProjectContext())
            {
                var subject = context.Subjects.FirstOrDefault(x => x.Id == Id);
                subject.Name = Name;
                subject.TeacherId = TeacherId;
                subject.Image = Image;
                context.SaveChanges();
                return RedirectToAction("SubjectManager");
            }

        }
        public IActionResult DeleteSubject(string id)
        {
            using (PrnProjectContext context = new PrnProjectContext())
            {
                try
                {
                    var subject = context.Subjects.FirstOrDefault(x => x.Id == id);
                    context.Subjects.Remove(subject);
                    context.SaveChanges();
                }
                catch (Exception)
                {

                }

            }
            return RedirectToAction("SubjectManager");
        }


        public IActionResult Import(string id)
        {
            string text = System.IO.File.ReadAllText(@"D:\demo.txt");
            string[] list = text.Trim().Split("--");
            List<string> listQ = new List<string>();
            List<string> listA = new List<string>();
            for (int i = 0; i < list.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    listQ.Add(list[i]);
                }
                else
                {
                    listA.Add(list[i]);
                }
            }
            List<Question> qlist = new List<Question>();
            int qid;
            using (PrnProjectContext context = new PrnProjectContext())
            {
                qid = context.Questions.ToList().Count + 1;
            }
            for (int i = 0; i < listQ.Count; i++)
            {
                Question question = new Question(qid++, listQ[i], listA[i], id);
                qlist.Add(question);
            }
            context.Questions.AddRange(qlist);
            context.SaveChanges();

            return RedirectToAction("SubjectManager");
        }
        public IActionResult TransmissionQuestion(int qid, string subjectid)
        {

            var ques = context.Questions.FirstOrDefault(x => x.Id == qid);
            ViewBag.sid = subjectid;
            return View("UpdateQuestionForm", ques);


        }
        public IActionResult UpdateQuestion(string Content, string QuestionAnswer, int Id, string sid)
        {

            var ques = context.Questions.FirstOrDefault(x => x.Id == Id);
            ques.Content = Content;
            ques.QuestionAnswer = QuestionAnswer;
            context.SaveChanges();
            return RedirectToAction("TransmissionSubject", new { id = sid, type = 1 });


        }
        public IActionResult DeleteQuestion(int qid, string subjectid)
        {
            var ques = context.Questions.FirstOrDefault(x => x.Id == qid);
            context.Questions.Remove(ques);
            context.SaveChanges();
            return RedirectToAction("TransmissionSubject", new { id = subjectid, type = 1 });
        }
        public IActionResult AddQuestion(string content, string answer, string sid)
        {
            int qlist;
            using (PrnProjectContext context = new PrnProjectContext())
            {
                qlist = context.Questions.ToList().Count + 1;
            }
            context.Questions.Add(new Question(qlist, content, answer, sid));
            context.SaveChanges();
            return RedirectToAction("TransmissionSubject", new { id = sid, type = 1 });
        }

        public IActionResult ShowQuestion(string id)
        {

            var question = context.Questions.Where(x => x.SubjectId == id).ToList();
            foreach (var item in question)
            {
                string[] data = item.Content.Split("/");
                string s = "";
                foreach (var str in data)
                {
                    s += str;
                    s += "\n";
                }
                item.Content = s;
            }

            ViewBag.question = question;
            return View("ShowQuestion");
        }
        [HttpPost]
        public IActionResult AddMultiple(IFormFile file, string id)
        {
            string text = "";
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                text = reader.ReadToEnd();
            }

            string[] list = text.Trim().Split("--");
            List<string> listID = new List<string>();
            List<string> listQ = new List<string>();
            List<string> listA = new List<string>();
            int o = 0;

            while (o < list.Length - 1)
            {
                int j = o + 1;
                int k = o + 2;
                listID.Add(list[o].Trim());
                listQ.Add(list[j].Trim());
                listA.Add(list[k].Trim());
                o = o + 3;
            }
            List<Question> qlist = new List<Question>();
            List<int> eid = new List<int>();
            using (PrnProjectContext context = new PrnProjectContext())
            {
                var ques = context.Questions.Where(x => x.SubjectId == id).ToList();
                foreach (var item in ques)
                {
                    eid.Add(item.Id);
                }
            }
            int qid;
            for (int i = 0; i < listQ.Count; i++)
            {

                qid = int.Parse(listID[i]);
                Question question;
                if (eid.Contains(qid))
                {
                    question = context.Questions.FirstOrDefault(x => x.Id == qid);
                    question.Content = listQ[i];
                    question.QuestionAnswer = listA[i];
                }
                else
                {
                    question = new Question(qid, listQ[i], listA[i], id);
                    qlist.Add(question);
                }


            }
            if (qlist.Count > 0)
            {
                context.Questions.AddRange(qlist);
            }
            context.SaveChanges();

            return RedirectToAction("SubjectManager");
        }
        [HttpPost]
        public IActionResult ImportExcel(IFormFile postedFile, string id)
        {
            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                List<string> str = new List<string>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string s = dt.Rows[i].ItemArray[j].ToString();
                        str.Add(s);
                    }
                }
                List<string> listID = new List<string>();
                List<string> listQ = new List<string>();
                List<string> listA = new List<string>();
                int o = 0;

                while (o < str.Count)
                {
                    int j = o + 1;
                    int k = o + 2;
                    listID.Add(str[o].Trim());
                    listQ.Add(str[j].Trim());
                    listA.Add(str[k].Trim());
                    o = o + 3;
                }
                List<Question> qlist = new List<Question>();
                List<int> eid = new List<int>();
                using (PrnProjectContext context = new PrnProjectContext())
                {
                    var ques = context.Questions.Where(x => x.SubjectId == id).ToList();
                    foreach (var item in ques)
                    {
                        eid.Add(item.Id);
                    }
                }
                int qid;
                for (int i = 0; i < listQ.Count; i++)
                {

                    qid = int.Parse(listID[i]);
                    Question question;
                    if (eid.Contains(qid))
                    {
                        question = context.Questions.FirstOrDefault(x => x.Id == qid);
                        question.Content = listQ[i];
                        question.QuestionAnswer = listA[i];
                    }
                    else
                    {
                        question = new Question(qid, listQ[i], listA[i], id);
                        qlist.Add(question);
                    }


                }
                if (qlist.Count > 0)
                {
                    context.Questions.AddRange(qlist);
                }
                context.SaveChanges();




            }

            return RedirectToAction("SubjectManager");
        }


    }
}
