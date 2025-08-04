//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Inventory.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Entities;
[Table("posts")]
public class Posts
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    
    public int Id { get; set; }
    
    
    
    
    [Column("title")]
    [Required]
    public string Title { get; set; }
    
    
    
    
    [Column("content")]
    
    public string Content { get; set; }
    
    
    
    
    [Column("user_id")]
    
    public int UserId { get; set; }
    
    
    
    
    [Column("published_at")]
    
    public DateTime? PublishedAt { get; set; }
    
    
    
    
    [Column("created_at")]
    
    public DateTime CreatedAt { get; set; }
    
    
    
    
    [Column("updated_at")]
    
    public DateTime? UpdatedAt { get; set; }
    
    
}