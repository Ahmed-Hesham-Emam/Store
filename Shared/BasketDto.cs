using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
    {
    public class BasketDto
        {
        public string Id { get; set; }
        public List<BasketItemDto> Items { get; set; }
        }
    }
