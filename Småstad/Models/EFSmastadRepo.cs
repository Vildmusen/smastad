using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Småstad.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;


namespace Småstad.Models
{
    public class EFSmastadRepo : ISmastadRepo
    {
        private ApplicationDbContext context;
        private IHttpContextAccessor contextAcc;  

        public EFSmastadRepo (ApplicationDbContext ctx, IHttpContextAccessor cont)
        {
            context = ctx;
            contextAcc = cont;
        }

        public IQueryable<Department> Department => context.Departments;
        public IQueryable<Employee> Employee => context.Employees;
        public IQueryable<Errand> Errand => context.Errands;
        public IQueryable<ErrandStatus> ErrandStatus => context.ErrandStatuses;
        public IQueryable<Picture> Picture => context.Pictures;
        public IQueryable<Sample> Sample => context.Samples;
        public IQueryable<Sequence> Sequence => context.Sequences;

        /// <summary>
        /// Saves a new errand to the database, or updates a current one, depending on if the errand
        /// already has an "ErrandId".
        /// </summary>
        /// <param name="errand"> Errand to be saved or updated. </param>
        public void SaveErrand(Errand errand)
        {
            if(errand.ErrandId == 0)
            {
                errand.RefNumber = "2018-45-" + GetCurrentRefNum();
                errand.StatusId = "S_A";
                context.Errands.Add(errand);
                UpdateSequence();
            }
            else
            {
                Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandId == errand.ErrandId);
                if(dbEntry != null)
                {
                    context.Errands.Update(dbEntry = errand);
                }
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Saves a picture.
        /// </summary>
        /// <param name="pictures"></param>
        public void SavePic(List<Picture> pictures)
        {
            context.Pictures.Add(pictures.FirstOrDefault());
            context.SaveChanges();
        }

        /// <summary>
        /// Saves a sample.
        /// </summary>
        /// <param name="samples"></param>
        public void SaveSamp(List<Sample> samples)
        {
            context.Samples.Add(samples.FirstOrDefault());
            context.SaveChanges();
        }

        ///<summary> Matches and returns an errand on its property "ErrandId". </summary>
        ///<param name="id"> string of id to be matched. </param>
        public Task<Errand> GetErrandTask(int id)
        {
            return Task.Run(() =>
            {
                var errand = context.Errands.Where(ed => ed.ErrandId == id);
                return errand.First();
            });
        }

        /// <summary>
        /// Matches and returns an errand on its property "ErrandId".
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Errand GetErrand(int id)
        {
            var errand = context.Errands.Where(ed => ed.ErrandId == id);
            return errand.FirstOrDefault();
        }

        /// <summary>
        /// Returns the CurrentValue of the Sequence as a string.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentRefNum()
        {
            Sequence current = context.Sequences.FirstOrDefault();
            return current.CurrentValue.ToString();
        }

        /// <summary>
        /// Get all the pictures connected to an errand ID.
        /// </summary>
        /// <param name="id"> id of the current errand. </param>
        /// <returns> list of pictures </returns>
        public List<Picture> GetPictures(int id)
        {
            return context.Pictures.Where(p => p.ErrandId == id).ToList();
        }

        /// <summary>
        /// Get all the samples connected to an errand ID.
        /// </summary>
        /// <param name="id"> id of the current errand. </param>
        /// <returns> list of samples </returns>
        public List<Sample> GetSamples(int id)
        {
            return context.Samples.Where(p => p.ErrandId == id).ToList();
        }

        /// <summary>
        /// Gets the department Id of the current user
        /// </summary>
        /// <param name="name"> Name of the current user </param>
        /// <returns> DepartmentId </returns>
        public string getDepartment(string name)
        {
            Employee employee = context.Employees.Where(n => n.EmployeeId == name).FirstOrDefault();
            return employee.DepartmentId;
        }

        /// <summary>
        /// Gets the current users name.
        /// </summary>
        /// <returns> Name of the user. </returns>
        public string getCurrentUser()
        {
            return contextAcc.HttpContext.User.Identity.Name;
        }

        /// <summary>
        /// Returns the rank of the current user.
        /// </summary>
        /// <param name="name"> username to refer to </param>
        /// <returns> Rank of user </returns>
        public string getUserRank(string name)
        {
            Employee user = context.Employees.Where(p => p.EmployeeId == name).FirstOrDefault();
            return user.RoleTitle;
        }

        /// <summary>
        /// Returns a list to be used in the viewcomponent for dropdown lists.
        /// </summary>
        /// <param name="type"> type of list to be created. </param>
        /// <returns> list of string </returns>
        public Task<List<string>> getList(string type)
        {
            return Task.Run(() =>
            {
                if (type == "ErrandStatus")
                {
                    List<string> list = context.ErrandStatuses.Select(n => n.StatusName).ToList();
                    return list;
                }
                else if (type == "Investigator")
                {
                    string department = getDepartment(getCurrentUser());
                    return context.Employees.Where(x => x.DepartmentId == department).Select(n => n.EmployeeName).ToList();
                }
                else if (type == "Department")
                {
                    List<string> list = context.Departments.Select(n => n.DepartmentName).ToList();
                    return list;
                }
                else
                {
                    List<string> list = new List<string> { "Inget att visa." };
                    return list;
                }

            });
        }

        /// <summary>
        /// Selects all relevant information to be displayed as errand on the coordinator start page.
        /// </summary>
        /// <returns> List of DisplayErrand </returns>
        public List<DisplayErrand> DisplayAllErrands()
        {
            var errandList =
                from err in Errand
                join stat in ErrandStatus on err.StatusId equals stat.StatusId
                join dep in Department on err.DepartmentId equals dep.DepartmentId
                    into departmentErrand
                from deptE in departmentErrand.DefaultIfEmpty()
                join em in Employee on err.EmployeeId equals em.EmployeeId
                    into employeErrand
                from emptE in employeErrand.DefaultIfEmpty()
                orderby err.RefNumber descending
                select new DisplayErrand
                {
                    DateOfObservation = err.DateOfObservation,
                    ErrandId = err.ErrandId,
                    RefNumber = err.RefNumber,
                    TypeOfCrime = err.TypeOfCrime,
                    StatusName = stat.StatusName,
                    DepartmentName = (err.DepartmentId == null ? "Ej tillsatt" : deptE.DepartmentName),
                    EmployeeName = (err.EmployeeId == null ? "Ej tillsatt" : emptE.EmployeeName)
                };

            return errandList.ToList();
        }

        /// <summary>
        /// Displays the correct errands according to the managers department.
        /// </summary>
        /// <returns> List of DisplayErrand </returns>
        public List<DisplayErrand> DisplayDepartmentErrands()
        {
            string department = getDepartment(getCurrentUser());

            var errandList =
                from err in Errand
                join stat in ErrandStatus on err.StatusId equals stat.StatusId
                join dep in Department on err.DepartmentId equals dep.DepartmentId
                    where dep.DepartmentId == department // FIlter out departments
                join em in Employee on err.EmployeeId equals em.EmployeeId
                    into employeErrand
                from emptE in employeErrand.DefaultIfEmpty()
                orderby err.RefNumber descending
                select new DisplayErrand
                {
                    DateOfObservation = err.DateOfObservation,
                    ErrandId = err.ErrandId,
                    RefNumber = err.RefNumber,
                    TypeOfCrime = err.TypeOfCrime,
                    StatusName = stat.StatusName,
                    DepartmentName = dep.DepartmentName,
                    EmployeeName = (err.EmployeeId == null ? "Ej tillsatt" : emptE.EmployeeName)
                };

            return errandList.ToList();
        }

        /// <summary>
        /// Displays the correct errand according to which employee that is logged in.
        /// </summary>
        /// <returns> List of DisplayErrand </returns>
        public List<DisplayErrand> DisplayEmployeeErrands()
        {
            string employee = getCurrentUser();

            var errandList =
                from err in Errand
                join stat in ErrandStatus on err.StatusId equals stat.StatusId
                join dep in Department on err.DepartmentId equals dep.DepartmentId
                join em in Employee on err.EmployeeId equals em.EmployeeId
                    where em.EmployeeId == employee // filter out employees
                orderby err.RefNumber descending
                select new DisplayErrand
                {
                    DateOfObservation = err.DateOfObservation,
                    ErrandId = err.ErrandId,
                    RefNumber = err.RefNumber,
                    TypeOfCrime = err.TypeOfCrime,
                    StatusName = stat.StatusName,
                    DepartmentName = dep.DepartmentName,
                    EmployeeName = em.EmployeeName
                };

            return errandList.ToList();
        }

        /// <summary>
        /// Updates the CurrentValue of the Sequence with +1.
        /// </summary>
        public void UpdateSequence()
        { 
            Sequence current = context.Sequences.FirstOrDefault();
            current.CurrentValue++;
            context.SaveChanges();
        }

        /// <summary>
        /// Searches the table Errands for an errand with the input id, if it exists it gets deleted.
        /// </summary>
        /// <param name="id"> id to match with ErrandId. </param>
        /// <returns> True if correctly deleted, else false. </returns>
        public bool DeleteErrand(int id)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandId == id);
            if (dbEntry != null)
            {
                context.Errands.Remove(dbEntry);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
