using ecommerce.Models.DB;
using ecommerce.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecommerce.Models.EnctityManager
{
    public class UserManager
    {
        public void AddUserAccount(UserSignUpView user)
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                SYSUser SU = new SYSUser();
                SU.LoginName = user.LoginName;
                SU.PasswordEncryptedText = user.Password;
                SU.RowCreatedSYSUserID = user.SYSUserId > 0 ? user.SYSUserId : 1;
                SU.RowModifiedSYSUserID = user.SYSUserId > 0 ? user.SYSUserId : 1;
                SU.RowCreatedDateTime = DateTime.Now;
                SU.RowModifiedDateTime = DateTime.Now;

                db.SYSUsers.Add(SU);
                db.SaveChanges();

                SYSUserProfile SUP = new SYSUserProfile();
                SUP.SYSUserID = SU.SYSUserID;
                SUP.FirstName = user.FirstName;
                SUP.LastName = user.LastName;
                SUP.Gender = user.Gender;
                SUP.RowCreatedSYSUserID = user.SYSUserId > 0 ? user.SYSUserId : 1;
                SUP.RowModifiedSYSUserID = user.SYSUserId > 0 ? user.SYSUserId : 1;
                SUP.RowCreatedDateTime = DateTime.Now;
                SUP.RowModifiedDateTime = DateTime.Now;

                db.SYSUserProfiles.Add(SUP);
                db.SaveChanges();

                if (user.LOOKUPRoleID > 0)
                {
                    SYSUserRole SUR = new SYSUserRole();
                    SUR.LOOKUPRoleID = user.LOOKUPRoleID;
                    SUR.SYSUserID = user.SYSUserId;
                    SUR.IsActive = true;
                    SUR.RowCreatedSYSUserID = user.SYSUserId > 0 ? user.SYSUserId : 1;
                    SUR.RowModifiedSYSUserID = user.SYSUserId > 0 ? user.SYSUserId : 1;
                    SUR.RowCreatedDateTime = DateTime.Now;
                    SUR.RowModifiedDateTime = DateTime.Now;

                    db.SYSUserRoles.Add(SUR);
                    db.SaveChanges();
                }
            }
        }

        public bool IsLoginNameExist(string loginName)
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                //Vérifiaction en base si le loginName ne correspond à aucun autre en base, si oui on retourne un bool
                return db.SYSUsers.Where(o => o.LoginName.Equals(loginName)).Any();
            }
        }

        public string GetUserPassword(string loginName)
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                var user = db.SYSUsers.Where(o => o.LoginName.ToLower().Equals(loginName.ToLower()));
                if (user.Any())
                {
                    return user.FirstOrDefault().PasswordEncryptedText;
                }
                else
                {
                    return string.Empty;
                }

            }
        }

        public bool isUserRole(string loginName, string roleName)
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                // ? pour vérifier si la condition est vérifiée sinon on renvoit le default (= ? : ); FirstOrDefault() équivalent de Optional en java
                SYSUser SU = db.SYSUsers.Where(u => u.LoginName.ToLower().Equals(loginName.ToLower()))?.FirstOrDefault();
                if (SU != null)
                {
                    Console.WriteLine("userId ==" + SU.SYSUserID);
                    var roles = from q in db.SYSUserRoles
                                join r in db.LOOKUPRoles on q.LOOKUPRoleID equals r.LOOKUPRoleID
                                where r.RoleName.Equals(roleName) && q.SYSUserID.Equals(SU.SYSUserID)
                                select r.RoleName;
                    Console.WriteLine("All roles ==" + roles);
                    if (roles != null)
                    {
                        return roles.Any();
                    }

                }
            }
            return false;

        }

        public List<LOOKUPAvailableRole> GetAllRoles()
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                var roles = db.LOOKUPRoles.Select(o => new LOOKUPAvailableRole
                {
                    LOOKUPRoleID = o.LOOKUPRoleID,
                    RoleName = o.RoleName,
                    RoleDescription = o.RoleDescription
                }).ToList();

                return roles;
            }
        }

        public int GetUserID(string loginName)
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                var user = db.SYSUsers.Where(o => o.LoginName.ToLower().Equals(loginName.ToLower()));
                if (user.Any())
                {
                    return user.FirstOrDefault().SYSUserID;
                }
                else
                {
                    return 0;
                }
            }
        }

        public List<UserProfileView> GetAllUserProfiles()
        {
            List<UserProfileView> profiles = new List<UserProfileView>();
            using (ecommerceEntities db = new ecommerceEntities())
            {
                UserProfileView UPV;
                var users = db.SYSUsers.ToList();
                foreach (SYSUser u in db.SYSUsers)
                {
                    UPV = new UserProfileView();
                    UPV.SYSUserID = u.SYSUserID;
                    UPV.LoginName = u.LoginName;
                    UPV.Password = u.PasswordEncryptedText;
                    // Profile
                    var SUP = db.SYSUserProfiles.Find(u.SYSUserID);
                    if (SUP != null)
                    {
                        UPV.FirstName = SUP.FirstName;
                        UPV.LastName = SUP.LastName;
                        UPV.Gender = SUP.Gender;
                    }
                    // Role
                    var SUR = db.SYSUserRoles.Where(o => o.SYSUserID.Equals(u.SYSUserID));
                    if (SUR.Any())
                    {
                        var userRole = SUR.FirstOrDefault();
                        //Console.WriteLine(userRole);
                        UPV.LOOKUPRoleID = userRole.LOOKUPRoleID;
                        UPV.RoleName = userRole.LOOKUPRole.RoleName;
                        UPV.IsRoleActive = userRole.IsActive;
                    }

                    profiles.Add(UPV);
                }
            }
            return profiles;
        }

        public UserDataView GetUserDataView(string loginName)
        {
            UserDataView UDV = new UserDataView();
            //profiles
            List<UserProfileView> profiles = GetAllUserProfiles();
            //roles
            List<LOOKUPAvailableRole> roles = GetAllRoles();
            int? userAssignedRoleID = 0, userID = 0;
            string userGender = string.Empty;
            userID = GetUserID(loginName);
            using (ecommerceEntities db = new ecommerceEntities())
            {
                userAssignedRoleID = db.SYSUserRoles.Where(o => o.SYSUserID == userID)?.FirstOrDefault().LOOKUPRoleID;
                userGender = db.SYSUserProfiles.Where(o => o.SYSUserID == userID)?.FirstOrDefault().Gender;

            }
            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender
            {
                Test = "Male",
                Value = "M"
            });
            genders.Add(new Gender
            {
                Test = "Female",
                Value = "F"
            });

            UDV.UserProfile = profiles;
            UDV.UserRole = new UserRoles
            {
                SelectRoleID = userAssignedRoleID,
                UserRoleList = roles
            };
            UDV.UserGender = new UserGender
            {
                SelectedGender = userGender,
                Gender = genders
            };

            return UDV;
        }

        public void UpdateUserAccount(UserProfileView user)
        {
            using (ecommerceEntities db = new ecommerceEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        SYSUser SU = db.SYSUsers.Find(user.SYSUserID);
                        SU.LoginName = user.LoginName;
                        SU.PasswordEncryptedText = user.Password;
                        SU.RowCreatedSYSUserID = user.SYSUserID;
                        SU.RowModifiedSYSUserID = user.SYSUserID;
                        SU.RowCreatedDateTime = DateTime.Now;
                        SU.RowModifiedDateTime = DateTime.Now;
                        db.SaveChanges();
                        // profile
                        var userProfile = db.SYSUserProfiles.Where(o => o.SYSUserID == user.SYSUserID);
                        if (userProfile.Any())
                        {
                            SYSUserProfile SUP = userProfile.FirstOrDefault();
                            SUP.SYSUserID = SU.SYSUserID;
                            SUP.FirstName = user.FirstName;
                            SUP.LastName = user.LastName;
                            SUP.Gender = user.Gender;
                            SUP.RowCreatedSYSUserID = user.SYSUserID;
                            SUP.RowModifiedSYSUserID = user.SYSUserID;
                            SUP.RowCreatedDateTime = DateTime.Now;
                            SUP.RowModifiedDateTime = DateTime.Now;

                            db.SaveChanges();

                        }
                        // role
                        if (user.LOOKUPRoleID > 0)
                        {
                            var userRole = db.SYSUserRoles.Where(o => o.SYSUserID == user.SYSUserID);
                            SYSUserRole SUR = null;
                            if (userRole.Any())
                            {
                                SUR = userRole.FirstOrDefault();
                                SUR.LOOKUPRoleID = user.LOOKUPRoleID;
                                SUR.SYSUserID = user.SYSUserID;
                                SUR.IsActive = true;
                                SUR.RowCreatedSYSUserID = user.SYSUserID;
                                SUR.RowModifiedSYSUserID = user.SYSUserID;
                                SUR.RowCreatedDateTime = DateTime.Now;
                                SUR.RowModifiedDateTime = DateTime.Now;

                                
                            }
                            else
                            {
                                SUR = new SYSUserRole();
                                SUR.LOOKUPRoleID = user.LOOKUPRoleID;
                                SUR.SYSUserID = user.SYSUserID;
                                SUR.IsActive = true;
                                SUR.RowCreatedSYSUserID = user.SYSUserID;
                                SUR.RowModifiedSYSUserID = user.SYSUserID;
                                SUR.RowCreatedDateTime = DateTime.Now;
                                SUR.RowModifiedDateTime = DateTime.Now;

                                db.SYSUserRoles.Add(SUR);
                            }

                            db.SaveChanges();
                        }

                        dbContextTransaction.Commit();

                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
                
            }
        }

    }
}