using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain.Entities.Common
{
	public class Pagination<T>
	{
		public List<T> Items { get; set; } = null!;
		public int TotalCount { get; set; }
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
	}
}
