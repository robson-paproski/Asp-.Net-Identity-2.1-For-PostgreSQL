using System;
using System.Collections.Generic;


namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Classe que representa a tabela AspNetUsers
    /// </summary>
    public class UserTable<TUser>
        where TUser :IdentityUser
    {
        private PostgreSQLDatabase _database;
        
        internal const string tableName           = "AspNetUsers";
        internal const string fieldID             = "Id";
        internal const string fieldUserName       = "UserName";
        internal const string fieldPassword       = "PasswordHash";
        internal const string fieldSecurityStamp  = "SecurityStamp";
        internal const string fieldEmail          = "Email";
        internal const string fieldEmailConfirmed = "EmailConfirmed";               
        internal const string fieldPhoneNumber          = "PhoneNumber";
        internal const string fieldPhoneNumberConfirmed = "PhoneNumberConfirmed";
        internal const string fieldTwoFactorEnabled     = "TwoFactorEnabled";
        internal const string fieldLockoutEndDate       = "LockoutEndDateUtc";
        internal const string fieldLockoutEnabled       = "LockoutEnabled";
        internal const string fieldAccessFailedCount    = "AccessFailedCount";



        internal static string fullTableName      = Consts.Schema.Quoted() + "." + tableName.Quoted();

        //TODO: Adicionar os campos faltantes para validação de lockout, phonenumber, e two steps authentication

        /// <summary>
        /// Construtor que Instancia a base postgresql
        /// </summary>
        /// <param name="database"></param>
        public UserTable(PostgreSQLDatabase database)
        {
            _database = database;
        }


        /// <summary>
        /// Function to load an user object
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>

        private TUser loadUser(List<Dictionary<string, string>> rows)
        {
            TUser user = null;
            

            if (rows != null && rows.Count == 1)
            {
                var row = rows[0];
                user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row[fieldID];
                user.UserName = row[fieldUserName];
                user.PasswordHash = string.IsNullOrEmpty(row[fieldPassword]) ? null : row[fieldPassword];
                user.SecurityStamp = string.IsNullOrEmpty(row[fieldSecurityStamp]) ? null : row[fieldSecurityStamp];
                user.Email = string.IsNullOrEmpty(row[fieldEmail]) ? null : row[fieldEmail];
                user.EmailConfirmed = row[fieldEmailConfirmed] == "True";
                user.PhoneNumber = string.IsNullOrEmpty(row[fieldPhoneNumber]) ? null : row[fieldPhoneNumber];
                user.PhoneNumberConfirmed = row[fieldPhoneNumberConfirmed] == "1" ? true : false;
                user.LockoutEnabled = row[fieldLockoutEnabled] == "1" ? true : false;
                user.LockoutEndDateUtc = string.IsNullOrEmpty(row[fieldLockoutEndDate]) ? DateTime.Now : DateTime.Parse(row[fieldLockoutEndDate]);
                user.AccessFailedCount = string.IsNullOrEmpty(row[fieldAccessFailedCount]) ? 0 : int.Parse(row[fieldAccessFailedCount]);
                user.TwoFactorEnabled = row[fieldTwoFactorEnabled] == "1" ? true : false;
            }
            return user;

        }


        /// <summary>
        /// Gets the user's name, provided with an ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(string userId)
        {
            string commandText = "SELECT " + fieldUserName.Quoted() + " FROM " + fullTableName + " WHERE " + fieldID.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Gets the user's ID, provided with a user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserId(string userName)
        {            
            if (userName != null)
                userName = userName.ToLower();
            string commandText = "SELECT " + fieldID.Quoted() + " FROM " + fullTableName + " WHERE lower(" + fieldUserName.Quoted() + ") = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns an TUser given the user's id.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public TUser GetUserById(string userId)
        {

            string commandText = "SELECT * FROM " + fullTableName + " WHERE " + fieldID.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            var rows = _database.Query(commandText, parameters);            
            return loadUser(rows);
        }


        /// <summary>
        /// Returns a list of TUser instances given a user name.
        /// </summary>
        /// <param name="userName">User's name.</param>
        /// <returns></returns>
        public List<TUser> GetUserByName(string userName)
        {
            if (userName != null)
                userName = userName.ToLower();

            List<TUser> users = new List<TUser>();
            string commandText = "SELECT *  FROM " + fullTableName + " WHERE " + fieldUserName.Quoted() + " = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                users.Add(loadUser(rows));
            }

            return users;
        }

        /// <summary>
        /// Returns a list of TUser instances given a user email.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <returns></returns>
        public List<TUser> GetUserByEmail(string email)
        {
            //Due to PostgreSQL's case sensitivity, we have another column for the user name in lowercase.
            if (email != null)
                email = email.ToLower();

            List<TUser> users = new List<TUser>();
            string commandText = "SELECT *  FROM " + fullTableName + " WHERE " + fieldEmail.Quoted() + " = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@email", email } };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                users.Add(loadUser(rows));
            }

            return users;
        }

        /// <summary>
        /// Return the user's password hash.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {

            string commandText = "select " + fieldPassword.Quoted() + " from " + fullTableName + " where " + fieldID.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", userId);

            var passHash = _database.GetStrValue(commandText, parameters);
            if(string.IsNullOrEmpty(passHash))
            {
                return null;
            }

            return passHash;
        }

        /// <summary>
        /// Sets the user's password hash.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string userId, string passwordHash)
        {

            string commandText = "update " + fullTableName + " set " + fieldPassword.Quoted() + " = @pwdHash where " + fieldID.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@pwdHash", passwordHash);
            parameters.Add("@id", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns the user's security stamp.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userId)
        {

            string commandText = "select " + fieldSecurityStamp.Quoted() + " from " + fullTableName + " where " + fieldID.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };
            var result = _database.GetStrValue(commandText, parameters);

            return result;
        }

        /// <summary>
        /// Inserts a new user in the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(TUser user)
        {
            var lowerCaseEmail = user.Email == null ? null : user.Email.ToLower();

            string commandText = "insert into " + fullTableName +
                "(" + fieldID.Quoted() + ", " + 
                fieldUserName.Quoted() + ", " + 
                fieldPassword.Quoted() + ", " + 
                fieldSecurityStamp.Quoted() + ", " + 
                fieldEmail.Quoted() + ", " +
                fieldEmailConfirmed.Quoted() + ", " +
                fieldPhoneNumber.Quoted() + ", " +
                fieldPhoneNumberConfirmed.Quoted() + ", " +
                fieldTwoFactorEnabled.Quoted() + ", " +
                fieldLockoutEndDate.Quoted() + ", " +
                fieldLockoutEnabled.Quoted() + ", " +
                fieldAccessFailedCount.Quoted() + ")" +
                " VALUES (@id, @name, @pwdHash, @SecStamp, @email, @emailconfirmed, @phoneNumber, @phoneNumberConfirmed, @twoFactorEnabled, @lockoutEndDate, @lockoutEnabled, @AccessFailedCount);";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", user.UserName);
            parameters.Add("@id", user.Id);
            parameters.Add("@pwdHash", user.PasswordHash);
            parameters.Add("@SecStamp", user.SecurityStamp);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);
            parameters.Add("@phoneNumber", user.PhoneNumber);
            parameters.Add("@phoneNumberConfirmed", user.PhoneNumberConfirmed);
            parameters.Add("@twoFactorEnabled", user.TwoFactorEnabled);
            parameters.Add("@lockoutEndDate", user.LockoutEndDateUtc);
            parameters.Add("@lockoutEnabled", user.LockoutEnabled);
            parameters.Add("@AccessFailedCount", user.AccessFailedCount);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        private int Delete(string userId)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fieldID.Quoted()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(TUser user)
        {
            return Delete(user.Id);
        }

        /// <summary>
        /// Updates a user in the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(TUser user)
        {   
            string commandText = "UPDATE " + fullTableName + " set " +
                fieldUserName.Quoted() + " = @userName, " +
                fieldPassword.Quoted() + " = @pswHash, " +
                fieldSecurityStamp.Quoted() + " = @secStamp, " +
                fieldEmail.Quoted() + " = @email, " +
                fieldEmailConfirmed.Quoted() + " = @emailconfirmed, " +
                fieldPhoneNumber.Quoted() + " = @phoneNumber, " +
                fieldPhoneNumberConfirmed.Quoted() + " =  @phoneNumberConfirmed, "+
                fieldTwoFactorEnabled.Quoted() + " =  @twoFactorEnabled, "+
                fieldLockoutEndDate.Quoted() + " =  @lockoutEndDate, "+
                fieldLockoutEnabled.Quoted() + " =  @lockoutEnabled, "+
                fieldAccessFailedCount.Quoted() + " =  @AccessFailedCount "+
                " where " + fieldID.Quoted() + " = @userId";


            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userName", user.UserName);
            parameters.Add("@pswHash", user.PasswordHash);
            parameters.Add("@secStamp", user.SecurityStamp);
            parameters.Add("@userId", user.Id);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);
            parameters.Add("@phoneNumber", user.PhoneNumber);
            parameters.Add("@phoneNumberConfirmed", user.PhoneNumberConfirmed);
            parameters.Add("@twoFactorEnabled", user.TwoFactorEnabled);
            parameters.Add("@lockoutEndDate", user.LockoutEndDateUtc);
            parameters.Add("@lockoutEnabled", user.LockoutEnabled);
            parameters.Add("@AccessFailedCount", user.AccessFailedCount);
            return _database.Execute(commandText, parameters);
        }
    }
}