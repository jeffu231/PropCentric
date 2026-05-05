using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vixen.Annotations;

namespace Vixen.Model
{
    public class BindableBase: INotifyPropertyChanged
    {
		protected virtual void SetProperty<T>(ref T member, T val,
            [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(member, val)) return;

            member = val;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
	}
}