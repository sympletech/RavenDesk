using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;
using TeamMgmtCal.Core.Email;
using TeamMgmtCal.Core.Helpers;
using TeamMgmtCal.Core.UserManagement;

namespace TeamMgmtCal.Core.Security
{
    public class SecurityWorker
    {
        public IDataContext Db { get; set; }

        public SecurityWorker(IDataContext db)
        {
            this.Db = db;
        }

        public enum LoginResponse
        {
            InvalidUsername,
            InvalidPassword,
            Success
        }

        public LoginResponse AuthenticateUser(string username, string password)
        {
            var user = Db.Session.Query<AccountUser>().FirstOrDefault(
                x => x.Username == username && x.Active == true);
            
            if (user == null) return LoginResponse.InvalidUsername;

            string pwdHash = EncryptionHelpers.HashPassword(password);
            if (user.Password != pwdHash) return LoginResponse.InvalidPassword;

            this.UserInfo = user;
            return LoginResponse.Success;
        }

        public AccountUser UserInfo { get; private set; }


        public bool ForgotPassword(string email)
        {
            var aUser = Db.Session.Query<AccountUser>().FirstOrDefault(x => x.Email == email);
            if (aUser != null)
            {
                string tempPassword = Guid.NewGuid().ToString();
                aUser.NewPassword = tempPassword;
                aUser.Save();

                var eWorker = new EmailWorker();
                eWorker.SendForgetPasswordEmail(aUser, tempPassword);

                return true;
            }

            return false;
        }
    }
}
