using System.Collections.Specialized;
using System.Configuration;
using System.Web.Security;
using WebMatrix.WebData;
using TournamentReport.Models;

[assembly: WebActivator.PreApplicationStartMethod(typeof(TournamentReport.App_Start.SimpleMembershipMvc3), "Start")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(TournamentReport.App_Start.SimpleMembershipMvc3), "PostApplicationStart")]

namespace TournamentReport.App_Start
{
    public static class SimpleMembershipMvc3
    {
        public static readonly string EnableSimpleMembershipKey = "enableSimpleMembership";

        public static bool SimpleMembershipEnabled
        {
            get { return IsSimpleMembershipEnabled(); }
        }

        public static void PostApplicationStart()
        {
            // Modify the settings below as appropriate for your application
            WebSecurity.InitializeDatabaseConnection(
                connectionStringName: "TournamentReport.TournamentContext", 
                userTableName: "Users", 
                userIdColumn: "Id", 
                userNameColumn: "Name", 
                autoCreateTables: true);

            try {
                WebSecurity.CreateAccount("Chad", "password");
            }
            catch (MembershipCreateUserException e) {
                if (!e.Message.Contains("The username is already in use")) {
                    throw;
                }
            }

            // Comment the line above and uncomment these lines to use the IWebSecurityService abstraction
            //var webSecurityService = DependencyResolver.Current.GetService<IWebSecurityService>();
            //webSecurityService.InitializeDatabaseConnection(connectionStringName: "Default", userTableName: "Users", userIdColumn: "ID", userNameColumn: "Username", autoCreateTables: true);
        }

        public static void Start()
        {
            if (SimpleMembershipEnabled)
            {
                MembershipProvider membershipProvider = Membership.Providers["AspNetSqlMembershipProvider"];
                if (membershipProvider != null)
                {
                    MembershipProvider currentDefault = membershipProvider;
                    SimpleMembershipProvider provider2 = CreateDefaultSimpleMembershipProvider("AspNetSqlMembershipProvider", currentDefault);
                    Membership.Providers.Remove("AspNetSqlMembershipProvider");
                    Membership.Providers.Add(provider2);
                }
                Roles.Enabled = true;
                RoleProvider roleProvider = Roles.Providers["AspNetSqlRoleProvider"];
                if (roleProvider != null)
                {
                    RoleProvider provider6 = roleProvider;
                    SimpleRoleProvider provider4 = CreateDefaultSimpleRoleProvider("AspNetSqlRoleProvider", provider6);
                    Roles.Providers.Remove("AspNetSqlRoleProvider");
                    Roles.Providers.Add(provider4);
                }
            }
        }

        #region : Private Methods :

        private static bool IsSimpleMembershipEnabled()
        {
            bool flag;
            string str = ConfigurationManager.AppSettings[EnableSimpleMembershipKey];
            if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out flag))
            {
                return flag;
            }
            return true;
        }

        private static SimpleMembershipProvider CreateDefaultSimpleMembershipProvider(string name, MembershipProvider currentDefault)
        {
            MembershipProvider previousProvider = currentDefault;
            SimpleMembershipProvider provider = new SimpleMembershipProvider(previousProvider);
            NameValueCollection config = new NameValueCollection();
            provider.Initialize(name, config);
            return provider;
        }

        private static SimpleRoleProvider CreateDefaultSimpleRoleProvider(string name, RoleProvider currentDefault)
        {
            RoleProvider previousProvider = currentDefault;
            SimpleRoleProvider provider = new SimpleRoleProvider(previousProvider);
            NameValueCollection config = new NameValueCollection();
            provider.Initialize(name, config);
            return provider;
        }

        #endregion

    }
}
