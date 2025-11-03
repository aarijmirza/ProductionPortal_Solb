using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DAL.Models.ViewModel;

namespace DAL.Models
{
    public class RspLogin : Rsp
    {
        public UserBLL login { get; set; }
        public List<UserRoleBLL> UserRole { get; set; }
    }
    public class UserBLL
    {
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public List<UserRoleBLL> UserRole { get; set; }
    }
}
