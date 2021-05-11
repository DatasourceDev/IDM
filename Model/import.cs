using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using IDM.Extensions;

namespace IDM.Models
{
    public class import
    {
        [Key]
        public Int64 id { get; set; }

        public string basic_sn { get; set; }

        public string basic_uid { get; set; }
        public string basic_givenname { get; set; }

        public string cu_thcn { get; set; }
        public string cu_thsn { get; set; }

        public string cu_jobcode { get; set; }
        public string cu_pplid { get; set; }
        public string system_org { get; set; }
        public string faculty_shot_name { get; set; }
        public string structure_1 { get; set; }
        public string structure_2 { get; set; }
        public string status { get; set; }
        public string cu_CUexpire { get; set; }

        public ImportType import_Type { get; set; }
        public IDMUserType system_idm_user_types { get; set; }
        public ImportCreateOption import_create_option { get; set; }


        public bool ImportVerify { get; set; }

        public int ImportRow { get; set; }

        public string ImportRemark { get; set; }

        [NotMapped]
        public string LockStaus { get; set; }

    }



}
