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
            group.Creator = user;
            user.Groups.Add(group);
            group.Users.Add(user);
            group.CreateTime = DateTime.Now;
            group.StartOfPaymentPeriod = DateTime.Now;
            group.EndOfPaymentPeriod = DateTime.Now.AddMonths(group.PaymentPeriodLength);
            group.NumberOfUsersInGroup += 1;
            db.Groups.Add(group);
            db.SaveChanges();
        }

        public void AddUserToGroup(Group group, User user)
        {

            group.NumberOfUsersInGroup += 1;
            user = db.GetUserByEmail(user.Email);
            user.Groups.Add(group);
            group.Users.Add(user);
            db.SaveChanges();
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