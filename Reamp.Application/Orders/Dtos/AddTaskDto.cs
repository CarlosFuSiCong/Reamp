using System.ComponentModel.DataAnnotations;
using Reamp.Domain.Shoots.Enums;

namespace Reamp.Application.Orders.Dtos
{
    public class AddTaskDto
    {
        [Required]
        public ShootTaskType Type { get; set; }

        public string? Notes { get; set; }

        public decimal? Price { get; set; }
    }
}



