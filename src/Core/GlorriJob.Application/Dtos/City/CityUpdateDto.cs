using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Application.Dtos.City
{
    public record CityUpdateDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
