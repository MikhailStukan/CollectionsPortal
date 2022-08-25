using CollectionsPortal.Models;

namespace CollectionsPortal.ViewModels
{
    public class UserViewModel
    {

        public IEnumerable<User> Users { get; set; }

        public int UsersPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(Users.Count() / (double)UsersPerPage));
        }

        public IEnumerable<User> Paginated()
        {
            return Users.OrderBy(u => u.Id).Skip((CurrentPage - 1) * UsersPerPage).Take(UsersPerPage);
        }
    }
}
