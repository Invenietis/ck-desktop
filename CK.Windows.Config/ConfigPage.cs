using Caliburn.Micro;

namespace CK.Windows.Config
{

    public class ConfigPage : Screen, IConfigItemContainer
    {
        ConfigManager _configManager;
        string _description;
        BindableCollection<object> _items;

        public ConfigPage( ConfigManager configManager )
        {
            _configManager = configManager;
            _items = new BindableCollection<object>();
        }

        public ConfigManager ConfigManager { get { return _configManager; } }

        public string Description
        {
            get { return _description; }
            set
            {
                if( _description != value )
                {
                    _description = value;
                    NotifyOfPropertyChange( "Description" );
                }
            }
        }

        public IObservableCollection<object> Items { get { return _items; } }

    }


}
