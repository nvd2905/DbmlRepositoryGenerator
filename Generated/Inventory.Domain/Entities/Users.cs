//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Inventory.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Entities;
[Table("users")]
public class Users
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    
    public int Id { get; set; }
    
    
    
    
    [Column("username")]
    [Required]
    public string Username { get; set; }
    
    
    
    
    [Column("email")]
    [Required]
    public string Email { get; set; }
    
    
    
    
    [Column("password_hash")]
    [Required]
    public string PasswordHash { get; set; }
    
    
    
    
    [Column("created_at")]
    
    public DateTime CreatedAt { get; set; }
    
    
    
    
    [Column("updated_at")]
    
    public DateTime? UpdatedAt { get; set; }
    
    
    
    
    [Column("is_active")]
    
    public bool IsActive { get; set; }
    
    
}