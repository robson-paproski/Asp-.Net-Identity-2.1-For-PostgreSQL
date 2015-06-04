using System;
using System.Collections.Generic;


namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Classe que representa a tabela AspNetUserRoles na base PostgreSQL
    /// </summary>
    public class UserRolesTable
    {
        private PostgreSQLDatabase _database;

        internal const string tableName = "AspNetUserRoles";
        internal const string fieldUserID = "UserId";
        internal const string fieldRoleID = "RoleId";
        internal static string fullTableName = Consts.Schema.Quoted() + "." + tableName.Quoted();
        

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserRolesTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Retorna uma lista de Roles do Usuário em questão
        /// </summary>
        /// <param name="userId">Código do Usuário</param>
        /// <returns></returns>
        public List<string> FindByUserId(string userId)
        {
            List<string> roles = new List<string>();
            //TODO: This probably does not work, and may need testing.

            string commandText = "select AspRoles." + RoleTable.fieldName.Quoted() + " from " + UserTable<IdentityUser>.fullTableName + " AspUsers" +
                " INNER JOIN " + fullTableName + " AspUserRoles " +
                " ON AspUsers." + UserTable<IdentityUser>.fieldID.Quoted() + " = AspUserRoles." + fieldUserID.Quoted() +
                " INNER JOIN " + RoleTable.fullTableName + " AspRoles " +
                " ON AspUserRoles." + fieldRoleID.Quoted() + " = AspRoles." + RoleTable.fieldId.Quoted() +
                " where AspUsers." + UserTable<IdentityUser>.fieldID.Quoted() + " = @userId";
                        
            /*select AspNetRoles.Name from AspNetUsers
             * inner join AspNetUserRoles
             * ON AspNetUsers.ID = AspNetUserRoles.UserID
             * inner join AspNetRoles
             * ON aspNetUserRoles.RoleID = AspNetRoles.ID
             * where AspNetUser.ID = :id
            */

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                roles.Add(row[fieldRoleID]);
            }

            return roles;
        }

        /// <summary>
        /// Deletes all roles from a user in the AspNetUserRoles table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fieldUserID.Quoted()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role record for a user in the UserRoles table.
        /// </summary>
        /// <param name="user">The User.</param>
        /// <param name="roleId">The Role's id.</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, string roleId)
        {
            string commandText = "INSERT INTO "+fullTableName+" ("+fieldUserID.Quoted()+", "+fieldRoleID.Quoted()+") VALUES (@userId, @roleId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("roleId", roleId);

            return _database.Execute(commandText, parameters);
        }
    }
}
