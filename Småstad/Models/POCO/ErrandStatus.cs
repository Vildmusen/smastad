using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Småstad.Models
{
    ///<summary> Public class of errand statuses. </summary>
    public class ErrandStatus
    {
        [Key]
        public string StatusId { get; set; }

        public string StatusName { get; set; }
    }
}
