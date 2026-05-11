using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ChMS.Modules.Auth.Core.DTOs
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
