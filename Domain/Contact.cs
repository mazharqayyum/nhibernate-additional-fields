using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NibernateAditionalFields.Domain
{
    public partial class Contact
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }

        public virtual IDictionary Additional { get; set; }
    }
}
