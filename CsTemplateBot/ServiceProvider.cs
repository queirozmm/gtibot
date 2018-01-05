using NLog;
using CsTemplateBot.Decorators;
using CsTemplateBot.NoActionHandlers;
using CsTemplateBot.Services;
using CsTemplateBot.Extensions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Services;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.ContainerProvider;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationExtensions;

namespace CsTemplateBot
{
    public class ServiceProvider : Container, Takenet.MessagingHub.Client.Host.IServiceContainer
    {
        public ServiceProvider()
        {
			// Here is a good place to register new Lime document types, too: 
			//TypeUtil.RegisterDocument<MyDocument>();

            Options.AllowOverridingRegistrations = true;
            ContainerProvider.DefaultRegister(this);
            RegisterDecorator<INavigationExtension, MetricsNavigationExtension>(Lifestyle.Singleton);
            RegisterSingleton<ILogger>(LogManager.GetLogger($"Bot{Startup.LogApplicationName}"));
			RegisterSingleton<IContextManager, ContextManager>();
			RegisterSingleton<IEventNotificator, EventTrackService>();
			RegisterSingleton<IContactService, ContactService>();
            RegisterSingleton<INoAction, DictionaryNoAction>();
			RegisterSingleton<IGenericErrorService, GenericErrorService>();
			RegisterSingleton<IMpaService, MpaService>();
			RegisterSingleton<IMediaService, MediaService>();
            RegisterCollection<INoActionHandler>(new[]
            {
                typeof(CommandsNoActionHandler)
            });
			RegisterSingleton<INLPService, NLPService>();
        }

        public void RegisterService(Type serviceType, Func<object> instanceFactory)
        {
            RegisterSingleton(serviceType, instanceFactory);
        }

        public void RegisterService(Type serviceType, object instance)
        {
            RegisterSingleton(serviceType, instance);

            if (serviceType == typeof(MySettings))
            {
                var settings = (MySettings)instance;
                var navSettings = new NavigationSettings(settings.MPASettings, settings.TalkServiceSettings);
                RegisterSingleton(settings.MPASettings);
                RegisterSingleton(settings.TalkServiceSettings);
                RegisterSingleton(navSettings);
            }
        }
    }
}

