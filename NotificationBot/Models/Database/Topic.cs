using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationsBot.Models.Database
{
    [PrimaryKey(nameof(Id), nameof(ChatId))]
    public class Topic
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public long ChatId { get; set; }

        [ForeignKey("Projects")]
        public int? ProjectsId { get; set; }
    }
}
