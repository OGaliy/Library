using System;
using System.Linq;
using System.Web.Security;
using Library.Models;
using System.Data.Entity;

namespace Library.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] roles = new string[] { };
            using(BookContext db = new BookContext())
            {
                User user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == username);
                if (user != null && user.Role != null)
                    roles = new string[] { user.Role.Name };
                return roles;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using(BookContext db = new BookContext())
            {
                User user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == username);
                if (user != null && user.Role != null && user.Role.Name == roleName)
                    return true;
                else
                    return false;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}