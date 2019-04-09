using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Småstad.Models
{
    /// <summary> Public class of an errand. </summary>
    public class Errand
    {
        public int ErrandId { get; set; }
        public string RefNumber { get; set; }

        [Required(ErrorMessage = "Du måste ange en plats.")]
        [Display(Name = "Var har brottet skett någonstans?")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Du måste ange vilen typ av brott det var.")]
        [Display(Name = "Vilken typ av brott?")]
        public string TypeOfCrime { get; set; }

        [Required(ErrorMessage = "Du måste ange ett datum.")]
        [Display(Name = "När skedde brottet?")]
        [DisplayFormat(DataFormatString = "{0:yyyy - MM - dd}")]
        public DateTime DateOfObservation { get; set; }
        public string Observation { get; set; }
        public string InvestigatorInfo { get; set; }
        public string InvestigatorAction { get; set; }

        [Required(ErrorMessage = "Du måste ange ditt namn.")]
        [Display(Name = "Ditt namn (för- och efternamn):")]
        public string InformerName { get; set; }

        [RegularExpression("[0]{1}[7]{1}[0-9]{8}$", ErrorMessage = "Du måste ange ett giltigt telefonnummer.")]
        [Display(Name = "Din telefon:")]
        public string InformerPhone { get; set; }
        public string StatusId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }

        //Should fix a display or something to display placeholder text.
        public ICollection<Sample> Samples { get; set; }
        public ICollection<Picture> Pictures { get; set; }

    }

}
