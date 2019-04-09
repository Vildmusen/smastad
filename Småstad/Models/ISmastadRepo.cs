using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Småstad.Models
{
    public interface ISmastadRepo
    {
        //Create
        void SaveErrand(Errand errand);
        void SavePic(List<Picture> pictures);
        void SaveSamp(List<Sample> samples);

        //Read
        IQueryable<Errand> Errand { get; }
        IQueryable<Department> Department { get; }
        IQueryable<ErrandStatus> ErrandStatus { get; }
        IQueryable<Employee> Employee { get; }
        IQueryable<Picture> Picture { get; }
        IQueryable<Sample> Sample { get; }
        IQueryable<Sequence> Sequence { get; }

        Task<Errand> GetErrandTask(int id);
        Task<List<string>> getList(string tpye);

        Errand GetErrand(int id);
        List<Picture> GetPictures(int id);
        List<Sample> GetSamples(int id);

        string GetCurrentRefNum();
        string getDepartment(string name);
        string getCurrentUser();
        string getUserRank(string name);
        
        List<DisplayErrand> DisplayAllErrands();
        List<DisplayErrand> DisplayDepartmentErrands();
        List<DisplayErrand> DisplayEmployeeErrands();

        //Update
        void UpdateSequence();

        //Delete
        bool DeleteErrand(int ErrandId);

    }
}
