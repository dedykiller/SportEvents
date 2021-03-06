﻿using SportEvents.Controllers.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class DataContext : DbContext
    {

        public DataContext()
            : base("dedekDB") 
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UsersInGroup> UsersInGroups { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UsersInEvent> UserInEvents { get; set; }
        public DbSet<PaymentPeriod> PaymentPeriods { get; set; }
        public DbSet<TypeOfPaymentForUserInPeriod> TypeOfPaymentForUserInPeriods { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>()
            //    .HasMany(a => a.Groups)
            //    .WithMany()
            //    .Map(x =>
            //    {
            //        x.MapLeftKey("User_Id");
            //        x.MapRightKey("Group_Id");
            //        x.ToTable("GroupsUsers");
            //    });

            modelBuilder.Entity<Event>()
                .HasRequired<Group>(a => a.Group)
                .WithMany(a => a.Events)
                .HasForeignKey(a => a.GrpId);


            //modelBuilder.Entity<UsersInEvent>().HasKey(x => new { x.UserId, x.EventId });


                
               
        }

        public List<UsersInEvent> GetAllUnspokenUsers(double numberOfDaysToEvent)
        {
            List<User> ReturnUsers = new List<User>();
            List<Event> AllEventsStartInExactDays = new List<Event>();
            List<UsersInEvent> UIN = new List<UsersInEvent>();

            DateTime NowPlusSomeDays = new DateTime();
            NowPlusSomeDays = DateTime.Now;
            NowPlusSomeDays = NowPlusSomeDays.AddDays(numberOfDaysToEvent);
         //   NowPlusSomeDays = NowPlusSomeDays.Date;

            AllEventsStartInExactDays = Events.Where(x =>  DbFunctions.TruncateTime(x.TimeOfEvent) == (NowPlusSomeDays.Date)).ToList();
            foreach (var item in AllEventsStartInExactDays)
	            {
                    UIN.AddRange(UserInEvents.Where(x=> x.EventId == item.Id).ToList());		 
	            }
            return UIN;

            //foreach (var item in UIN)
            //{
            //    ReturnUsers.Add(Users.Where(x => x.Id == item.UserId).Single());
            //}
            //return ReturnUsers;
        }

        public User GetUserById(int UserId)
        {
            User User = Users.Find(UserId);
            return User;
        }

        public List<Article> GetAllArticlesOfGroup(int GroupId)
        {
            return Articles.Where(x => x.GroupID == GroupId).ToList();
        }


        public PaymentPeriod GetActualPaymentPeriod(int GroupId)
        {
            return PaymentPeriods.Where(x => x.Start <= DateTime.Today && x.End >= DateTime.Today && x.GroupId == GroupId).Single();
        }

        public PaymentPeriod GetNextPaymentPeriod(PaymentPeriod ActualPaymentPeriod)
        {
            return PaymentPeriods.Where(x => x.Start > DateTime.Today && x.End >= DateTime.Today && x.GroupId == ActualPaymentPeriod.GroupId).Single();
        }

        public void SetDefaultTypeOfPaymentForUser(User User, PaymentPeriod PaymentPeriod)
        {
            TypeOfPaymentForUserInPeriod TypeOfPaymentForUserInPeriod = new TypeOfPaymentForUserInPeriod();
            TypeOfPaymentForUserInPeriod.User = User;
            TypeOfPaymentForUserInPeriod.PaymentPeriod = PaymentPeriod;
            TypeOfPaymentForUserInPeriod.UserId = User.Id;
            TypeOfPaymentForUserInPeriod.PaymentPeriodId = PaymentPeriod.Id;
            TypeOfPaymentForUserInPeriod.UserTypeOfPaymentInPeriod = TypeOfPaymentInPeriod.Cash;

            TypeOfPaymentForUserInPeriods.Add(TypeOfPaymentForUserInPeriod);
            SaveChanges();


        }

        public TypeOfPaymentForUserInPeriod GetTypeOfPaymentForUserInPeriod(User user, PaymentPeriod PaymentPeriod)
        {
            return TypeOfPaymentForUserInPeriods.Where(x => x.UserId == user.Id && x.PaymentPeriodId == PaymentPeriod.Id).Single();
        }

        public void UpdateParticipation (int EventId, int UserId, participation participation) {
            UserInEvents.Where(x => x.EventId == EventId && x.UserId == UserId).Single().participation = participation;
            UsersInEvent e = new UsersInEvent();
            SaveChanges();            
        }
        //Je už definované následující účtovací období po skončení aktuálního? Podle toho vrat ano, ne
        public bool IsAlreadyDefinedNextPaymentPeriodInThisGroup(int groupId)
        {
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            if (PaymentPeriods.FirstOrDefault(x=> x.Start > DateTime.Today && groupId == x.GroupId)  == null)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public bool IsActualPaymentPeriodOfThisGroupEnding (int groupId)
        {
            PaymentPeriod PaymentPeriod = new PaymentPeriod();
            if (PaymentPeriods.FirstOrDefault(x => x.Start.AddDays(2) > DateTime.Today && groupId == x.GroupId) == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public participation GetParticipation(int eventId, int userId)
        {
            return UserInEvents.Where(x => x.UserId == userId && x.EventId == eventId).Single().participation;
        }
        
        public bool IsEmailInDatabase(string email)
        {
            
            if (Users.Any(x => x.Email == email))
            {
                return true;
            }
            return false;
        }

        public List<PaymentPeriod> GetAllPaymentPeriodsOfGroup (int groupId)
        {
            return PaymentPeriods.Where(x => x.GroupId == groupId).ToList();
        }

        public List<Event> SelectEventsById(List<int> eventsIds)
        {
            List<Event> events = new List<Event>();
            foreach (var item in eventsIds)
            {
                //events = Events.Where(x => x.Id == item).ToList();// chyba
                events.Add(Events.Single(x => x.Id == item));

            }

            return events;
        }

        //public List<Event> takeSeveralLastRecords(int numberOfRecords)
        //{
        //    List<Event> events = new List<Event>();
        //    events = Events.skip
        //    return events;
        //}

        public List<Event> AllEventsOfGroup(int groupId)
        {
            List<Event> list = new List<Event>();
            list = Events.Where(x => x.GrpId == groupId).ToList();
            return list;
        }

        public List<Event> AllEventsWhereIsUserCreator(int User_Id)
        {
            
            List<Event> listOfEvents = new List<Event>();
            listOfEvents = Events.Where(x => x.CreatorId == User_Id).ToList();
            
            return listOfEvents;
        }
        //TODO : zde budou chyby ohledně podmínek u dat. atributů, zkontrolovat a opravit
        public List<Event> GetAllEventsOfPaymentPeriod (int periodId) 
        {
            int groupId = PaymentPeriods.Where(x => x.Id == periodId).Single().GroupId;
            PaymentPeriod PP = PaymentPeriods.Where(x => x.Id == periodId).Single();
            //List<PaymentPeriod> AllPaymentsPeriod = GetAllPaymentPeriodsOfGroup(groupId);
            return Events.Where(x => x.GrpId == groupId && PP.Start <= x.TimeOfEvent && PP.End >= x.TimeOfEvent).ToList();
                       
        }

        public List<Group> AllGroupsWhereIsUserCreator(int User_Id)
        {

            List<Group> listOfGroups = new List<Group>();
            listOfGroups = Groups.Where(x => x.Creator == User_Id).ToList();

            return listOfGroups;
        }

        public bool HasUserAnyGroupWhereIsCreator (int userId)
        {
            if (Groups.Any(x => x.Creator == userId)) return true;
            else return false;
        }

        public List<User> AllUsersInGroup(int GroupId)
        {
           

            List<UsersInGroup> listOfGroups = new List<UsersInGroup>();
            var list = new List<User>();


            listOfGroups = UsersInGroups.Where(x => x.GroupID == GroupId).ToList();

            foreach (UsersInGroup usersInGroup in listOfGroups)
            {
                User g = Users.Find(usersInGroup.UserID);

                list.Add(g);
            }

            return list;
        }
        
        public List<User> GetAllUsersPayingByCashOrAfterPeriod(List<User> UsersOfGroup, int paymentPeriodId, TypeOfPaymentInPeriod TOPIP) 
        {
            List<TypeOfPaymentForUserInPeriod> TypeOfPaymentForUserInPeriod = new List<TypeOfPaymentForUserInPeriod>();
            List<User> UsersPayingByCashOrAfterPeriod = new List<User>();
            foreach (var item in UsersOfGroup)
	        {
                if (TypeOfPaymentForUserInPeriods.Where(x => x.UserId == item.Id &&
                    x.UserTypeOfPaymentInPeriod == TOPIP && x.PaymentPeriodId == paymentPeriodId).SingleOrDefault() != null)
                {
                    TypeOfPaymentForUserInPeriod.Add(TypeOfPaymentForUserInPeriods.Where(x => x.UserId == item.Id &&
                    x.UserTypeOfPaymentInPeriod == TOPIP && x.PaymentPeriodId == paymentPeriodId).SingleOrDefault());
                }
                
	        }

            
            


            

            //if (TypeOfPaymentForUserInPeriod.Any(x=> x == null))
            //{
            //    return UsersPayingByCashOrAfterPeriod; 
            //}

            foreach (var item in TypeOfPaymentForUserInPeriod)
	        {
                
                UsersPayingByCashOrAfterPeriod.Add(Users.Where(x => x.Id == item.UserId).Single());
	        }

            return UsersPayingByCashOrAfterPeriod;                      
        
        }
        // vyber mi z listu
        public List<User> GetAllChargedUsers (List<User> PayingUsers, List<Event> EventsOfPeriod, int PaymentPeriodId) 
        {
            List<User> AllChargedUsers = new List<User>();
            List<UsersInEvent> UsInEvents = new List<UsersInEvent>();
            foreach (var eve in EventsOfPeriod)
            {
                foreach (var us in PayingUsers)
                {
                    
                        UsInEvents.Add(UserInEvents.Where(x => x.EventId == eve.Id && x.UserId == us.Id && (x.participation == participation.Yes ||
                    x.participation == participation.Unspoken)).SingleOrDefault());
                    
                    
                }
                        
            }

            
            foreach (var item in UserInEvents)
            {
                if (PayingUsers.Where(x => x.Id == item.UserId).SingleOrDefault() != null)
                {
                    AllChargedUsers.Add(PayingUsers.Where(x => x.Id == item.UserId).SingleOrDefault());
                }
                
            }
            AllChargedUsers = AllChargedUsers.Distinct().ToList();

            return AllChargedUsers;
            
        }

        public void AddUserToEvent(User User, Event Event)
        {       
            UsersInEvent UIE = new UsersInEvent();
            UIE.User = User;
            UIE.Event = Event;
            UserInEvents.Add(UIE);            
            SaveChanges();
        }

        public List<Group> AllGroupsWhereIsUserMember(int userId)
        {

            List<UsersInGroup> listOfGroups = new List<UsersInGroup>();
            var list = new List<Group>();


            listOfGroups = UsersInGroups.Where(x => x.UserID == userId).ToList();

            foreach (UsersInGroup usersInGroup in listOfGroups)
            {
                Group g = Groups.Find(usersInGroup.GroupID);

                list.Add(g);
            }

            return list;
        }

        public bool IsUserCreatorOfGroup(int user_Id, int grp_Id)
        {
            if (Groups.Any(x => x.Creator == user_Id && x.Id == grp_Id))
            {
                return true;
            }
            return false;
        }

        public string GetHashedPassword(string email)
        {
            
            return Users.Where(x => x.Email == email).Select(x => x.Password).Single();
        }

        public bool IsUserRegistered(string email, string hashedFormPassword)
        {
            if (IsEmailInDatabase(email) && UtilityMethods.ComparePasswords(hashedFormPassword, GetHashedPassword(email)))
            {
                return true;
            }
            return false;
        }

        public User GetUserByEmail(string email)
        {
            User user = (User) Users.Where(x => x.Email == email).Single();
            return user;
        }

        public Event GetEventById(int eventId)
        {
            return Events.FirstOrDefault(x => x.Id == eventId);    
        }

        public List<Event> GetEventsWhereIsThisParticipation(List<Event> ev, participation part, int userId) 
        {
            List<UsersInEvent> UIN = new List<UsersInEvent>();
            foreach (var item in ev)
            {
                UsersInEvent UsInEv = new UsersInEvent();

                UsInEv = UserInEvents.Where(x => x.EventId == item.Id && x.UserId == userId && (x.participation == part || x.participation == participation.Unspoken)).SingleOrDefault();
                if (UsInEv != null)
                {
                    UIN.Add(UsInEv);
                }
                
            }
            List<Event> returnEvents = new List<Event>();
            foreach (var item in UIN)
            {
                returnEvents.Add(Events.Where(x => x.Id == item.EventId).Single());
            }

            return returnEvents;
        }

        public List<User> UsersInEventParticipation (int evendId, participation part)
        {
            //participation part = new participation();
            
            //part = participation.Yes;
            List<User> users = new List<User>();
            List<UsersInEvent> userInEvents = new List<UsersInEvent>();

            userInEvents = UserInEvents.Where(x => x.EventId == evendId).ToList();
            foreach (var item in userInEvents)
            {
                if (Users.Where(x => x.Id == item.UserId && item.participation == part).SingleOrDefault() != null)
                {
                    users.Add(Users.Where(x => x.Id == item.UserId && item.participation == part).SingleOrDefault());
                }
                
            }
            return users;
        }

        public participation UserParticipationStatusOfEvent(int eventId, int userId)
        {
            return UserInEvents.Where(x => x.UserId == userId && x.EventId == eventId).Single().participation;
        }      





        public bool IsUserInGroup(int userId, int groupId)
        {

            if (UsersInGroups.Any(x => x.UserID == userId && x.GroupID == groupId))
            {
                return true;
            }
            return false;
        }
        public List<Event> GetEventByIdToList(List<int> listEventId)
        {
            List<Event> events = new List<Event>();
            foreach (var item in listEventId)
            {
                events.Add(Events.FirstOrDefault(x => x.Id == item));
            }
            return events;
        }

        public void AddAllUsersOfGroupToAllNewEvents(List<Event> events, int groupId) 
        {
            List<User> users = new List<User>();
            users = AllUsersInGroup(groupId);
            //List<Event> events = new List<Event>();
            

            foreach (var ev in events)
            {
                foreach (User us in users)
                {
                    UsersInEvent u = new UsersInEvent()
                    {
                        participation = participation.Unspoken,
                        User = us,
                        UserId = us.Id,
                        EventId = ev.Id,
                        Event = ev
                    };
                    UserInEvents.Add(u);
                    SaveChanges();
            }
            }
        }

        public void AddNewUserOfGroupToAllEvents(List<Event> events, int groupId, User user)
        {
            List<User> users = new List<User>();
            users = AllUsersInGroup(groupId);
            //List<Event> events = new List<Event>();


            foreach (var ev in events)
            {
                
                    UsersInEvent u = new UsersInEvent()
                    {
                        participation = participation.Unspoken,
                        User = user,
                        UserId = user.Id,
                        EventId = ev.Id,
                        Event = ev
                    };
                    UserInEvents.Add(u);
                    SaveChanges();
                
            }
        }

        public Group GetGroupById(int groupId)
        {
            return Groups.Where(x => x.Id == groupId).Single();
        }


        public List<Comment> getAllCommentsByParent(int? ParentID, ParentType parentType)
        {
            return Comments.Where(x => x.ParentID == ParentID && x.ParentType == parentType).ToList();
        }

        public List<PaymentPeriod> GetAllPaymentsPeriods()
        {
            return PaymentPeriods.ToList();
        }

        public List<Group> GetAllGroups()
        {
            return Groups.ToList();
        }

        public UsersInGroup GetUserInGroup(Group group, User user)
        {
            return UsersInGroups.Where(x => x.GroupID == group.Id && x.UserID == user.Id).Single();
        }

        internal bool isUserInEvent(int userID, int eventID)
        {
            if (UserInEvents.Any(x => x.UserId == userID && x.EventId == eventID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}