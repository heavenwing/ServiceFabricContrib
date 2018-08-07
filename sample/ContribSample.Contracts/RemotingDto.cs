using System;
using System.Collections.Generic;
using System.Text;

namespace ContribSample.Contracts
{
    public class PeopleDto
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public List<ToyDto> Toys { get; set; }
    }

    public class ToyDto
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }
}
