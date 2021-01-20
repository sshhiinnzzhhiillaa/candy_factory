using Microsoft.Extensions.DependencyInjection;

namespace _task8
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.ServiceProvider.GetRequiredService<MainViewModel>();
    }
}
