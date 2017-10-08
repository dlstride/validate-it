using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateValidator.Validators;
using ValidationServer.Data.Validators;

namespace TemplateValidator
{
    public class ConfigModule : NinjectModule
    {
        public override void Load()
        {
            this.Kernel.Bind<IValidatorProxy>().To<TemplateValidatorProxy>().InTransientScope();

            this.Kernel.Bind<TemplateTestValidator>().ToSelf().InTransientScope();
        }
    }
}
