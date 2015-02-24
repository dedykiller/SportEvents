using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportEvents.Models.Application
{
    public class GroupsBO
    {
        private DataContext db = new DataContext();


        public List<Group> Index()
        {
            return db.Groups.ToList();
        }

        public Group GetGroupById(int? id)
        {
            Group group = db.Groups.Find(id);

            return group;
        }


        public void CreateGroup(Group group, User user)
        {
            user = db.GetUserByEmail(user.Email);
            group.Creator = user.Id;
            group.CreateTime = DateTime.Now;            
            UsersInGroup userIngroup = new UsersInGroup(); 
            group.StartOfPaymentPeriod = DateTime.Now;            
            group.NumberOfUsersInGroup += 1;
            db.Groups.Add(group);
            db.SaveChanges();
            userIngroup.UserID = user.Id;
            userIngroup.GroupID = group.Id;
            db.UsersInGroups.Add(userIngroup);
            
            db.SaveChanges();
        }

        public void AddUserToGroup(Group group, User user)
        {
            if (db.IsUserInGroup(user.Id, group.Id) == false)   
            {
                group.NumberOfUsersInGroup += 1;
                user = db.GetUserByEmail(user.Email);
           
                UsersInGroup userIngroup = new UsersInGroup();
                userIngroup.UserID = user.Id;
                userIngroup.GroupID = group.Id;
                db.UsersInGroups.Add(userIngroup);
                db.SaveChanges();
                
            }
            // TODO : dodělat výpis zda byl uživatel nebo nebyl přidán do skupiny
            
        }


        public void EditGroup(Group group)
        {

            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();
        }


        public void DeleteGroup(Group group)
        {
            db.Groups.Remove(group);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}