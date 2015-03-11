using SportEvents.Controllers.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SportEvents.Models
{
    public class DataContext : DbContext
    {

        public DataContext()
            : base("masterDB") 
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UsersInGroup> UsersInGroups { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<UsersInEvent> UserInEvents { get; set; }
      //  public DbSet<UsersInEvent> UsersInEvents { get; set; }

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


            modelBuilder.Entity<UsersInEvent>().HasKey(x => new { x.UserId, x.EventId });


                
               
        }

        
        public bool IsEmailInDatabase(string email)
        {
            
            if (Users.Any(x => x.Email == email))
            {
                return true;
            }
            return false;
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

        public List<Group> AllGroupsWhereIsUserCreator(int User_Id)
        {

            List<Group> listOfGroups = new List<Group>();
            listOfGroups = Groups.Where(x => x.Creator == User_Id).ToList();

            return listOfGroups;
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

        public void AddUserToEvent(User User, Event Event)
        {            
        //    User u = Users.First(x => x.Id == User.Id);
        //   // u.UserInEvents.Add(Event);
        //    Event e = Events.First(x => x.Id == Event.Id);
        //   // e.UserInEvents.Add(User);

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

        public bool IsUserInGroup(int userId, int groupId)
        {

            if (UsersInGroups.Any(x => x.UserID == userId && x.GroupID == groupId))
            {
                return true;
            }
            return false;
        }

        
    }
}