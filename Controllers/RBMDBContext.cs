using Microsoft.AspNetCore.Mvc;
using System.Data;
using RoleBasedMatrix.Models;
using System.Data.SqlClient;
using System.Text.Json.Nodes;
//using Microsoft.TeamFoundation.Build.WebApi;

namespace RoleBasedMatrix.Database
{
    /// <summary>
    /// Represents an insatnce of a database connection
    /// </summary>
    public class RBMDBContext : Controller
    {
        private readonly IConfiguration? _configuration;
        public SqlConnection? con;


        public RBMDBContext(IConfiguration configuration)
        {
            this._configuration = configuration;

        }


        /// <summary>
        /// Initializes a database connection
        /// </summary>
        public bool ConnectToDataBase()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("RBMDB");
                con = new(connectionString);
                con.Open();
                return true;


            }
            catch 
            {
                throw;

            }


        }


        /// <summary>
        /// Queries a database to get permissions assigned to a role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string QueryApplicationsAndPermissions(int id)
        {

            ConnectToDataBase();

            SqlCommand cmd = new("spGetAppAndPermissions", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            DataSet _dataSet = new();
            SqlDataAdapter adapter = new(cmd);
            DataTable response = new();
            adapter.Fill(response);
            con?.Close();

            List<object> rows = new List<object>();

            foreach (DataRow dr in response.Rows)
            {

                foreach (DataColumn col in response.Columns)
                {
                    rows.Add(dr[col]);
                }
            }
            string result = (string)rows[0];
            return result;
        }

        /// <summary>
        /// Querys database for departments and roles
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>DataTable</returns>
        private DataTable QueryDepartmentsAndRoles(string tableName, int value)
        {
            string query = string.Empty;
            try
            {
                switch (tableName)
                {
                    case "DivisionName":
                        query = $"Select * from Departments where DIVISION_ID = {value}";
                        break;
                    case "DepartmentName":
                        query = $"Select * from Roles where DEPARTMENT_ID = {value}";
                        break;
                    default:
                        query = $"Select * from Divisions";
                        break;

                }

                ConnectToDataBase();
                SqlCommand cmd = new(query, con);
                DataSet _dataSet = new();
                SqlDataAdapter adapter = new(cmd);
                DataTable response = new();
                adapter.Fill(response);
                con?.Close();
                return response;

            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Gets the list of Divisions
        /// </summary>
        /// <returns>List of Divisions</returns>
        public IList<DivisionsModel> GetDivisions(string condition = "load", int value = 0)
        {
            IList<DivisionsModel>? divisions = new List<DivisionsModel>();

            DataTable dbResponse = QueryDepartmentsAndRoles(condition, value);
            foreach (DataRow row in dbResponse.Rows)
            {
                divisions.Add(new DivisionsModel()
                {
                    DivisionId = Convert.ToInt32(row["DIVISION_ID"]),
                    DivisionName = ((string)row["DIVISION_NAME"]).Trim(),
                    DivisionDescription = String.IsNullOrEmpty(Convert.ToString(row["DIVISION_DESC"])) ? "null" : ((string)row["DIVISION_DESC"]).Trim()

                }
                    );
            }

            return divisions;

        }

        /// <summary>
        /// Gets the list of Departments
        /// </summary>
        /// <returns>List of sd]['xDepartments</returns>

        public IList<DepartmentsModel> GetDepartmentList(string condition, int value)
        {
            IList<DepartmentsModel> departments = new List<DepartmentsModel>();
            DataTable dbResponse = QueryDepartmentsAndRoles(condition, value);
            foreach (DataRow r in dbResponse.Rows)
            {
                departments.Add(new DepartmentsModel()
                {
                    DepartmentId = Convert.ToInt32(r["DEPARTMENT_ID"]),
                    DepartmentName = ((string)r["DEPARTMENT_NAME"]).Trim(),
                    DepartmentDescription = String.IsNullOrEmpty(Convert.ToString(r["DEPARTMENT_DESC"])) ? "null" : ((string)r["DIVISION_DESC"]).Trim()

                }
                     );
            };


            return departments;

        }

        /// <summary>
        /// Gets the list of Roles
        /// </summary>
        /// <param name="departmentid"></param>
        /// <returns></returns>

        public IList<RolesModel> GetRoles(int departmentid)
        {
            string query = $"Select * from Roles where DEPARTMENT_ID = {departmentid}";
            IList<RolesModel> roles = new List<RolesModel>();
            try
            {
                ConnectToDataBase();
                SqlCommand cmd = new SqlCommand(query, con);
                DataSet _dataSet = new();
                SqlDataAdapter adapter = new(cmd);
                DataTable response = new();
                adapter.Fill(response);
                foreach (DataRow r in response.Rows)
                {
                    roles.Add(new RolesModel()
                    {
                        RoleId = Convert.ToInt32(r["ROLE_ID"]),
                        DepartmentId = Convert.ToInt32(r["DEPARTMENT_ID"]),
                        RoleName = ((string)r["ROLE_NAME"]).Trim()

                    }
                        );
                };

                con?.Close();
                return roles;

            }
            catch
            {
                throw;
            }

        }

        public IList<string> GetCurrentPermissions(string appName, string role)
        {
            IList<string> permissions = new List<string>();
            try
            {
                ConnectToDataBase();
                SqlCommand cmd = new("spGetCurrentPermissions", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("appName", appName);
                cmd.Parameters.AddWithValue("roleName", role);
                DataSet _dataSet = new();
                SqlDataAdapter adapter = new(cmd);
                DataTable response = new();
                adapter.Fill(response);
                con?.Close();
                foreach (DataRow r in response.Rows)
                {
                    permissions.Add((string)r["AUTH_LABEL"]);
                };

                return permissions;

            }
            catch
            {
                throw;
            }

        }


        /// <summary>
        /// Returns assigned application and permissions assigned
        /// </summary>
        public string GetAssignedAppsAndPermission(int id)
        {
            string dbResponse = QueryApplicationsAndPermissions(id);

            return dbResponse;
        }

        public IList<EditAssignmentModel> GetAppAuthorizations(string name)
        {
            ConnectToDataBase();
            IList<EditAssignmentModel> assignment = new List<EditAssignmentModel>();
            SqlCommand cmd = new("spGetAppAuthorizations", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("app_name", name);
            DataSet _dataSet = new();
            SqlDataAdapter adapter = new(cmd);
            DataTable response = new();
            adapter.Fill(response);
            con?.Close();

            foreach (DataRow dr in response.Rows)
            {

                assignment.Add(
                    new EditAssignmentModel()
                    {
                        Authid = (Int32)dr["AUTH_ID"],
                        AuthLabel = (string)dr["AUTH_LABEL"]
                    }

                    );
            }

            return assignment;
        }

    }

}