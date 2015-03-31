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

        public List<Group> IndexCreator(int userId)
        {
            
            return db.AllGroupsWhereIsUserCreator(userId);
        }

        public List<Group> IndexMember(int userId)
        {

            return db.AllGroupsWhereIsUserMember(userId);
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
            group.CreateTime = DateTime.Today;            
            UsersInGroup userIngroup = new UsersInGroup(); 
            //group.StartOfPaymentPeriod = DateTime.Now;            
            group.NumberOfUsersInGroup += 1;
            group.CreatorFullName = user.FirstName + " " + user.Surname;
            db.Groups.Add(group);
            db.SaveChanges();
            userIngroup.UserID = user.Id;
            userIngroup.GroupID = group.Id;
            db.UsersInGroups.Add(userIngroup);
            db.AddNewUserOfGroupToAllEvents(db.AllEventsOfGroup(group.Id), group.Id, user);
            db.PaymentPeriods.Add(new PaymentPeriod
            {
                GroupId = group.Id,
                Start = DateTime.Today,
                End = group.EndOfPaymentPeriod
            });
            db.SaveChanges();
          //  SetDefaultTypeOfPaymentForUser(user, group);
            
        }

        public void ChangeTypeOfPayment (User User, Group Group,TypeOfPaymentInPeriod TypeOfPaymentPeriod)
        {
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            PaymentPeriod = db.PaymentPeriods.Where(x => x.GroupId == Group.Id && x.Start < DateTime.Now && x.End > DateTime.Now).Single();

            db.TypeOfPaymentForUserInPeriods.Where(x => x.PaymentPeriodId == PaymentPeriod.Id && User.Id == x.User.Id).Single().UserTypeOfPaymentInPeriod = TypeOfPaymentPeriod;
            db.SaveChanges();

        }

        public void CreatePaymentPeriod(Group group, DateTime start, DateTime end)
        {
            
        }

        public void SetDefaultTypeOfPaymentForUser(User User, Group Group)
        {
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            PaymentPeriod = db.PaymentPeriods.Where(x => x.GroupId == Group.Id && x.Start < DateTime.Now && x.End > DateTime.Now).Single();

            TypeOfPaymentForUserInPeriod TypeOfPaymentForUserInPeriod = new TypeOfPaymentForUserInPeriod();
            TypeOfPaymentForUserInPeriod.User = User;
            TypeOfPaymentForUserInPeriod.PaymentPeriod = PaymentPeriod;
            TypeOfPaymentForUserInPeriod.UserId = User.Id;
            TypeOfPaymentForUserInPeriod.PaymentPeriodId = PaymentPeriod.Id;
            TypeOfPaymentForUserInPeriod.UserTypeOfPaymentInPeriod = TypeOfPaymentInPeriod.Cash;

            db.TypeOfPaymentForUserInPeriods.Add(TypeOfPaymentForUserInPeriod);
            db.SaveChanges();
            

        }

        public void AddUserToGroup(Group group, User user)
        {

            group.NumberOfUsersInGroup += 1;
            user = db.GetUserByEmail(user.Email);

                
           
            UsersInGroup userIngroup = new UsersInGroup();
            userIngroup.UserID = user.Id;
            userIngroup.GroupID = group.Id;
            db.UsersInGroups.Add(userIngroup);
            AddUserToAllEventsOfGroup(group.Id, user);
            SetDefaultTypeOfPaymentForUser(user, group);
            db.SaveChanges();

            // TODO : dodělat výpis zda byl uživatel nebo nebyl přidán do skupiny
            
        }

        public List<Event> AllEventsOfGroup(int groupId)
        {

            return db.AllEventsOfGroup(groupId);
        }

        public void AddUsersToAllNewEvents(Event e)
        {
            List<User> users = db.AllUsersInGroup(e.GrpId);
            foreach (var item in users)
            {
                db.AddUserToEvent(item, e);
                
            }
        }

        public void AddUserToAllEventsOfGroup(int groupId, User user)
        {
            List<Event> AllEventsOfGroup = new List<Event>();
            AllEventsOfGroup = db.AllEventsOfGroup(groupId);            
            foreach (var item in AllEventsOfGroup)
            {
                db.AddUserToEvent(user, item);
            }
            
        }

        public bool IsUserInGroup(int userId, int groupId)
        {
            return db.IsUserInGroup(userId, groupId);
        }

      //  public void AddUserToEvent (int groupId, User user)


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


        public List<Article> GetAllArticlesOfGroup(int groupId)
        {
            return db.GetAllArticlesOfGroup(groupId);
        }
    }
}