using System;
using System.Collections.Generic;
using System.Configuration;
using Ninject;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;

namespace SportsStore.WebUI.Infrastructure {
    public class NinjectDependencyResolver : IDependencyResolver {
        private IKernel kernal;

        private void AddBindings () {
            kernal.Bind<IProductsRepository> ().To<EFProductRepository> ();

            EmailSettings emailSettings = new EmailSettings {
                WriteAsFile = bool.Parse(ConfigurationManager
                    .AppSettings["Email.WriteAsFile"] ?? "false")
            };

            kernal.Bind<IOrderProcessor>().To<EmailOrderProcessor>()
                .WithConstructorArgument(emailSettings);

            kernal.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }

        public NinjectDependencyResolver (IKernel kernal) {
            this.kernal = kernal;
            AddBindings ();
        }

        public object GetService (Type serviceType) {
            return kernal.TryGet (serviceType);
        }

        public IEnumerable<object> GetServices (Type serviceType) {
            return kernal.GetAll (serviceType);
        }
    }
}