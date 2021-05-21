using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forester.Models
{
    public struct VerifiedApplication
    {
        public VerificationKey verificationKey { get; set; }
        public Application application { get; set; }
    }
}
