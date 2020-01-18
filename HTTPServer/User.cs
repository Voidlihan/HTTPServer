using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPServer
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
