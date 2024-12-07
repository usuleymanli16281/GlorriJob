using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlorriJob.Domain.Entities.Common;

namespace GlorriJob.Domain.Entities
{
    public class Biography : BaseEntity
    {
        public string? Icon { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}
