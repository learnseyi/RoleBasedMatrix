using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoleBasedMatrix.Models
{

    /// <summary>
    /// Represents a support group
    /// </summary>
    public class SupportGroupsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        [Required]
        [DisplayName("Department Name")]
        public string GroupName { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string GroupDescription { get; set; } = string.Empty;

    }


    /// <summary>
    /// Represents a type of Resource (hardware or software)
    /// </summary>
    public class ResourcesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceId { get; set; }

        [Required]
        [ForeignKey("SupportGroupsModel")]
        public int SupportId { get; set; }

        [Required]
        [DisplayName("Resource Type")]
        public string ResourceType { get; set; } = string.Empty;

        [DisplayFormat(NullDisplayText = "No Description available")]
        public string ResourceDescription { get; set; } = string.Empty;

        [NotMapped]
        public List<ApplicationsModel>? ApplicationList { get; set; }
    }


    /// <summary>
    /// Represents an application
    /// </summary>
    public class ApplicationsModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppID { get; set; }

        [Required]
        [ForeignKey("ResourcesModel")]
        public int ResourceId { get; set; }

        [Required]
        [ForeignKey("SupportGroupsModel")]
        public int SupportId { get; set; }

        [Required]
        [DisplayName("Application Name")]
        public string ApplicationName { get; set; } = string.Empty;

        [DisplayName("Description")]
        [DisplayFormat(NullDisplayText = "No Description available")]
        public string ResourceDescription { get; set; } = string.Empty;


    }

    /// <summary>
    /// Represents permissions assigned to a role
    /// </summary>
    public class AppPermissionsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionsId { get; set; }

        [Required]
        [ForeignKey("RolesModel")]
        public int RoleId { get; set; }


        [Required]
        [ForeignKey("ApplicationsModel")]
        public int AppId { get; set; }


        [Required]
        [ForeignKey("AuthorizationsModel")]
        public int AuthId { get; set; }


        [DisplayName("Notes")]
        public string Notes { get; set; } = string.Empty;
    }


    /// <summary>
    /// Represents a Division
    /// </summary>
    public class DivisionsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DivisionId { get; set; }

        [Required]
        [DisplayName("Division")]
        public string DivisionName { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string? DivisionDescription { get; set; }

        [NotMapped]
        public IList<DepartmentsModel> DepartmentList { get; set; } = new List<DepartmentsModel>();
    }


    /// <summary>
    /// Represents a Department
    /// </summary>
    public class DepartmentsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }

        [Required]
        [DisplayName("Department Name")]
        public string DepartmentName { get; set; } = string.Empty;

        [DisplayName("Department Description")]
        public string DepartmentDescription { get; set; } = string.Empty;

        [NotMapped]
        public IList<RolesModel> RolesList { get; set; } = new List<RolesModel>();
    }

    /// <summary>
    /// Represents a role in a department
    /// </summary>
    public class RolesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [ForeignKey("DepartmentsModel")]
        public int DepartmentId { get; set; }

        [Required]
        [DisplayName("Role Name")]
        public string RoleName { get; set; } = string.Empty;

        [NotMapped]
        [DisplayName("Applications")]
        public List<ApplicationsModel> Applications { get; set; } = new List<ApplicationsModel>();

        [NotMapped]
        [DisplayName("Permissions")]
        public List<AppPermissionsModel> AppPermissions { get; set; } = new List<AppPermissionsModel>();
    }


    /// <summary>
    /// Represents an assignment table for resources (Software or Hardware) 
    /// </summary>
    public class ResourceAssignmentsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentId { get; set; }

        [Required]
        [ForeignKey("RolesModel")]
        public int RoleId { get; set; }

        [Required]
        [ForeignKey("ResourcesModel")]
        public int ResourceId { get; set; }
    }


    /// <summary>
    /// Represents asssignable permissions within an application
    /// </summary>
    public class AuthorizationsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthId { get; set; }

        [Required]
        [ForeignKey("ApplicationsModel")]
        public int AppId { get; set; }

        [Required]
        [DisplayName("Authorization Level")]
        public string AuthLabel { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string AuthDesc { get; set; } = string.Empty;
    }


    /// <summary>
    /// Represents options available to perform a search on the search tab
    /// </summary>
    public class SearchTabOptionsModel
    {
        [DisplayName("Division")]
        [Required(ErrorMessage = "Please select a Division")]
        public string DivisionName { get; set; } = string.Empty;

        [DisplayName("Department")]
        [Required(ErrorMessage = "Please select a Department")]
        public string DepartmentName { get; set; } = string.Empty;

        [DisplayName("Role")]
        [Required(ErrorMessage = "Please Select a role")]
        public string RoleName { get; set; } = string.Empty;
    }


    public class EditAssignmentModel
    {
        [NotMapped]
        public int Authid { get; set; }

        [NotMapped]
        public string AuthLabel { get; set; } = string.Empty;




    }
}
