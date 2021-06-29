using IDM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDM.DTO
{
    public class Result
    {
        public bool result { get; set; }
        public string Message { get; set; }

    }
    public class Organization
    {
        public string ouname { get; set; }
        public string ou { get; set; }
        public string schemaname { get; set; }
        public string path { get; set; }

    }
    public class SearchDTO
    {
        public string text_search { get; set; }
        public string logstatus_search { get; set; }

        public IDMUserType? usertype_search { get; set; }
        public string log_type_search { get; set; }

        public Status? status_search { get; set; }
        public string dfrom { get; set; }
        public string dto { get; set; }
        public string import_option { get; set; }

        public string option { get; set; }
        public ScriptFormat script_format { get; set; }
        public string script_param { get; set; }


        private int _pageno; // field
        public int pageno {
            get
            {
                if (_pageno == 0)
                    return 1;
                return _pageno;
            }
            set { _pageno = value; }
        }
        private int _pagelen; // field
        public int pagelen
        {
            get
            {
                if (_pagelen == 0)
                    return 1;
                return _pagelen;
            }
            set { _pagelen = value; }
        }
        
        public int itemcnt { get; set; }

        public ReturnCode? code { get; set; }
        public string msg { get; set; }

        private IQueryable<object> _lists;
        
        public IQueryable<object> lists {
            get
            {                
                return _lists;
            }
            set
            {
                _lists = value;
            }
        }
    }

    public class ImportDTO
    {
        public string cu_pplid { get; set; }
        public string basic_uid { get; set; }

    }
    public class GetPasswordDTO : BaseDTO
    {
        [MaxLength(10)]
        [Required]
        public string cu_jobcode { get; set; }


        [MaxLength(13)]
        public string cu_pplid { get; set; }

        [MaxLength(500)]
        public string basic_sn { get; set; }

        [MaxLength(500)]
        public string basic_givenname { get; set; }
        
    }
    public class LoginDTO : BaseDTO
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(100)]
        public string Password { get; set; }
    }
    public class ChangePasswordDTO : BaseDTO
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,16}$", ErrorMessage = "รหัสผ่านจะต้องประกอบด้วยตัวพิมพ์ใหญ่อย่างน้อย 1 ตัว ตัวพิมพ์เล็กอย่างน้อย 1 ตัวและตัวเลขอย่างน้อย 1 ตัว")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,16}$", ErrorMessage = "รหัสผ่านจะต้องประกอบด้วยตัวพิมพ์ใหญ่อย่างน้อย 1 ตัว ตัวพิมพ์เล็กอย่างน้อย 1 ตัวและตัวเลขอย่างน้อย 1 ตัว")]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangePassword2DTO : BaseDTO
    {
        [Required]
        public int UserID { get; set; }

        public string Code { get; set; }



        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,16}$", ErrorMessage = "รหัสผ่านจะต้องประกอบด้วยตัวพิมพ์ใหญ่อย่างน้อย 1 ตัว ตัวพิมพ์เล็กอย่างน้อย 1 ตัวและตัวเลขอย่างน้อย 1 ตัว")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        [StringLength(16, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,16}$", ErrorMessage = "รหัสผ่านจะต้องประกอบด้วยตัวพิมพ์ใหญ่อย่างน้อย 1 ตัว ตัวพิมพ์เล็กอย่างน้อย 1 ตัวและตัวเลขอย่างน้อย 1 ตัว")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangePassword3DTO
    {
        [Required]
        public string basic_uid { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,16}$", ErrorMessage = "รหัสผ่านจะต้องประกอบด้วยตัวพิมพ์ใหญ่อย่างน้อย 1 ตัว ตัวพิมพ์เล็กอย่างน้อย 1 ตัวและตัวเลขอย่างน้อย 1 ตัว")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        [StringLength(16, ErrorMessage = "รหัสผ่านต้องไม่น้อยกว่า {2} ตัวและไม่เกิน {1} ตัว", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,16}$", ErrorMessage = "รหัสผ่านจะต้องประกอบด้วยตัวพิมพ์ใหญ่อย่างน้อย 1 ตัว ตัวพิมพ์เล็กอย่างน้อย 1 ตัวและตัวเลขอย่างน้อย 1 ตัว")]

        public string ConfirmPassword { get; set; }
    }
    public class MacAddressDTO : BaseDTO
    {
        [MaxLength(17)]
        [Display(Name = "รหัสประจำอุปกรณ์ 1")]
        public string MacAddress1 { get; set; }
        [MaxLength(17)]
        [Display(Name = "รหัสประจำอุปกรณ์ 2")]
        public string MacAddress2 { get; set; }
        [MaxLength(17)]
        [Display(Name = "รหัสประจำอุปกรณ์ 3")]
        public string MacAddress3 { get; set; }

        public int? MacAddressNumShow { get; set; }

    }
    public class BaseDTO
    {

    }

    public class OTPDTO : BaseDTO
    {
        [Required]
        [MaxLength(6)]
        public string OTP { get; set; }
        public string RefNo { get; set; }
        public bool Renew { get; set; }

    }
    public class ForgotPasswordDTO : BaseDTO
    {
        [Required]
        [MaxLength(150)]
        public string Code { get; set; }

        public SendMessageType SendMessageType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class SMSModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Msnlist { get; set; }
        public string Msg { get; set; }
        public string Sender { get; set; }

    }

    public class PasswordGenerateDTO 
    {
        public int PasswordCnt { get; set; }
        public int Length { get; set; }
        public bool Number { get; set; }
        public bool Lower { get; set; }
        public bool Upper { get; set; }

        public List<string> Passwords { get; set; }
    }

    public class ShowPasswordDTO
    {
        public string Password { get; set; }
    }
    public class script_temp
    {
        public Int64 id { get; set; }
        public string displayname { get; set; }

        public string login_name { get; set; }
        public string password_initial { get; set; }
        public string email_address { get; set; }
        public string server_name { get; set; }
        public string expire { get; set; }
        public string status_id { get; set; }
        public string org { get; set; }
        public DateTime? create_date { get; set; }
        public DateTime? receive_date { get; set; }
        public string manage_by { get; set; }
        public string ticket { get; set; }
        public visual_fim_user visual_fim_user { get; set; }
    }
}
