using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }



        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }



        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
