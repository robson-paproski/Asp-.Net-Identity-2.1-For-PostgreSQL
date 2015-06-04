# AspNet.Identity.PostgreSQL
This project is an Asp.Net Identity Provider for work with PostgreSQL Database. It has been used [Npgsql](http://npgsql.projects.pgfoundry.org/), so, should work with most PostgreSQL versions.

The base for this project was [AspNet.Identity.PostgreSQL](https://github.com/danellos/AspNet.Identity.PostgreSQL) and [AspNet.Identity.MySQL](https://aspnet.codeplex.com/SourceControl/latest#Samples/Identity/AspNet.Identity.MySQL/) with a lot of modifications to make more simple to use your own database structure. For example, in every Table Class, you gonna see all the consts used for that class. The fields used in this project is similar to Asp.Net Identity 2.1

If you wanna to make a change in the name of the field used by this provider, you should modify the consts located in each table class, and reflect this in your database. This project doesn't use CodeFirst.

To use this provider in your project, follow these steps: 

1. Add the AspNet.Identity.PostgreSQL project as reference to your main MVC project.
2. In your MVC project, replace all references to "using Microsoft.AspNet.Identity.EntityFramework;" with "using AspNet.Identity.PostgreSQL;".
3. In IdentityModel.cs, ensure that that ApplicationDbContext inherits from "PostgreSQLDatabase".
4. Ensure that Web.config has your PostgreSQL connection string included in "DefaultConnection". For example:   ``` <connectionStrings>
    <add name="DefaultConnection" connectionString="Server=127.0.0.1;Port=5432;Database=DATABASE;User Id=USER;Password=PASSWORD;" providerName="System.Data.SqlClient"/>
  </connectionStrings>```. If you have a different schema, you should modify the Consts file in this project, or pass a different parameter on connectionString. [Click here for more info](https://www.connectionstrings.com/npgsql/). [Avaliable Parameters](http://npgsql.projects.pgfoundry.org/docs/api/Npgsql.NpgsqlConnection.ConnectionString.html)
5. In Solution Explorer, right-click your MVC5 project and select "Manage NuGet Packages". In the search text box dialog, type "Identity.EntityFramework". Select this package in the list of results and click "Uninstall". You will be prompted to uninstall the dependency package EntityFramework. Click on Yes as we will no longer need this package.
6. Lastly, execute the SQL script found in the solution against your database in pgAdmin (or via the command line).

I hope you enjoy =D
