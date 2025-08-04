//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Inventory.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Entities;
[Table("comments")]
public class Comments
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    
    public int Id { get; set; }
    
    
    
    
    [Column("content")]
    [Required]
    public string Content { get; set; }
    
    
    
    
    [Column("post_id")]
    
    public int PostId { get; set; }
    
    
    
    
    [Column("user_id")]
    
    public int UserId { get; set; }
    
    
    
    
    [Column("created_at")]
    
    public DateTime CreatedAt { get; set; }
    
    
}