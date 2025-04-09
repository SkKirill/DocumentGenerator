using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentGenerator.Data.Models.DataBase;

public abstract class BaseDatabaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public BaseDatabaseModel()
    {
        CreatedAt = DateTime.Now;
    }
}