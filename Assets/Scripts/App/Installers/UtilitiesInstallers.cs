using App.Counters;
using Zenject;

namespace App.Installers
{
    public class UtilitiesInstallers : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Container.Bind<CounterUtility>().FromMethod(Method);
        }

        private CounterUtility Method()
        {
               return new CounterUtility();
        }
    }
}
